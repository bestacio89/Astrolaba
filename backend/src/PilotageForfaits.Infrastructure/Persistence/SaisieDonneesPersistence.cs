using Microsoft.EntityFrameworkCore;
using PilotageForfaits.Application.Suivi;
using PilotageForfaits.Domain.Entites;
using PilotageForfaits.Infrastructure.Donnees;

namespace PilotageForfaits.Infrastructure.Persistence;

/// <summary>
/// EF Core implementation of tracking-write persistence. Application receives domain entities and
/// use-case-specific operations, never DbSet or IQueryable.
/// </summary>
internal sealed class SaisieDonneesPersistence(PilotageDbContext db) : ISaisieDonneesPersistence
{
    public Task<int> EnregistrerAsync(CancellationToken annulation) => db.SaveChangesAsync(annulation);

    public Task<Tache?> ObtenirTacheAsync(Guid projetId, string repere, CancellationToken annulation) =>
        db.Taches.Include(t => t.Evolutions)
            .FirstOrDefaultAsync(t => t.ProjetId == projetId && t.Repere == repere && !t.EstSection, annulation);

    public Task<ProfilProjet?> ObtenirProfilAsync(Guid projetId, Guid profilId, CancellationToken annulation) =>
        db.Profils.FirstOrDefaultAsync(p => p.ProjetId == projetId && p.Id == profilId, annulation);

    public Task<Livrable?> ObtenirLivrableTacheAsync(Guid tacheId, CancellationToken annulation) =>
        db.Livrables.FirstOrDefaultAsync(l => l.TacheId == tacheId, annulation);

    public void SupprimerLivrable(Livrable livrable) => db.Livrables.Remove(livrable);

    public Task<TypeTache?> ObtenirTypeTacheAsync(Guid projetId, string code, CancellationToken annulation) =>
        db.TypesTache.FirstOrDefaultAsync(x => x.ProjetId == projetId && x.Code == code, annulation);

    public Task<Echeance?> ObtenirEcheanceAsync(Guid projetId, int numero, CancellationToken annulation) =>
        db.Echeances.FirstOrDefaultAsync(x => x.ProjetId == projetId && x.Numero == numero, annulation);

    public Task<List<Consomme>> ListerConsommesJourAsync(Guid projetId, Guid profilId, DateOnly jour, CancellationToken annulation) =>
        db.Consommes.Where(x => x.ProjetId == projetId && x.ProfilProjetId == profilId && x.Jour == jour).ToListAsync(annulation);

    public Task<List<decimal>> ListerJoursConsommesTacheAsync(Guid tacheId, CancellationToken annulation) =>
        db.Consommes.Where(x => x.TacheId == tacheId).Select(x => x.JoursHomme).ToListAsync(annulation);

    public void SupprimerConsomme(Consomme consomme) => db.Consommes.Remove(consomme);
    public void AjouterConsomme(Consomme consomme) => db.Consommes.Add(consomme);

    public Task<Projet> ObtenirProjetAsync(Guid projetId, CancellationToken annulation) =>
        db.Projets.AsNoTracking().FirstAsync(p => p.Id == projetId, annulation);

    public Task<Axe?> ObtenirAxeAsync(Guid projetId, string code, CancellationToken annulation) =>
        db.Axes.FirstOrDefaultAsync(x => x.ProjetId == projetId && x.Code == code, annulation);

    public async Task<int> OrdreAxeMaxAsync(Guid projetId, CancellationToken annulation) =>
        await db.Axes.Where(x => x.ProjetId == projetId).Select(x => (int?)x.Ordre).MaxAsync(annulation) ?? -1;

    public void AjouterAxe(Axe axe) => db.Axes.Add(axe);

    public Task<Phase?> ObtenirPhaseAsync(Guid projetId, string code, CancellationToken annulation) =>
        db.Phases.FirstOrDefaultAsync(x => x.ProjetId == projetId && x.Code == code, annulation);

    public async Task<int> OrdrePhaseMaxAsync(Guid projetId, CancellationToken annulation) =>
        await db.Phases.Where(x => x.ProjetId == projetId).Select(x => (int?)x.Ordre).MaxAsync(annulation) ?? -1;

    public void AjouterPhase(Phase phase) => db.Phases.Add(phase);

    public Task<List<string>> ListerReferencesDecisionsAsync(Guid projetId, CancellationToken annulation) =>
        db.Decisions.Where(x => x.ProjetId == projetId).Select(x => x.Reference).ToListAsync(annulation);

    public void AjouterDecision(Decision decision) => db.Decisions.Add(decision);
    public void AjouterEvolution(EvolutionPerimetre evolution) => db.Evolutions.Add(evolution);

    public Task<List<ProfilProjet>> ListerProfilsProjetAsync(Guid projetId, CancellationToken annulation) =>
        db.Profils.Where(p => p.ProjetId == projetId).OrderBy(p => p.Rang).ToListAsync(annulation);

    public void AjouterProfil(ProfilProjet profil) => db.Profils.Add(profil);

    public Task<List<Tache>> ListerTachesProjetAsync(Guid projetId, CancellationToken annulation) =>
        db.Taches.Where(x => x.ProjetId == projetId).ToListAsync(annulation);

    public Task<Guid?> ObtenirAxeIdAsync(Guid projetId, string code, CancellationToken annulation) =>
        db.Axes.Where(x => x.ProjetId == projetId && x.Code == code).Select(x => (Guid?)x.Id).FirstOrDefaultAsync(annulation);

    public Task<Guid?> ObtenirPhaseIdAsync(Guid projetId, string code, CancellationToken annulation) =>
        db.Phases.Where(x => x.ProjetId == projetId && x.Code == code).Select(x => (Guid?)x.Id).FirstOrDefaultAsync(annulation);

    public void AjouterTache(Tache tache) => db.Taches.Add(tache);
    public void AjouterChargeTache(ChargeTache charge) => db.ChargesTache.Add(charge);
    public void AjouterAtelier(Atelier atelier) => db.Ateliers.Add(atelier);

    public Task<List<Consomme>> ListerConsommesTacheAsync(Guid tacheId, CancellationToken annulation) =>
        db.Consommes.Where(x => x.TacheId == tacheId).ToListAsync(annulation);

    public void SupprimerConsommes(IEnumerable<Consomme> consommes) => db.Consommes.RemoveRange(consommes);

    public Task<List<ChargeTache>> ListerChargesTacheAsync(Guid tacheId, CancellationToken annulation) =>
        db.ChargesTache.Where(x => x.TacheId == tacheId).ToListAsync(annulation);

    public void SupprimerChargesTache(IEnumerable<ChargeTache> charges) => db.ChargesTache.RemoveRange(charges);

    public Task<Atelier?> ObtenirAtelierAvecDestinatairesAsync(Guid tacheId, CancellationToken annulation) =>
        db.Ateliers.Include(x => x.Destinataires).FirstOrDefaultAsync(x => x.TacheId == tacheId, annulation);

    public void SupprimerDestinataires(IEnumerable<Destinataire> destinataires) => db.Destinataires.RemoveRange(destinataires);
    public void SupprimerAtelier(Atelier atelier) => db.Ateliers.Remove(atelier);

    public Task<List<Livrable>> ListerLivrablesTacheAsync(Guid tacheId, CancellationToken annulation) =>
        db.Livrables.Where(x => x.TacheId == tacheId).ToListAsync(annulation);

    public void SupprimerLivrables(IEnumerable<Livrable> livrables) => db.Livrables.RemoveRange(livrables);

    public Task<List<LignePlanReference>> ListerLignesPlanReferenceTacheAsync(Guid tacheId, CancellationToken annulation) =>
        db.LignesPlanReference.Where(x => x.TacheId == tacheId).ToListAsync(annulation);

    public void SupprimerLignesPlanReference(IEnumerable<LignePlanReference> lignes) => db.LignesPlanReference.RemoveRange(lignes);

    public Task<List<MiseAJourTache>> ListerMisesAJourTacheAsync(Guid tacheId, CancellationToken annulation) =>
        db.MisesAJour.Where(x => x.TacheId == tacheId).ToListAsync(annulation);

    public void SupprimerMisesAJour(IEnumerable<MiseAJourTache> misesAJour) => db.MisesAJour.RemoveRange(misesAJour);
    public void AjouterMiseAJour(MiseAJourTache miseAJour) => db.MisesAJour.Add(miseAJour);

    public Task<List<ActionProjet>> ListerActionsTacheAsync(Guid tacheId, CancellationToken annulation) =>
        db.Actions.Where(x => x.TacheId == tacheId).ToListAsync(annulation);

    public void SupprimerTache(Tache tache) => db.Taches.Remove(tache);

    public Task<ChargeTache?> ObtenirChargeTacheAsync(Guid tacheId, Guid profilId, CancellationToken annulation) =>
        db.ChargesTache.FirstOrDefaultAsync(x => x.TacheId == tacheId && x.ProfilProjetId == profilId, annulation);

    public void SupprimerChargeTache(ChargeTache charge) => db.ChargesTache.Remove(charge);

    public Task<ProfilProjet?> ObtenirProfilParIdAsync(Guid profilId, CancellationToken annulation) =>
        db.Profils.FirstOrDefaultAsync(p => p.Id == profilId, annulation);

    public void SupprimerEvolution(EvolutionPerimetre evolution) => db.Evolutions.Remove(evolution);
    public void SupprimerEvolutions(IEnumerable<EvolutionPerimetre> evolutions) => db.Evolutions.RemoveRange(evolutions);

    public Task<Dictionary<Guid, string>> ListerCodesAxesAsync(Guid projetId, CancellationToken annulation) =>
        db.Axes.AsNoTracking().Where(x => x.ProjetId == projetId).ToDictionaryAsync(x => x.Id, x => x.Code, annulation);

    public Task<Dictionary<Guid, string>> ListerCodesPhasesAsync(Guid projetId, CancellationToken annulation) =>
        db.Phases.AsNoTracking().Where(x => x.ProjetId == projetId).ToDictionaryAsync(x => x.Id, x => x.Code, annulation);
}
