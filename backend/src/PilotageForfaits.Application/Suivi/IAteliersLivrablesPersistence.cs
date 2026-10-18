using PilotageForfaits.Domain.Entites;

namespace PilotageForfaits.Application.Suivi;

/// <summary>
/// Persistence operations for workshop scheduling and deliverable tracking.
/// </summary>
public interface IAteliersLivrablesPersistence
{
    Task<Atelier?> ChargerAtelierAsync(Guid tacheId, CancellationToken annulation = default);
    Task<Livrable?> ChargerLivrableAsync(Guid tacheId, CancellationToken annulation = default);
    void AjouterAtelier(Atelier atelier);
    void SupprimerAtelier(Atelier atelier);
    void AjouterLivrable(Livrable livrable);
    void SupprimerLivrable(Livrable livrable);
    void RemplacerDestinataires(Atelier atelier, IReadOnlyList<Destinataire> destinataires);
}
