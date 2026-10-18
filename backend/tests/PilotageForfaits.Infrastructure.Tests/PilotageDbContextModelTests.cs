using Franz.Common.Mediator.Dispatchers;
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
