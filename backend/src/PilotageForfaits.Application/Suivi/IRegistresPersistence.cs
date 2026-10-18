using PilotageForfaits.Domain.Entites;

namespace PilotageForfaits.Application.Suivi;

/// <summary>Persistence operations for project action, decision, and risk registers.</summary>
public interface IRegistresPersistence
{
    Task<IReadOnlyList<ActionProjet>> ListerActionsAsync(Guid projetId, CancellationToken annulation = default);
    Task<ActionProjet?> ChargerActionAsync(Guid projetId, Guid actionId, CancellationToken annulation = default);
    Task<IReadOnlyList<Risque>> ListerRisquesAsync(Guid projetId, CancellationToken annulation = default);
    Task<Risque?> ChargerRisqueAsync(Guid projetId, Guid risqueId, CancellationToken annulation = default);
    Task<IReadOnlyList<Risque>> ChargerRisquesParActionAsync(Guid actionId, CancellationToken annulation = default);
    Task<IReadOnlyList<Decision>> ListerDecisionsAsync(Guid projetId, CancellationToken annulation = default);
    Task<Decision?> ChargerDecisionAsync(Guid projetId, Guid decisionId, CancellationToken annulation = default);
    Task<IReadOnlyList<EvolutionPerimetre>> ChargerEvolutionsParDecisionAsync(Guid decisionId, CancellationToken annulation = default);
    Task<bool> ActionExisteAsync(Guid projetId, Guid actionId, CancellationToken annulation = default);
    void AjouterAction(ActionProjet action);
    void SupprimerAction(ActionProjet action);
    void AjouterDecision(Decision decision);
    void SupprimerDecision(Decision decision);
    void AjouterRisque(Risque risque);
    void SupprimerRisque(Risque risque);
}
