using Franz.Common.EntityFramework.Auditing;
using Franz.Common.Mediator.Dispatchers;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PilotageForfaits.Domain.Entites;
using PilotageForfaits.Infrastructure.Donnees;
using PilotageForfaits.Infrastructure.Persistence;
using Xunit;

namespace PilotageForfaits.Infrastructure.Tests;

public sealed class PostgreSqlFactAttribute : FactAttribute
{
    public PostgreSqlFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASTROLABA_POSTGRES")))
            Skip = "Set ASTROLABA_POSTGRES to run PostgreSQL provider integration tests.";
    }
}

/// <summary>
/// Provider-level verification against PostgreSQL. Each test migrates a fresh database so it
/// exercises the production provider and initial migration rather than EnsureCreated/SQLite.
/// </summary>
public sealed class PostgreSqlPersistenceIntegrationTests
{
    private static PilotageDbContext CreateContext(string connectionString, string actor = "integration-test-actor")
    {
        var options = new DbContextOptionsBuilder<PilotageDbContext>()
            .UseNpgsql(connectionString, postgres => postgres.EnableRetryOnFailure())
            .Options;
        var dispatcherServices = new ServiceCollection().BuildServiceProvider();
        return new PilotageDbContext(options, new FranzDispatcher(dispatcherServices), new FixedCurrentUserService(actor));
    }

    private static async Task WithFreshDatabaseAsync(Func<string, Task> action)
    {
        var template = Environment.GetEnvironmentVariable("ASTROLABA_POSTGRES");
        Assert.False(string.IsNullOrWhiteSpace(template));

        var target = new NpgsqlConnectionStringBuilder(template!)
        {
            Database = $"astrolaba_integration_{Guid.NewGuid():N}"
        };
        var databaseName = target.Database;
        var admin = new NpgsqlConnectionStringBuilder(template!)
        {
            Database = "postgres"
        };

        try
        {
            await using (var adminConnection = new NpgsqlConnection(admin.ConnectionString))
            {
                await adminConnection.OpenAsync();
                await using var create = adminConnection.CreateCommand();
                create.CommandText = $"CREATE DATABASE \"{databaseName}\"";
                await create.ExecuteNonQueryAsync();
            }

            await using (var migrationContext = CreateContext(target.ConnectionString))
                await migrationContext.Database.MigrateAsync();

            await action(target.ConnectionString);
        }
        finally
        {
            await using var adminConnection = new NpgsqlConnection(admin.ConnectionString);
            await adminConnection.OpenAsync();
            await using var drop = adminConnection.CreateCommand();
            drop.CommandText = $"DROP DATABASE IF EXISTS \"{databaseName}\" WITH (FORCE)";
            await drop.ExecuteNonQueryAsync();
        }
    }

    [PostgreSqlFact]
    public async Task Fresh_migration_applies_audit_soft_delete_and_optimistic_concurrency()
    {
        await WithFreshDatabaseAsync(async connectionString =>
        {
            var userId = Guid.NewGuid();
            await using (var seed = CreateContext(connectionString))
            {
                var user = CreateUser("Concurrency");
                seed.Utilisateurs.Add(user);
                await seed.SaveChangesAsync();

                userId = user.Id;
                Assert.Equal("integration-test-actor", user.CreatedBy);
                Assert.NotEqual(DateTimeOffset.MinValue, user.DateCreated);
                Assert.False(user.IsDeleted);
                Assert.Equal(1, user.Version);
            }

            await using var first = CreateContext(connectionString);
            await using var second = CreateContext(connectionString);
            var firstCopy = await first.Utilisateurs.SingleAsync(user => user.Id == userId);
            var secondCopy = await second.Utilisateurs.SingleAsync(user => user.Id == userId);

            firstCopy.Nom = "First writer";
            await first.SaveChangesAsync();
            secondCopy.Nom = "Stale writer";
            await Assert.ThrowsAsync<PilotageForfaits.Application.Commun.ConflitException>(() => second.SaveChangesAsync());

            await using var deletion = CreateContext(connectionString);
            var persisted = await deletion.Utilisateurs.SingleAsync(user => user.Id == userId);
            deletion.Utilisateurs.Remove(persisted);
            await deletion.SaveChangesAsync();

            Assert.True(persisted.IsDeleted);
            Assert.Equal("integration-test-actor", persisted.DeletedBy);
            Assert.NotEqual(DateTimeOffset.MinValue, persisted.DateDeleted);
            Assert.Null(await deletion.Utilisateurs.SingleOrDefaultAsync(user => user.Id == userId));
            Assert.NotNull(await deletion.Utilisateurs.IgnoreQueryFilters()
                .SingleOrDefaultAsync(user => user.Id == userId));
        });
    }

    [PostgreSqlFact]
    public async Task Explicit_transaction_rolls_back_saved_entities()
    {
        await WithFreshDatabaseAsync(async connectionString =>
        {
            Guid userId = Guid.Empty;
            await using (var context = CreateContext(connectionString))
            {
                var strategy = context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await context.Database.BeginTransactionAsync();
                    var user = CreateUser("Rollback");
                    userId = user.Id;
                    context.Utilisateurs.Add(user);
                    await context.SaveChangesAsync();
                    await transaction.RollbackAsync();
                });
            }

            await using var verification = CreateContext(connectionString);
            Assert.Null(await verification.Utilisateurs.IgnoreQueryFilters()
                .SingleOrDefaultAsync(user => user.Id == userId));
        });
    }

    [PostgreSqlFact]
    public async Task Journal_writer_uses_the_same_unit_of_work_as_business_mutations()
    {
        await WithFreshDatabaseAsync(async connectionString =>
        {
            Guid projectId = Guid.Empty;
            await using (var context = CreateContext(connectionString))
            {
                var strategy = context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await context.Database.BeginTransactionAsync();
                    var project = new Projet
                    {
                        Client = "Client",
                        Nom = "Projet avec journal",
                        CreeLe = DateTimeOffset.UtcNow
                    };
                    context.Projets.Add(project);
                    projectId = project.Id;

                    var writer = new JournalWriter(context);
                    writer.Ajouter(new EntreeJournal
                    {
                        ProjetId = projectId,
                        Le = DateTime.UtcNow,
                        Objet = "Projet",
                        ObjetId = projectId,
                        Champ = "Nom",
                        NouvelleValeur = project.Nom
                    });

                    await context.SaveChangesAsync();
                    await transaction.RollbackAsync();
                });
            }

            await using var verification = CreateContext(connectionString);
            Assert.Null(await verification.Projets.IgnoreQueryFilters()
                .SingleOrDefaultAsync(project => project.Id == projectId));
            Assert.Empty(await verification.Journal.IgnoreQueryFilters()
                .Where(entry => entry.ProjetId == projectId)
                .ToListAsync());
        });
    }

    [PostgreSqlFact]
    public async Task Permanent_project_removal_deletes_dependent_rows()
    {
        await WithFreshDatabaseAsync(async connectionString =>
        {
            Guid projectId;
            Guid fieldId;
            await using (var context = CreateContext(connectionString))
            {
                var project = new Projet
                {
                    Client = "Client",
                    Nom = "Projet PostgreSQL",
                    CreeLe = DateTimeOffset.UtcNow
                };
                context.Projets.Add(project);
                await context.SaveChangesAsync();
                projectId = project.Id;

                var field = new ChampFiche
                {
                    ProjetId = projectId,
                    Libelle = "Client",
                    Valeur = "Example"
                };
                context.ChampsFiche.Add(field);
                await context.SaveChangesAsync();
                fieldId = field.Id;

                await context.MarquerProjetPourSuppressionAsync(projectId);
                await context.SaveChangesAsync();
            }

            await using var verification = CreateContext(connectionString);
            Assert.Null(await verification.ChampsFiche.IgnoreQueryFilters()
                .SingleOrDefaultAsync(field => field.Id == fieldId));
            Assert.Null(await verification.Projets.IgnoreQueryFilters()
                .SingleOrDefaultAsync(project => project.Id == projectId));
        });
    }

    private static Utilisateur CreateUser(string name) => new()
    {
        IdentifiantExterne = $"postgresql-{Guid.NewGuid():N}",
        Nom = name,
        Prenom = "Integration",
        Email = $"{Guid.NewGuid():N}@example.invalid",
        CreeLe = DateTimeOffset.UtcNow
    };

    private sealed class FixedCurrentUserService(string? userId) : ICurrentUserService
    {
        public string? UserId => userId;
    }
}
