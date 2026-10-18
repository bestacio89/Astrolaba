using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PilotageForfaits.Infrastructure.Donnees;

namespace PilotageForfaits.Infrastructure;

/// <summary>
/// Owns development-database startup checks and schema initialization.
/// This is intentionally invoked only by the API in the Development environment.
/// </summary>
public interface IInitialiseurBaseDeDeveloppement
{
    Task InitialiserAsync(CancellationToken annulation = default);
}

internal sealed class InitialiseurBaseDeDeveloppement(
    PilotageDbContext db,
    ILogger<InitialiseurBaseDeDeveloppement> logger) : IInitialiseurBaseDeDeveloppement
{
    public async Task InitialiserAsync(CancellationToken annulation = default)
    {
        if (db.Database.GetMigrations().Any())
        {
            if (db.Database.HasPendingModelChanges())
            {
                logger.LogCritical(
                    "LE MODÈLE DE DONNÉES A CHANGÉ DEPUIS LA DERNIÈRE MIGRATION : la base n'est pas mise à jour. Arrêtez l'API (Ctrl+C), puis depuis le dossier du dépôt : " +
                    ".\\scripts\\creer-migration.ps1 -Nom <NomDuChangement> (Windows) ou ./scripts/creer-migration.sh <NomDuChangement>, et relancez l'API " +
                    "(README, « Faire évoluer la base »).");
                return;
            }

            await db.Database.MigrateAsync(annulation);
            return;
        }

        logger.LogWarning(
            "Aucune migration trouvée : la base de développement est créée sans migration. Créez la migration initiale avant toute recette.");

        await db.Database.EnsureCreatedAsync(annulation);

        var manquants = await db.ElementsManquantsAsync(annulation);
        if (manquants.Count > 0)
        {
            logger.LogCritical(
                "LA BASE DE DÉVELOPPEMENT DATE D'UNE VERSION PRÉCÉDENTE : il lui manque {Manquants}. Arrêtez l'API (Ctrl+C), puis depuis le dossier du dépôt : " +
                ".\\scripts\\reinitialiser-base.ps1 (Windows) ou ./scripts/reinitialiser-base.sh. La base est supprimée puis recréée, gérée par migrations " +
                "(README, « Repartir propre et passer aux migrations »).",
                string.Join(", ", manquants));
        }
    }
}
