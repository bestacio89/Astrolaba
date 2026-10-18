using Microsoft.EntityFrameworkCore;
using PilotageForfaits.Application.Calcul;
using PilotageForfaits.Domain.Entites;
using PilotageForfaits.Infrastructure.Donnees;

namespace PilotageForfaits.Infrastructure.Persistence;

/// <summary>
/// Loads the project calculation read model with EF Core. Queries remain no-tracking and
/// respect Franz's soft-delete filters; calculation and business transformation stay in Application.
/// </summary>
internal sealed class ChargeurDonneesProjet(PilotageDbContext db) : IChargeurDonneesProjet
{
    public async Task<ConstructeurProjetCalcul.DonneesProjet> ChargerDonneesAsync(
        Guid projetId,
        CancellationToken annulation = default)
    {
        var projet = await db.Projets.AsNoTracking().FirstAsync(p => p.Id == projetId, annulation);
        // One query per table keeps query plans bounded and avoids a large join fan-out.
        var profils = await db.Profils.AsNoTracking().Where(x => x.ProjetId == projetId).OrderBy(x => x.Rang).ToListAsync(annulation);
        var taches = await db.Taches.AsNoTracking().Where(x => x.ProjetId == projetId).OrderBy(x => x.Position).ToListAsync(annulation);
        var charges = await db.ChargesTache.AsNoTracking().Where(x => x.ProjetId == projetId).ToListAsync(annulation);
        var consommes = await db.Consommes.AsNoTracking().Where(x => x.ProjetId == projetId).ToListAsync(annulation);
        var evolutions = await db.Evolutions.AsNoTracking().Where(x => x.ProjetId == projetId).ToListAsync(annulation);
        var axes = await db.Axes.AsNoTracking().Where(x => x.ProjetId == projetId).OrderBy(x => x.Ordre).ToListAsync(annulation);
        var phases = await db.Phases.AsNoTracking().Where(x => x.ProjetId == projetId).OrderBy(x => x.Ordre).ToListAsync(annulation);
        var etapes = await db.Etapes.AsNoTracking().Where(x => x.ProjetId == projetId).OrderBy(x => x.Ordre).ToListAsync(annulation);
        var types = await db.TypesTache.AsNoTracking().Where(x => x.ProjetId == projetId).OrderBy(x => x.Ordre).ToListAsync(annulation);
        var valeursIa = await db.ValeursIa.AsNoTracking().Where(x => x.ProjetId == projetId).OrderBy(x => x.Ordre).ToListAsync(annulation);
        var echeances = await db.Echeances.AsNoTracking().Where(x => x.ProjetId == projetId).OrderBy(x => x.Numero).ToListAsync(annulation);
        var liensPhases = await db.LiensEcheancePhase.AsNoTracking().Where(x => x.ProjetId == projetId).ToListAsync(annulation);
        var planning = await db.PlanningsProfils.AsNoTracking().Where(x => x.ProjetId == projetId).ToListAsync(annulation);
        var planningPhases = await db.PlanningsPhases.AsNoTracking().Where(x => x.ProjetId == projetId).OrderBy(x => x.Ordre).ToListAsync(annulation);
        var planningPhasesMois = await db.PlanningsPhasesMois.AsNoTracking().Where(x => x.ProjetId == projetId).ToListAsync(annulation);
        var budgetAchats = await db.BudgetsAchats.AsNoTracking().Where(x => x.ProjetId == projetId).OrderBy(x => x.Ordre).ToListAsync(annulation);
        var budgetFrais = await db.BudgetsFrais.AsNoTracking().Where(x => x.ProjetId == projetId).OrderBy(x => x.Ordre).ToListAsync(annulation);
        var prestataires = await db.Prestataires.AsNoTracking().Where(x => x.ProjetId == projetId).OrderBy(x => x.Ordre).ToListAsync(annulation);
        var metriques = await db.Metriques.AsNoTracking().Where(x => x.ProjetId == projetId).OrderBy(x => x.Ordre).ToListAsync(annulation);
        var achatsReels = await db.AchatsReels.AsNoTracking().Where(x => x.ProjetId == projetId).ToListAsync(annulation);
        var fraisReels = await db.FraisReels.AsNoTracking().Where(x => x.ProjetId == projetId).ToListAsync(annulation);
        var actions = await db.Actions.AsNoTracking().Where(x => x.ProjetId == projetId).ToListAsync(annulation);
        var decisions = await db.Decisions.AsNoTracking().Where(x => x.ProjetId == projetId).ToListAsync(annulation);
        var risques = await db.Risques.AsNoTracking().Where(x => x.ProjetId == projetId).ToListAsync(annulation);
        var ateliers = await db.Ateliers.AsNoTracking().Where(x => x.ProjetId == projetId).ToListAsync(annulation);
        var destinataires = await db.Destinataires.AsNoTracking().Where(x => x.ProjetId == projetId).OrderBy(x => x.Ordre).ToListAsync(annulation);
        var livrables = await db.Livrables.AsNoTracking().Where(x => x.ProjetId == projetId).ToListAsync(annulation);
        var photos = await db.PhotosIndicateurs.AsNoTracking().Where(x => x.ProjetId == projetId).ToListAsync(annulation);
        var imports = await db.ImportsTsp.AsNoTracking().Where(x => x.ProjetId == projetId).ToListAsync(annulation);
        var champs = await db.ChampsFiche.AsNoTracking().Where(x => x.ProjetId == projetId).OrderBy(x => x.Ordre).ToListAsync(annulation);
        var grille = await db.Grilles.AsNoTracking().Where(x => x.ProjetId == projetId).OrderBy(x => x.Ordre).ToListAsync(annulation);
        var idsAuteurs = evolutions.Select(e => e.ParId).Concat(charges.Select(c => c.AjouteeParId)).Concat(profils.Select(p => p.AjouteParId))
            .Concat(actions.Select(a => a.CreeeParId)).Concat(decisions.Select(x => x.CreeeParId)).Concat(risques.Select(r => r.CreeParId))
            .Concat(achatsReels.Select(a => a.CreeParId)).Concat(fraisReels.Select(f => f.CreeParId))
            .Where(x => x is not null).Select(x => x!.Value).Distinct().ToList();
        var auteurs = await db.Utilisateurs.AsNoTracking().Where(u => idsAuteurs.Contains(u.Id)).ToDictionaryAsync(u => u.Id, annulation);

        // Dates are sorted in memory where provider translation is inconsistent (notably SQLite).
        return new ConstructeurProjetCalcul.DonneesProjet(
            projet, profils, taches, charges, consommes, evolutions, axes, phases, etapes, types, valeursIa,
            echeances, liensPhases, planning, planningPhases, planningPhasesMois, budgetAchats, budgetFrais, prestataires, metriques,
            achatsReels, fraisReels, actions, decisions, risques, auteurs,
            ateliers, destinataires, livrables, photos.OrderBy(x => x.Date).ThenBy(x => x.Nature).ToList(),
            imports.OrderBy(x => x.ImporteLe).ToList(), champs, grille);
    }
}
