using Franz.Common.Business.Domain;
using Franz.Common.Business.Repositories;
using Franz.Common.EntityFramework.Auditing;
using Franz.Common.EntityFramework.Extensions;
using Franz.Common.Mediator.Dispatchers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Metadata;
using PilotageForfaits.Domain.Entites;
using PilotageForfaits.Infrastructure.Donnees;

namespace PilotageForfaits.Infrastructure.Tests;

/// <summary>
/// Protects persistence invariants that must survive the Franz persistence migration.
/// These tests inspect the EF model without requiring a running SQL Server instance.
/// </summary>
public sealed class PilotageDbContextModelTests
{
    private static PilotageDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PilotageDbContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=PilotageForfaitsModelTests;Trusted_Connection=True;")
            .Options;

        var dispatcherServices = new ServiceCollection().BuildServiceProvider();
        return new PilotageDbContext(options, new FranzDispatcher(dispatcherServices));
    }

    private static PilotageDbContext CreateSqliteContext(SqliteConnection connection, string? userId)
    {
        var options = new DbContextOptionsBuilder<PilotageDbContext>()
            .UseSqlite(connection)
            .Options;

        var dispatcherServices = new ServiceCollection().BuildServiceProvider();
        return new PilotageDbContext(options, new FranzDispatcher(dispatcherServices),
            new FixedCurrentUserService(userId));
    }

    private sealed class FixedCurrentUserService(string? userId) : ICurrentUserService
    {
        public string? UserId => userId;
    }

    [Fact]
    public void Astrolaba_entity_base_extends_Franz_without_redeclaring_lifecycle_state()
    {
        Assert.Equal(typeof(Entity<Guid>), typeof(Entite).BaseType);

        var declaredProperties = typeof(Entite)
            .GetProperties(System.Reflection.BindingFlags.Instance |
                           System.Reflection.BindingFlags.Public |
                           System.Reflection.BindingFlags.DeclaredOnly)
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);

        Assert.Equal(new[] { nameof(Entite.Version) }, declaredProperties);

        var franzLifecycleProperties = new[]
        {
            "Id",
            "IsDeleted",
            "DateCreated",
            "LastModifiedDate",
            "CreatedBy",
            "LastModifiedBy",
            "DateDeleted",
            "DeletedBy"
        };

        Assert.All(franzLifecycleProperties, property =>
            Assert.DoesNotContain(property, declaredProperties));
    }

    [Fact]
    public void Every_persisted_entity_uses_Franz_IEntity_and_framework_auditing()
    {
        using var context = CreateContext();

        var entities = context.Model.GetEntityTypes()
            .Where(type => !type.IsOwned())
            .ToArray();

        Assert.NotEmpty(entities);
        Assert.All(entities, entityType =>
        {
            Assert.Contains(entityType.ClrType.GetInterfaces(), contract =>
                contract.IsGenericType &&
                contract.GetGenericTypeDefinition() == typeof(IEntity<>));

            Assert.NotNull(entityType.FindProperty(nameof(IEntity<Guid>.IsDeleted)));
            Assert.NotNull(entityType.FindProperty(nameof(IEntity<Guid>.DateCreated)));
            Assert.NotNull(entityType.FindProperty(nameof(Entity<Guid>.CreatedBy)));
            Assert.NotEmpty(entityType.GetDeclaredQueryFilters());
        });
    }

    [Fact]
    public void Franz_entity_repositories_are_registered_for_every_persisted_entity()
    {
        var services = new ServiceCollection();
        services.AddEntityRepositories<PilotageDbContext>();

        var registeredRepositories = services
            .Where(descriptor => descriptor.ServiceType.IsGenericType &&
                                 descriptor.ServiceType.GetGenericTypeDefinition() == typeof(IEntityRepository<,>))
            .Select(descriptor => descriptor.ServiceType)
            .ToHashSet();

        using var context = CreateContext();
        var persistedTypes = context.Model.GetEntityTypes()
            .Where(type => !type.IsOwned() &&
                           type.ClrType.GetInterfaces().Any(contract =>
                               contract.IsGenericType &&
                               contract.GetGenericTypeDefinition() == typeof(IEntity<>)))
            .ToArray();

        Assert.NotEmpty(persistedTypes);
        Assert.All(persistedTypes, entityType =>
        {
            var entityContract = entityType.ClrType.GetInterfaces().Single(contract =>
                contract.IsGenericType &&
                contract.GetGenericTypeDefinition() == typeof(IEntity<>));
            var repositoryContract = typeof(IEntityRepository<,>)
                .MakeGenericType(entityType.ClrType, entityContract.GetGenericArguments()[0]);

            Assert.Contains(repositoryContract, registeredRepositories);
        });
    }

    [Fact]
    public void New_domain_entities_keep_generated_Guid_identity()
    {
        Assert.NotEqual(Guid.Empty, new Projet().Id);
        Assert.NotEqual(Guid.Empty, new Utilisateur().Id);
        Assert.NotEqual(Guid.Empty, new Tache().Id);
    }

    [Fact]
    public async Task SaveChanges_applies_Franz_auditing_and_soft_deletion()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var context = CreateSqliteContext(connection, "entra-object-id");
        await context.Database.EnsureCreatedAsync();

        var user = new Utilisateur
        {
            IdentifiantExterne = "subject-1",
            Nom = "Example",
            Prenom = "Test",
            Email = "test@example.invalid",
            CreeLe = DateTimeOffset.UtcNow
        };

        context.Utilisateurs.Add(user);
        await context.SaveChangesAsync();

        Assert.Equal(1, user.Version);
        Assert.Equal("entra-object-id", user.CreatedBy);
        Assert.NotEqual(DateTimeOffset.MinValue, user.DateCreated);
        Assert.False(user.IsDeleted);

        user.Nom = "Updated";
        await context.SaveChangesAsync();

        Assert.Equal(2, user.Version);
        Assert.Equal("entra-object-id", user.LastModifiedBy);
        Assert.True(user.LastModifiedDate >= user.DateCreated);

        context.Utilisateurs.Remove(user);
        await context.SaveChangesAsync();

        Assert.Equal(3, user.Version);
        Assert.True(user.IsDeleted);
        Assert.Equal("entra-object-id", user.DeletedBy);
        Assert.Null(await context.Utilisateurs.SingleOrDefaultAsync(entity => entity.Id == user.Id));
        Assert.NotNull(await context.Utilisateurs.IgnoreQueryFilters()
            .SingleOrDefaultAsync(entity => entity.Id == user.Id));
    }

    [Fact]
    public async Task Explicit_project_removal_remains_a_transactional_permanent_delete()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var context = CreateSqliteContext(connection, "entra-object-id");
        await context.Database.EnsureCreatedAsync();

        var project = new Projet
        {
            Client = "Client",
            Nom = "Projet de test",
            CreeLe = DateTimeOffset.UtcNow
        };

        context.Projets.Add(project);
        await context.SaveChangesAsync();
        var projectId = project.Id;

        // A dependent row proves the permanent-delete path orders children before the project.
        var field = new ChampFiche
        {
            ProjetId = projectId,
            Libelle = "Client",
            Valeur = "Example"
        };
        context.ChampsFiche.Add(field);
        await context.SaveChangesAsync();
        var fieldId = field.Id;

        await context.MarquerProjetPourSuppressionAsync(projectId);
        await context.SaveChangesAsync();

        Assert.Null(await context.ChampsFiche.IgnoreQueryFilters()
            .SingleOrDefaultAsync(entity => entity.Id == fieldId));
        Assert.Null(await context.Projets.IgnoreQueryFilters()
            .SingleOrDefaultAsync(entity => entity.Id == projectId));
    }

    [Fact]
    public void Every_persisted_domain_entity_uses_optimistic_concurrency()
    {
        using var context = CreateContext();

        var entities = context.Model.GetEntityTypes()
            .Where(type => !type.IsOwned() && typeof(Entite).IsAssignableFrom(type.ClrType))
            .ToArray();

        Assert.NotEmpty(entities);
        Assert.All(entities, entityType =>
        {
            var version = entityType.FindProperty(nameof(Entite.Version));
            Assert.NotNull(version);
            Assert.True(version!.IsConcurrencyToken,
                $"{entityType.ClrType.Name}.{nameof(Entite.Version)} must be a concurrency token.");
        });
    }

    [Fact]
    public void Relationships_must_not_enable_database_cascade_delete()
    {
        using var context = CreateContext();

        var foreignKeys = context.Model.GetEntityTypes()
            .SelectMany(entityType => entityType.GetForeignKeys())
            .Where(foreignKey => !foreignKey.IsOwnership)
            .ToArray();

        Assert.NotEmpty(foreignKeys);
        Assert.All(foreignKeys, foreignKey =>
            Assert.Equal(DeleteBehavior.Restrict, foreignKey.DeleteBehavior));
    }

    [Fact]
    public void Decimal_columns_must_keep_financial_and_planning_precision()
    {
        using var context = CreateContext();

        var decimalProperties = context.Model.GetEntityTypes()
            .SelectMany(entityType => entityType.GetProperties())
            .Where(property => (Nullable.GetUnderlyingType(property.ClrType) ?? property.ClrType) == typeof(decimal))
            .ToArray();

        Assert.NotEmpty(decimalProperties);
        Assert.All(decimalProperties, property =>
        {
            Assert.Equal(18, property.GetPrecision());
            Assert.Equal(6, property.GetScale());
        });
    }

    [Fact]
    public void Project_owned_entities_must_have_a_project_foreign_key()
    {
        using var context = CreateContext();

        var projectEntities = context.Model.GetEntityTypes()
            .Where(entityType => !entityType.IsOwned() &&
                                 typeof(EntiteDeProjet).IsAssignableFrom(entityType.ClrType))
            .ToArray();

        Assert.NotEmpty(projectEntities);
        Assert.All(projectEntities, entityType =>
        {
            var projectForeignKey = entityType.GetForeignKeys()
                .SingleOrDefault(foreignKey =>
                    foreignKey.Properties.Any(property => property.Name == nameof(EntiteDeProjet.ProjetId)));

            Assert.NotNull(projectForeignKey);
            Assert.Contains(projectForeignKey!.Properties, property =>
                property.Name == nameof(EntiteDeProjet.ProjetId));
            Assert.Equal(DeleteBehavior.Restrict, projectForeignKey.DeleteBehavior);
        });
    }
}
