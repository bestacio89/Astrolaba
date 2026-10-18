using PilotageForfaits.Domain.Entites;

namespace PilotageForfaits.Application.Suivi;

/// <summary>Persistence operations for invoice schedules and their phase/task scope.</summary>
public interface IFacturationPersistence
{
    Task<IReadOnlyList<Phase>> ChargerPhasesAsync(Guid projetId, CancellationToken annulation = default);
    Task<IReadOnlyList<LienEcheancePhase>> ChargerLiensAsync(Guid projetId, CancellationToken annulation = default);
    Task<IReadOnlyList<Tache>> ChargerTachesAsync(Guid projetId, CancellationToken annulation = default);
    Task<Echeance?> ChargerEcheanceAsync(Guid projetId, int numero, CancellationToken annulation = default);
    void AjouterPhase(Phase phase);
    void AjouterLien(LienEcheancePhase lien);
    void SupprimerLien(LienEcheancePhase lien);
}
