using PilotageForfaits.Domain.Entites;

namespace PilotageForfaits.Application.Suivi;

/// <summary>
/// Persistence operations for tracking writes. Query composition and entity state management
/// remain in Infrastructure; this contract deliberately exposes no EF Core types.
/// </summary>
public interface ISaisieDonneesPersistence
{
    Task<int> EnregistrerAsync(CancellationToken annulation);
    Task<Tache?> ObtenirTacheAsync(Guid projetId, string repere, CancellationToken annulation);
    Task<ProfilProjet?> ObtenirProfilAsync(Guid projetId, Guid profilId, CancellationToken annulation);
    Task<Livrable?> ObtenirLivrableTacheAsync(Guid tacheId, CancellationToken annulation);
    void SupprimerLivrable(Livrable livrable);
    Task<TypeTache?> ObtenirTypeTacheAsync(Guid projetId, string code, CancellationToken annulation);
    Task<Echeance?> ObtenirEcheanceAsync(Guid projetId, int numero, CancellationToken annulation);
    Task<List<Consomme>> ListerConsommesJourAsync(Guid projetId, Guid profilId, DateOnly jour, CancellationToken annulation);
    Task<List<decimal>> ListerJoursConsommesTacheAsync(Guid tacheId, CancellationToken annulation);
    void SupprimerConsomme(Consomme consomme);
    void AjouterConsomme(Consomme consomme);
    Task<Projet> ObtenirProjetAsync(Guid projetId, CancellationToken annulation);
    Task<Axe?> ObtenirAxeAsync(Guid projetId, string code, CancellationToken annulation);
    Task<int> OrdreAxeMaxAsync(Guid projetId, CancellationToken annulation);
    void AjouterAxe(Axe axe);
    Task<Phase?> ObtenirPhaseAsync(Guid projetId, string code, CancellationToken annulation);
    Task<int> OrdrePhaseMaxAsync(Guid projetId, CancellationToken annulation);
    void AjouterPhase(Phase phase);

    Task<List<string>> ListerReferencesDecisionsAsync(Guid projetId, CancellationToken annulation);
    void AjouterDecision(Decision decision);
    void AjouterEvolution(EvolutionPerimetre evolution);
    Task<List<ProfilProjet>> ListerProfilsProjetAsync(Guid projetId, CancellationToken annulation);
    void AjouterProfil(ProfilProjet profil);
    Task<List<Tache>> ListerTachesProjetAsync(Guid projetId, CancellationToken annulation);
    Task<Guid?> ObtenirAxeIdAsync(Guid projetId, string code, CancellationToken annulation);
    Task<Guid?> ObtenirPhaseIdAsync(Guid projetId, string code, CancellationToken annulation);
    void AjouterTache(Tache tache);
    void AjouterChargeTache(ChargeTache charge);
    void AjouterAtelier(Atelier atelier);
    Task<List<Consomme>> ListerConsommesTacheAsync(Guid tacheId, CancellationToken annulation);
    void SupprimerConsommes(IEnumerable<Consomme> consommes);
    Task<List<ChargeTache>> ListerChargesTacheAsync(Guid tacheId, CancellationToken annulation);
    void SupprimerChargesTache(IEnumerable<ChargeTache> charges);
    Task<Atelier?> ObtenirAtelierAvecDestinatairesAsync(Guid tacheId, CancellationToken annulation);
    void SupprimerDestinataires(IEnumerable<Destinataire> destinataires);
    void SupprimerAtelier(Atelier atelier);
    Task<List<Livrable>> ListerLivrablesTacheAsync(Guid tacheId, CancellationToken annulation);
    void SupprimerLivrables(IEnumerable<Livrable> livrables);
    Task<List<LignePlanReference>> ListerLignesPlanReferenceTacheAsync(Guid tacheId, CancellationToken annulation);
    void SupprimerLignesPlanReference(IEnumerable<LignePlanReference> lignes);
    Task<List<MiseAJourTache>> ListerMisesAJourTacheAsync(Guid tacheId, CancellationToken annulation);
    void SupprimerMisesAJour(IEnumerable<MiseAJourTache> misesAJour);
    Task<List<ActionProjet>> ListerActionsTacheAsync(Guid tacheId, CancellationToken annulation);
    void SupprimerTache(Tache tache);
    Task<ChargeTache?> ObtenirChargeTacheAsync(Guid tacheId, Guid profilId, CancellationToken annulation);
    void SupprimerChargeTache(ChargeTache charge);
    Task<ProfilProjet?> ObtenirProfilParIdAsync(Guid profilId, CancellationToken annulation);
    void SupprimerEvolution(EvolutionPerimetre evolution);
    Task<Dictionary<Guid, string>> ListerCodesAxesAsync(Guid projetId, CancellationToken annulation);
    Task<Dictionary<Guid, string>> ListerCodesPhasesAsync(Guid projetId, CancellationToken annulation);
}
