using Microsoft.EntityFrameworkCore;
using PilotageForfaits.Application.Suivi;
using PilotageForfaits.Domain.Entites;
using PilotageForfaits.Infrastructure.Donnees;

namespace PilotageForfaits.Infrastructure.Persistence;

/// <summary>EF Core implementation for invoice schedules and their phase/task scope.</summary>
internal sealed class FacturationPersistence(PilotageDbContext db) : IFacturationPersistence
{
    public async Task<IReadOnlyList<Phase>> ChargerPhasesAsync(
        Guid projetId,
        CancellationToken annulation = default) =>
        await db.Phases
            .Where(phase => phase.ProjetId == projetId)
            .ToListAsync(annulation);

    public async Task<IReadOnlyList<LienEcheancePhase>> ChargerLiensAsync(
        Guid projetId,
        CancellationToken annulation = default) =>
        await db.LiensEcheancePhase
            .Where(lien => lien.ProjetId == projetId)
            .ToListAsync(annulation);

    public async Task<IReadOnlyList<Tache>> ChargerTachesAsync(
        Guid projetId,
        CancellationToken annulation = default) =>
        await db.Taches
            .Where(tache => tache.ProjetId == projetId && !tache.EstSection)
            .ToListAsync(annulation);

    public Task<Echeance?> ChargerEcheanceAsync(
        Guid projetId,
        int numero,
        CancellationToken annulation = default) =>
        db.Echeances.FirstOrDefaultAsync(
            echeance => echeance.ProjetId == projetId && echeance.Numero == numero,
            annulation);

    public void AjouterPhase(Phase phase)
    {
        ArgumentNullException.ThrowIfNull(phase);
        db.Phases.Add(phase);
    }

    public void AjouterLien(LienEcheancePhase lien)
    {
        ArgumentNullException.ThrowIfNull(lien);
        db.LiensEcheancePhase.Add(lien);
    }

    public void SupprimerLien(LienEcheancePhase lien)
    {
        ArgumentNullException.ThrowIfNull(lien);
        db.LiensEcheancePhase.Remove(lien);
    }
}
