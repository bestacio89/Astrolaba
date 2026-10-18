using Microsoft.EntityFrameworkCore;
using PilotageForfaits.Application.Suivi;
using PilotageForfaits.Domain.Entites;
using PilotageForfaits.Infrastructure.Donnees;

namespace PilotageForfaits.Infrastructure.Persistence;

/// <summary>
/// EF Core implementation of the tracking-view read port.
/// The application layer receives materialized data and never composes provider queries.
/// </summary>
internal sealed class SuiviPersistence(PilotageDbContext db) : ISuiviPersistence
{
    public async Task<JournalSuiviPersistence> ChargerJournalAsync(
        Guid projetId,
        int limite,
        CancellationToken annulation = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(limite);

        var entrees = await db.Journal
            .AsNoTracking()
            .Where(entree => entree.ProjetId == projetId)
            .OrderByDescending(entree => entree.Le)
            .Take(limite)
            .ToListAsync(annulation);

        var utilisateurIds = entrees
            .Where(entree => entree.UtilisateurId.HasValue)
            .Select(entree => entree.UtilisateurId!.Value)
            .Distinct()
            .ToArray();

        var noms = await ChargerNomsAsync(utilisateurIds, annulation);
        return new JournalSuiviPersistence(entrees, noms);
    }

    public async Task<PointHebdoSuiviPersistence> ChargerPointsHebdoAsync(
        Guid projetId,
        CancellationToken annulation = default)
    {
        var points = await db.PointsHebdo
            .AsNoTracking()
            .Where(point => point.ProjetId == projetId)
            .ToListAsync(annulation);

        var utilisateurIds = points
            .Where(point => point.ParId.HasValue)
            .Select(point => point.ParId!.Value)
            .Distinct()
            .ToArray();

        var noms = await ChargerNomsAsync(utilisateurIds, annulation);
        return new PointHebdoSuiviPersistence(points, noms);
    }

    public async Task<ComplementsSuiviPersistence> ChargerComplementsAsync(
        Guid projetId,
        CancellationToken annulation = default)
    {
        var plans = await db.PlansReference
            .AsNoTracking()
            .Where(plan => plan.ProjetId == projetId)
            .ToListAsync(annulation);

        var lignes = await db.LignesPlanReference
            .AsNoTracking()
            .Where(ligne => ligne.ProjetId == projetId)
            .ToListAsync(annulation);

        var misesAJour = await db.MisesAJour
            .AsNoTracking()
            .Where(miseAJour => miseAJour.ProjetId == projetId)
            .ToListAsync(annulation);

        var utilisateurIds = plans
            .Select(plan => plan.FigeParId)
            .Concat(misesAJour.Select(miseAJour => miseAJour.AuteurId))
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToArray();

        var noms = await ChargerNomsAsync(utilisateurIds, annulation);
        return new ComplementsSuiviPersistence(plans, lignes, misesAJour, noms);
    }

    private async Task<IReadOnlyDictionary<Guid, string>> ChargerNomsAsync(
        IReadOnlyCollection<Guid> utilisateurIds,
        CancellationToken annulation)
    {
        if (utilisateurIds.Count == 0)
        {
            return new Dictionary<Guid, string>();
        }

        return await db.Utilisateurs
            .AsNoTracking()
            .Where(utilisateur => utilisateurIds.Contains(utilisateur.Id))
            .ToDictionaryAsync(utilisateur => utilisateur.Id, utilisateur => utilisateur.NomComplet, annulation);
    }
}
