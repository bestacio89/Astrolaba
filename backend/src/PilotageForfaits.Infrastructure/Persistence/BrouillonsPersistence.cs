using Microsoft.EntityFrameworkCore;
using PilotageForfaits.Application.Brouillons;
using PilotageForfaits.Domain.Entites;
using PilotageForfaits.Infrastructure.Donnees;

namespace PilotageForfaits.Infrastructure.Persistence;

/// <summary>
/// EF implementation of draft persistence. Read-only screens use no-tracking queries;
/// draft edits remain tracked so Version concurrency checks and Franz auditing run at commit.
/// </summary>
internal sealed class BrouillonsPersistence(PilotageDbContext db) : IBrouillonsPersistence
{
    public async Task<IReadOnlyList<BrouillonProjet>> ListerAvecPersonnesAsync(
        Guid utilisateurId,
        CancellationToken annulation = default) =>
        await db.Brouillons.AsNoTracking()
            .Include(brouillon => brouillon.CreePar)
            .Include(brouillon => brouillon.CoRedacteur)
            .Where(brouillon => brouillon.CreeParId == utilisateurId || brouillon.CoRedacteurId == utilisateurId)
            .ToListAsync(annulation);

    public Task<BrouillonProjet?> ObtenirAvecPersonnesAsync(
        Guid id,
        CancellationToken annulation = default) =>
        db.Brouillons.AsNoTracking()
            .Include(brouillon => brouillon.CreePar)
            .Include(brouillon => brouillon.CoRedacteur)
            .FirstOrDefaultAsync(brouillon => brouillon.Id == id, annulation);

    public Task<BrouillonProjet?> ObtenirPourModificationAsync(
        Guid id,
        CancellationToken annulation = default) =>
        db.Brouillons.FirstOrDefaultAsync(brouillon => brouillon.Id == id, annulation);

    public Task<Utilisateur> ObtenirUtilisateurAsync(
        Guid id,
        CancellationToken annulation = default) =>
        db.Utilisateurs.AsNoTracking()
            .FirstAsync(utilisateur => utilisateur.Id == id, annulation);

    public Task<Utilisateur?> TrouverUtilisateurActifParEmailAsync(
        string emailNormalise,
        CancellationToken annulation = default) =>
        db.Utilisateurs.FirstOrDefaultAsync(
            utilisateur => utilisateur.Email.ToLower() == emailNormalise && utilisateur.Actif,
            annulation);

    public async Task<IReadOnlyList<BrouillonProjet>> ListerAutresAvecAuteurAsync(
        Guid? saufId,
        CancellationToken annulation = default) =>
        await db.Brouillons.AsNoTracking()
            .Include(brouillon => brouillon.CreePar)
            .Where(brouillon => saufId == null || brouillon.Id != saufId)
            .ToListAsync(annulation);

    public async Task<Dictionary<Guid, string>> NomsUtilisateursAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken annulation = default)
    {
        var liste = ids.ToList();
        if (liste.Count == 0) return new Dictionary<Guid, string>();

        return (await db.Utilisateurs.AsNoTracking()
                .Where(utilisateur => liste.Contains(utilisateur.Id))
                .ToListAsync(annulation))
            .ToDictionary(utilisateur => utilisateur.Id, utilisateur => utilisateur.NomComplet);
    }

    public void Ajouter(BrouillonProjet brouillon)
    {
        ArgumentNullException.ThrowIfNull(brouillon);
        db.Brouillons.Add(brouillon);
    }

    public void Supprimer(BrouillonProjet brouillon)
    {
        ArgumentNullException.ThrowIfNull(brouillon);
        db.Brouillons.Remove(brouillon);
    }
}
