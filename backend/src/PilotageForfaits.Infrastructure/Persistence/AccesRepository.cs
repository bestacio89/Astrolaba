using Microsoft.EntityFrameworkCore;
using PilotageForfaits.Application.Abstractions;
using PilotageForfaits.Domain.Commun;
using PilotageForfaits.Domain.Entites;
using PilotageForfaits.Infrastructure.Donnees;

namespace PilotageForfaits.Infrastructure.Persistence;

/// <summary>
/// EF implementation of the authorization persistence port. User lookups are tracked so the
/// authenticated profile can be synchronized; authorization checks are read-only.
/// </summary>
internal sealed class AccesRepository(PilotageDbContext db) : IAccesRepository
{
    public Task<Utilisateur?> TrouverUtilisateurAsync(
        string identifiantExterne,
        CancellationToken annulation = default) =>
        db.Utilisateurs.FirstOrDefaultAsync(
            utilisateur => utilisateur.IdentifiantExterne == identifiantExterne,
            annulation);

    public void AjouterUtilisateur(Utilisateur utilisateur)
    {
        ArgumentNullException.ThrowIfNull(utilisateur);
        db.Utilisateurs.Add(utilisateur);
    }

    public async Task<RoleProjet?> TrouverRoleAsync(
        Guid projetId,
        Guid utilisateurId,
        CancellationToken annulation = default)
    {
        var habilitation = await db.Habilitations
            .AsNoTracking()
            .FirstOrDefaultAsync(
                droit => droit.ProjetId == projetId && droit.UtilisateurId == utilisateurId,
                annulation);

        return habilitation?.Role;
    }

    public Task<bool> ProjetExisteAsync(Guid projetId, CancellationToken annulation = default) =>
        db.Projets.AnyAsync(projet => projet.Id == projetId, annulation);

    public Task<bool> ProjetArchiveAsync(Guid projetId, CancellationToken annulation = default) =>
        db.Projets.AnyAsync(
            projet => projet.Id == projetId && projet.Statut == StatutProjet.Archive,
            annulation);
}
