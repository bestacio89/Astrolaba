using PilotageForfaits.Domain.Entites;

namespace PilotageForfaits.Application.Suivi;

/// <summary>Persistence operations for frozen project planning references.</summary>
public interface IReferencesPersistence
{
    Task<int> CompterAsync(Guid projetId, CancellationToken annulation = default);
    Task<IReadOnlyList<Tache>> ChargerTachesAsync(Guid projetId, CancellationToken annulation = default);
    Task<LignePlanReference?> ChargerLigneAsync(
        Guid projetId,
        Guid referenceId,
        Guid tacheId,
        CancellationToken annulation = default);
    void AjouterPlan(PlanReference plan);
}
