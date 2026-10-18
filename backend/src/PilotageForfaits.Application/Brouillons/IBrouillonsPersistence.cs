using PilotageForfaits.Domain.Entites;

namespace PilotageForfaits.Application.Brouillons;

/// <summary>
/// Persistence port for draft workflows. Methods make tracking intent explicit so command
/// handlers can preserve optimistic concurrency and commit through the shared unit of work.
/// </summary>
public interface IBrouillonsPersistence
{
    Task<IReadOnlyList<BrouillonProjet>> ListerAvecPersonnesAsync(Guid utilisateurId, CancellationToken annulation = default);

    Task<BrouillonProjet?> ObtenirAvecPersonnesAsync(Guid id, CancellationToken annulation = default);

    Task<BrouillonProjet?> ObtenirPourModificationAsync(Guid id, CancellationToken annulation = default);

    Task<Utilisateur> ObtenirUtilisateurAsync(Guid id, CancellationToken annulation = default);

    Task<Utilisateur?> TrouverUtilisateurActifParEmailAsync(string emailNormalise, CancellationToken annulation = default);

    Task<IReadOnlyList<BrouillonProjet>> ListerAutresAvecAuteurAsync(Guid? saufId, CancellationToken annulation = default);

    Task<Dictionary<Guid, string>> NomsUtilisateursAsync(IReadOnlyCollection<Guid> ids, CancellationToken annulation = default);

    void Ajouter(BrouillonProjet brouillon);

    void Supprimer(BrouillonProjet brouillon);
}
