using Microsoft.EntityFrameworkCore;
using PilotageForfaits.Application.Suivi;
using PilotageForfaits.Domain.Entites;
using PilotageForfaits.Infrastructure.Donnees;

namespace PilotageForfaits.Infrastructure.Persistence;

/// <summary>EF Core implementation for project action, decision, and risk registers.</summary>
internal sealed class RegistresPersistence(PilotageDbContext db) : IRegistresPersistence
{
    public async Task<IReadOnlyList<ActionProjet>> ListerActionsAsync(Guid projetId, CancellationToken annulation = default) =>
        await db.Actions.Where(action => action.ProjetId == projetId).ToListAsync(annulation);

    public Task<ActionProjet?> ChargerActionAsync(Guid projetId, Guid actionId, CancellationToken annulation = default) =>
        db.Actions.FirstOrDefaultAsync(action => action.ProjetId == projetId && action.Id == actionId, annulation);

    public async Task<IReadOnlyList<Risque>> ListerRisquesAsync(Guid projetId, CancellationToken annulation = default) =>
        await db.Risques.Where(risque => risque.ProjetId == projetId).ToListAsync(annulation);

    public Task<Risque?> ChargerRisqueAsync(Guid projetId, Guid risqueId, CancellationToken annulation = default) =>
        db.Risques.FirstOrDefaultAsync(risque => risque.ProjetId == projetId && risque.Id == risqueId, annulation);

    public async Task<IReadOnlyList<Risque>> ChargerRisquesParActionAsync(Guid actionId, CancellationToken annulation = default) =>
        await db.Risques.Where(risque => risque.ActionId == actionId).ToListAsync(annulation);

    public async Task<IReadOnlyList<Decision>> ListerDecisionsAsync(Guid projetId, CancellationToken annulation = default) =>
        await db.Decisions.Where(decision => decision.ProjetId == projetId).ToListAsync(annulation);

    public Task<Decision?> ChargerDecisionAsync(Guid projetId, Guid decisionId, CancellationToken annulation = default) =>
        db.Decisions.FirstOrDefaultAsync(decision => decision.ProjetId == projetId && decision.Id == decisionId, annulation);

    public async Task<IReadOnlyList<EvolutionPerimetre>> ChargerEvolutionsParDecisionAsync(Guid decisionId, CancellationToken annulation = default) =>
        await db.Evolutions.Where(evolution => evolution.DecisionId == decisionId).ToListAsync(annulation);

    public Task<bool> ActionExisteAsync(Guid projetId, Guid actionId, CancellationToken annulation = default) =>
        db.Actions.AnyAsync(action => action.ProjetId == projetId && action.Id == actionId, annulation);

    public void AjouterAction(ActionProjet action)
    {
        ArgumentNullException.ThrowIfNull(action);
        db.Actions.Add(action);
    }

    public void SupprimerAction(ActionProjet action)
    {
        ArgumentNullException.ThrowIfNull(action);
        db.Actions.Remove(action);
    }

    public void AjouterDecision(Decision decision)
    {
        ArgumentNullException.ThrowIfNull(decision);
        db.Decisions.Add(decision);
    }

    public void SupprimerDecision(Decision decision)
    {
        ArgumentNullException.ThrowIfNull(decision);
        db.Decisions.Remove(decision);
    }

    public void AjouterRisque(Risque risque)
    {
        ArgumentNullException.ThrowIfNull(risque);
        db.Risques.Add(risque);
    }

    public void SupprimerRisque(Risque risque)
    {
        ArgumentNullException.ThrowIfNull(risque);
        db.Risques.Remove(risque);
    }
}
