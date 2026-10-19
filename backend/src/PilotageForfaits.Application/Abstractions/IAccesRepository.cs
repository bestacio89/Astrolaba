using PilotageForfaits.Domain.Commun;
using PilotageForfaits.Domain.Entites;

namespace PilotageForfaits.Application.Abstractions;

/// <summary>
/// Persistence operations for authorization and the current user's application profile.
/// EF query composition stays in Infrastructure; returned users are tracked for profile synchronization.
/// </summary>
public interface IAccesRepository
{
    Task<Utilisateur?> TrouverUtilisateurAsync(string identifiantExterne, CancellationToken annulation = default);

    void AjouterUtilisateur(Utilisateur utilisateur);

    Task<bool> EstAffecteAuProjetAsync(Guid projetId, Guid utilisateurId, CancellationToken annulation = default);

    Task<bool> ProjetExisteAsync(Guid projetId, CancellationToken annulation = default);

    Task<bool> ProjetArchiveAsync(Guid projetId, CancellationToken annulation = default);
}
