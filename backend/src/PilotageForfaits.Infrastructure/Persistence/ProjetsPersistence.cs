using Microsoft.EntityFrameworkCore;
using PilotageForfaits.Application.Projets;
using PilotageForfaits.Domain.Commun;
using PilotageForfaits.Domain.Entites;
using PilotageForfaits.Infrastructure.Donnees;

namespace PilotageForfaits.Infrastructure.Persistence;

/// <summary>
/// EF implementation of project workflow queries. All reads use no-tracking queries where
/// appropriate and therefore respect Franz's global soft-delete filters.
/// </summary>
internal sealed class ProjetsPersistence(PilotageDbContext db) : IProjetsPersistence
{
    public async Task<ProjetListePersistence> ListerAsync(
        Guid utilisateurId,
        bool voitTousLesProjets,
        bool archives,
        CancellationToken annulation = default)
    {
        var roles = await db.Habilitations.AsNoTracking()
            .Where(habilitation => habilitation.UtilisateurId == utilisateurId)
            .ToDictionaryAsync(habilitation => habilitation.ProjetId, habilitation => habilitation.Role, annulation);

        var requete = archives
            ? db.Projets.AsNoTracking().Where(projet => projet.Statut == StatutProjet.Archive)
            : db.Projets.AsNoTracking().Where(projet => projet.Statut != StatutProjet.Archive);

        if (!voitTousLesProjets)
        {
            var ids = roles.Keys.ToList();
            requete = requete.Where(projet => ids.Contains(projet.Id));
        }

        var projets = await requete
            .OrderBy(projet => projet.Client)
            .ThenBy(projet => projet.Nom)
            .ToListAsync(annulation);

        var idsProjets = projets.Select(projet => projet.Id).ToList();
        var chefs = (await db.Habilitations.AsNoTracking()
                .Include(habilitation => habilitation.Utilisateur)
                .Where(habilitation =>
                    habilitation.Role == RoleProjet.ChefDeProjet &&
                    idsProjets.Contains(habilitation.ProjetId))
                .ToListAsync(annulation))
            .GroupBy(habilitation => habilitation.ProjetId)
            .ToDictionary(
                groupe => groupe.Key,
                groupe => string.Join(", ", groupe.Select(habilitation => habilitation.Utilisateur!.NomComplet)));

        return new ProjetListePersistence(projets, roles, chefs);
    }

    public void Ajouter(Projet projet)
    {
        ArgumentNullException.ThrowIfNull(projet);
        db.Projets.Add(projet);
    }

    public async Task<ProjetSuppressionPersistence> ChargerPourSuppressionAsync(
        Guid projetId,
        CancellationToken annulation = default)
    {
        var projet = await db.Projets.AsNoTracking()
            .FirstAsync(item => item.Id == projetId, annulation);

        var aConsomme = await db.Consommes.AnyAsync(item => item.ProjetId == projetId, annulation);
        var aAchatsReels = await db.AchatsReels.AnyAsync(item => item.ProjetId == projetId, annulation);
        var aFraisReels = await db.FraisReels.AnyAsync(item => item.ProjetId == projetId, annulation);
        var aFactureEmise = await db.Echeances.AnyAsync(
            item => item.ProjetId == projetId && item.FactureeLe != null,
            annulation);

        return new ProjetSuppressionPersistence(projet, aConsomme, aAchatsReels, aFraisReels, aFactureEmise);
    }

    public async Task SupprimerDefinitivementAsync(Guid projetId, CancellationToken annulation = default)
    {
        await db.MarquerProjetPourSuppressionAsync(projetId, annulation);
        await db.SaveChangesAsync(annulation);
    }

    public async Task<ProjetDetailPersistence> ChargerDetailAsync(
        Guid projetId,
        CancellationToken annulation = default)
    {
        var projet = await db.Projets.AsNoTracking()
            .FirstAsync(item => item.Id == projetId, annulation);

        var champs = await db.ChampsFiche.AsNoTracking()
            .Where(champ => champ.ProjetId == projetId)
            .OrderBy(champ => champ.Ordre)
            .ToListAsync(annulation);

        // Sorting DateTimeOffset in memory remains compatible with the SQLite test provider.
        var dernierImport = (await db.ImportsTsp.AsNoTracking()
                .Where(import => import.ProjetId == projetId)
                .ToListAsync(annulation))
            .OrderByDescending(import => import.ImporteLe)
            .FirstOrDefault();

        return new ProjetDetailPersistence(projet, champs, dernierImport);
    }
}
