using Microsoft.EntityFrameworkCore;
using PilotageForfaits.Application.Suivi;
using PilotageForfaits.Domain.Entites;
using PilotageForfaits.Infrastructure.Donnees;

namespace PilotageForfaits.Infrastructure.Persistence;

/// <summary>EF Core implementation for frozen project planning references.</summary>
internal sealed class ReferencesPersistence(PilotageDbContext db) : IReferencesPersistence
{
    public Task<int> CompterAsync(Guid projetId, CancellationToken annulation = default) =>
        db.PlansReference.CountAsync(plan => plan.ProjetId == projetId, annulation);

    public async Task<IReadOnlyList<Tache>> ChargerTachesAsync(
        Guid projetId,
        CancellationToken annulation = default) =>
        await db.Taches
            .AsNoTracking()
            .Where(tache => tache.ProjetId == projetId && !tache.EstSection)
            .ToListAsync(annulation);

    public Task<LignePlanReference?> ChargerLigneAsync(
        Guid projetId,
        Guid referenceId,
        Guid tacheId,
        CancellationToken annulation = default) =>
        db.LignesPlanReference.FirstOrDefaultAsync(
            ligne => ligne.ProjetId == projetId &&
                     ligne.PlanReferenceId == referenceId &&
                     ligne.TacheId == tacheId,
            annulation);

    public void AjouterPlan(PlanReference plan)
    {
        ArgumentNullException.ThrowIfNull(plan);
        db.PlansReference.Add(plan);
    }
}
