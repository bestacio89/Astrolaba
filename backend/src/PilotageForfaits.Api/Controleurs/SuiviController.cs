using System.Text.Json.Nodes;
using Franz.Common.Mediator.Dispatchers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PilotageForfaits.Application.Suivi;
using PilotageForfaits.Contracts.Suivi;
using PilotageForfaits.Contracts.Exports;

namespace PilotageForfaits.Api.Controleurs;

/// <summary>
/// Écran de suivi d'un projet : lecture (saisies + indicateurs) et toutes les saisies de la version de référence (POC v65).
/// Chaque saisie renvoie le suivi recalculé et les messages à afficher.
/// </summary>
[ApiController]
[Authorize]
[Route("api/projets/{id:guid}")]
public sealed class SuiviController(IDispatcher dispatcher, SuiviService suivi) : ControllerBase
{
    /// <summary>Saisies, indicateurs calculés et droits de l'utilisateur sur le projet.</summary>
    [HttpGet("suivi")]
    public Task<JsonObject> Obtenir(Guid id, CancellationToken a) => suivi.ObtenirAsync(id, a);

    // ---------------------------------------------------------------- tâches

    [HttpPut("taches/{uid}/statut")]
    public Task<ResultatSaisieDto> Statut(Guid id, string uid, StatutCommande c, CancellationToken a) => dispatcher.SendAsync(new DefinirStatutTacheCommand(id, uid, c), a);

    [HttpPut("taches/{uid}/avancement")]
    public Task<ResultatSaisieDto> Avancement(Guid id, string uid, AvancementCommande c, CancellationToken a) => dispatcher.SendAsync(new DefinirAvancementTacheCommand(id, uid, c), a);

    [HttpPut("taches/{uid}/reste-a-faire")]
    public Task<ResultatSaisieDto> ResteAFaire(Guid id, string uid, ResteAFaireCommande c, CancellationToken a) => dispatcher.SendAsync(new DefinirResteAFaireTacheCommand(id, uid, c), a);

    [HttpPut("taches/{uid}/dates")]
    public Task<ResultatSaisieDto> Dates(Guid id, string uid, DatesTacheCommande c, CancellationToken a) => dispatcher.SendAsync(new DefinirDatesTacheCommand(id, uid, c), a);

    [HttpPut("taches/{uid}/libelle")]
    public Task<ResultatSaisieDto> Libelle(Guid id, string uid, LibelleCommande c, CancellationToken a) => dispatcher.SendAsync(new DefinirLibelleTacheCommand(id, uid, c), a);

    [HttpPut("taches/{uid}/commentaire")]
    public Task<ResultatSaisieDto> Commentaire(Guid id, string uid, CommentaireCommande c, CancellationToken a) => dispatcher.SendAsync(new DefinirCommentaireTacheCommand(id, uid, c), a);

    [HttpPut("taches/{uid}/classement")]
    public Task<ResultatSaisieDto> Classement(Guid id, string uid, ClassementCommande c, CancellationToken a) => dispatcher.SendAsync(new DefinirClassementTacheCommand(id, uid, c), a);

    [HttpPut("taches/{uid}/bilan-ia")]
    public Task<ResultatSaisieDto> BilanIa(Guid id, string uid, BilanIaCommande c, CancellationToken a) => dispatcher.SendAsync(new DefinirBilanIaTacheCommand(id, uid, c), a);

    [HttpPut("taches/{uid}/echeance")]
    public Task<ResultatSaisieDto> EcheanceTache(Guid id, string uid, EcheanceTacheCommande c, CancellationToken a) => dispatcher.SendAsync(new DefinirEcheanceTacheCommand(id, uid, c), a);

    /// <summary>Consommé d'un profil sur une tâche, un jour donné (chef de projet ou consultant).</summary>
    [HttpPut("taches/{uid}/consomme")]
    public Task<ResultatSaisieDto> Consomme(Guid id, string uid, ConsommeCommande c, CancellationToken a) => dispatcher.SendAsync(new SaisirConsommeTacheCommand(id, uid, c), a);

    // ---------------------------------------------------------------- périmètre

    [HttpPost("taches")]
    public Task<ResultatSaisieDto> NouvelleTache(Guid id, NouvelleTacheCommande c, CancellationToken a) => dispatcher.SendAsync(new CreerTacheProjetCommand(id, c), a);

    /// <summary>Supprime une tâche ajoutée en cours de projet (une tâche du TSP s'annule).</summary>
    [HttpDelete("taches/{uid}")]
    public Task<ResultatSaisieDto> SupprimerTache(Guid id, string uid, CancellationToken a) => dispatcher.SendAsync(new SupprimerTacheProjetCommand(id, uid), a);

    [HttpPost("taches/{uid}/annulation")]
    public Task<ResultatSaisieDto> Annuler(Guid id, string uid, AnnulationCommande c, CancellationToken a) => dispatcher.SendAsync(new AnnulerTacheProjetCommand(id, uid, c), a);

    [HttpDelete("taches/{uid}/annulation")]
    public Task<ResultatSaisieDto> Retablir(Guid id, string uid, CancellationToken a) => dispatcher.SendAsync(new RetablirTacheProjetCommand(id, uid), a);

    [HttpPost("taches/{uid}/profils")]
    public Task<ResultatSaisieDto> AjouterProfil(Guid id, string uid, AjoutProfilCommande c, CancellationToken a) => dispatcher.SendAsync(new AjouterProfilTacheCommand(id, uid, c), a);

    [HttpPut("taches/{uid}/profils/{profilId:guid}")]
    public Task<ResultatSaisieDto> ModifierAjout(Guid id, string uid, Guid profilId, ChargeAjouteeCommande c, CancellationToken a) =>
        dispatcher.SendAsync(new ModifierChargeAjouteeCommand(id, uid, profilId, c), a);

    [HttpDelete("taches/{uid}/profils/{profilId:guid}")]
    public Task<ResultatSaisieDto> RetirerAjout(Guid id, string uid, Guid profilId, CancellationToken a) => dispatcher.SendAsync(new RetirerChargeAjouteeCommand(id, uid, profilId), a);

    [HttpPost("taches/{uid}/reaffectations")]
    public Task<ResultatSaisieDto> Reaffecter(Guid id, string uid, ReaffectationCommande c, CancellationToken a) => dispatcher.SendAsync(new ReaffecterChargeTacheCommand(id, uid, c), a);

    [HttpDelete("taches/{uid}/reaffectations/{reaffectationId:guid}")]
    public Task<ResultatSaisieDto> AnnulerReaffectation(Guid id, string uid, Guid reaffectationId, CancellationToken a) =>
        dispatcher.SendAsync(new AnnulerReaffectationTacheCommand(id, uid, reaffectationId), a);

    // ---------------------------------------------------------------- ateliers et livrables

    [HttpPut("taches/{uid}/atelier/date")]
    public Task<ResultatSaisieDto> DateAtelier(Guid id, string uid, AtelierDateCommande c, CancellationToken a) => dispatcher.SendAsync(new DefinirDateAtelierCommand(id, uid, c), a);

    [HttpPut("taches/{uid}/atelier/horaire")]
    public Task<ResultatSaisieDto> HoraireAtelier(Guid id, string uid, AtelierHoraireCommande c, CancellationToken a) => dispatcher.SendAsync(new DefinirHoraireAtelierCommand(id, uid, c), a);

    [HttpPut("taches/{uid}/atelier/invitation")]
    public Task<ResultatSaisieDto> Invitation(Guid id, string uid, InvitationCommande c, CancellationToken a) => dispatcher.SendAsync(new MarquerInvitationAtelierCommand(id, uid, c), a);

    [HttpPut("taches/{uid}/atelier/invitation/date")]
    public Task<ResultatSaisieDto> DateInvitation(Guid id, string uid, AtelierDateCommande c, CancellationToken a) => dispatcher.SendAsync(new DefinirDateInvitationAtelierCommand(id, uid, c), a);

    [HttpPut("taches/{uid}/atelier/destinataires")]
    public Task<ResultatSaisieDto> Destinataires(Guid id, string uid, DestinatairesCommande c, CancellationToken a) => dispatcher.SendAsync(new DefinirDestinatairesAtelierCommand(id, uid, c), a);

    [HttpPut("taches/{uid}/livrable")]
    public Task<ResultatSaisieDto> ValidationClient(Guid id, string uid, ValidationClientCommande c, CancellationToken a) =>
        dispatcher.SendAsync(new DefinirValidationClientLivrableCommand(id, uid, c), a);

    /// <summary>Adresse du document livré (lien SharePoint, chemin réseau).</summary>
    [HttpPut("taches/{uid}/livrable/lien")]
    public Task<ResultatSaisieDto> LienLivrable(Guid id, string uid, LienLivrableCommande c, CancellationToken a) => dispatcher.SendAsync(new DefinirLienLivrableCommand(id, uid, c), a);

    // ---------------------------------------------------------------- point hebdo (diapositive de suivi delivery)

    [HttpPut("points-hebdo/semaines/{lundi}")]
    public Task<ResultatSaisieDto> PointHebdoSemaine(Guid id, string lundi, PointHebdoCommande c, CancellationToken a) =>
        dispatcher.SendAsync(new EnregistrerPointHebdoSemaineCommand(id, PointsHebdoService.CleSemaine(lundi), c), a);

    [HttpPut("points-hebdo/mois/{mois}")]
    public Task<ResultatSaisieDto> PointHebdoMois(Guid id, string mois, PointHebdoCommande c, CancellationToken a) =>
        dispatcher.SendAsync(new EnregistrerPointHebdoMoisCommand(id, PointsHebdoService.CleMois(mois), c), a);

    [HttpPut("points-hebdo/reglages/{champ}")]
    public Task<ResultatSaisieDto> PointHebdoReglage(Guid id, string champ, ReglagePointHebdoCommande c, CancellationToken a) =>
        dispatcher.SendAsync(new ReglerPointHebdoCommand(id, champ, c), a);

    /// <summary>Ajoute un message au fil de mises à jour d'une tâche (chef de projet ou consultant).</summary>
    [HttpPost("taches/{uid}/mises-a-jour")]
    public Task<ResultatSaisieDto> MiseAJour(Guid id, string uid, MiseAJourCommande c, CancellationToken a) => dispatcher.SendAsync(new AjouterMiseAJourTacheCommand(id, uid, c), a);

    // ---------------------------------------------------------------- planning de référence

    /// <summary>Fige le planning actuel comme référence.</summary>
    [HttpPost("references")]
    public Task<ResultatSaisieDto> FigerReference(Guid id, FigerReferenceCommande c, CancellationToken a) => dispatcher.SendAsync(new FigerReferenceProjetCommand(id, c), a);

    /// <summary>Cause et motif de l'écart d'une tâche par rapport à une référence.</summary>
    [HttpPut("references/{referenceId:guid}/taches/{uid}/derive")]
    public Task<ResultatSaisieDto> Derive(Guid id, Guid referenceId, string uid, DeriveCommande c, CancellationToken a) =>
        dispatcher.SendAsync(new DefinirDeriveReferenceCommand(id, referenceId, uid, c), a);

    // ---------------------------------------------------------------- facturation

    [HttpPut("echeances/{numero:int}/dates")]
    public Task<ResultatSaisieDto> DateEcheance(Guid id, int numero, EcheanceDateCommande c, CancellationToken a) => dispatcher.SendAsync(new DefinirDateEcheanceCommand(id, numero, c), a);

    [HttpPut("echeances/{numero:int}/perimetre")]
    public Task<ResultatSaisieDto> PerimetreEcheance(Guid id, int numero, PerimetreEcheanceCommande c, CancellationToken a) =>
        dispatcher.SendAsync(new LierPerimetreEcheanceCommand(id, numero, c), a);

    [HttpPut("phases/{code}/echeance")]
    public Task<ResultatSaisieDto> EcheancePhase(Guid id, string code, EcheancePhaseCommande c, CancellationToken a) =>
        dispatcher.SendAsync(new DefinirEcheancePhaseCommand(id, code, c), a);

    // ---------------------------------------------------------------- actions, décisions, risques

    [HttpPost("actions")]
    public Task<ResultatSaisieDto> CreerAction(Guid id, CancellationToken a) => dispatcher.SendAsync(new CreerActionRegistreCommand(id), a);

    [HttpPut("actions/{actionId:guid}")]
    public Task<ResultatSaisieDto> ModifierAction(Guid id, Guid actionId, ChampCommande c, CancellationToken a) => dispatcher.SendAsync(new ModifierActionRegistreCommand(id, actionId, c), a);

    [HttpPut("actions/{actionId:guid}/restaurer")]
    public Task<ResultatSaisieDto> RestaurerAction(Guid id, Guid actionId, RestaurerActionCommande c, CancellationToken a) =>
        dispatcher.SendAsync(new RestaurerActionRegistreCommand(id, actionId, c), a);

    [HttpDelete("actions/{actionId:guid}")]
    public Task<ResultatSaisieDto> SupprimerAction(Guid id, Guid actionId, CancellationToken a) => dispatcher.SendAsync(new SupprimerActionRegistreCommand(id, actionId), a);

    [HttpPost("decisions")]
    public Task<ResultatSaisieDto> CreerDecision(Guid id, CancellationToken a) => dispatcher.SendAsync(new CreerDecisionRegistreCommand(id), a);

    [HttpPut("decisions/{decisionId:guid}")]
    public Task<ResultatSaisieDto> ModifierDecision(Guid id, Guid decisionId, ChampCommande c, CancellationToken a) =>
        dispatcher.SendAsync(new ModifierDecisionRegistreCommand(id, decisionId, c), a);

    [HttpDelete("decisions/{decisionId:guid}")]
    public Task<ResultatSaisieDto> SupprimerDecision(Guid id, Guid decisionId, CancellationToken a) => dispatcher.SendAsync(new SupprimerDecisionRegistreCommand(id, decisionId), a);

    [HttpPost("risques")]
    public Task<ResultatSaisieDto> CreerRisque(Guid id, CancellationToken a) => dispatcher.SendAsync(new CreerRisqueRegistreCommand(id), a);

    [HttpPut("risques/{risqueId:guid}")]
    public Task<ResultatSaisieDto> ModifierRisque(Guid id, Guid risqueId, ChampCommande c, CancellationToken a) => dispatcher.SendAsync(new ModifierRisqueRegistreCommand(id, risqueId, c), a);

    [HttpDelete("risques/{risqueId:guid}")]
    public Task<ResultatSaisieDto> SupprimerRisque(Guid id, Guid risqueId, CancellationToken a) => dispatcher.SendAsync(new SupprimerRisqueRegistreCommand(id, risqueId), a);

    // ---------------------------------------------------------------- achats et frais

    [HttpPost("achats")]
    public Task<ResultatSaisieDto> CreerAchat(Guid id, NouvelAchatCommande c, CancellationToken a) => dispatcher.SendAsync(new CreerAchatReelCommand(id, c), a);

    [HttpPut("achats/{achatId:guid}")]
    public Task<ResultatSaisieDto> ModifierAchat(Guid id, Guid achatId, ChampCommande c, CancellationToken a) => dispatcher.SendAsync(new ModifierAchatReelCommand(id, achatId, c), a);

    [HttpDelete("achats/{achatId:guid}")]
    public Task<ResultatSaisieDto> SupprimerAchat(Guid id, Guid achatId, CancellationToken a) => dispatcher.SendAsync(new SupprimerAchatReelCommand(id, achatId), a);

    [HttpPost("frais")]
    public Task<ResultatSaisieDto> CreerFrais(Guid id, NouveauFraisCommande c, CancellationToken a) => dispatcher.SendAsync(new CreerFraisReelCommand(id, c), a);

    [HttpPut("frais/{fraisId:guid}")]
    public Task<ResultatSaisieDto> ModifierFrais(Guid id, Guid fraisId, ChampCommande c, CancellationToken a) => dispatcher.SendAsync(new ModifierFraisReelCommand(id, fraisId, c), a);

    [HttpDelete("frais/{fraisId:guid}")]
    public Task<ResultatSaisieDto> SupprimerFrais(Guid id, Guid fraisId, CancellationToken a) => dispatcher.SendAsync(new SupprimerFraisReelCommand(id, fraisId), a);

    // ---------------------------------------------------------------- paramètres

    [HttpPut("parametres/regroupement")]
    public Task<ResultatSaisieDto> Regroupement(Guid id, RegroupementCommande c, CancellationToken a) => dispatcher.SendAsync(new DefinirRegroupementProjetCommand(id, c), a);

    [HttpPut("parametres/alertes")]
    public Task<ResultatSaisieDto> Alerte(Guid id, AlerteCommande c, CancellationToken a) => dispatcher.SendAsync(new DefinirAlerteProjetCommand(id, c), a);

    [HttpPut("parametres/profil-ia")]
    public Task<ResultatSaisieDto> ProfilIa(Guid id, ProfilIaCommande c, CancellationToken a) => dispatcher.SendAsync(new DefinirProfilIaProjetCommand(id, c), a);

    [HttpPut("parametres/leviers-ia")]
    public Task<ResultatSaisieDto> LeviersIa(Guid id, ListeIaCommande c, CancellationToken a) => dispatcher.SendAsync(new DefinirListeIaProjetCommand(id, ListeIaType.Levier, c), a);

    [HttpPut("parametres/causes-ia")]
    public Task<ResultatSaisieDto> CausesIa(Guid id, ListeIaCommande c, CancellationToken a) => dispatcher.SendAsync(new DefinirListeIaProjetCommand(id, ListeIaType.Cause, c), a);

    [HttpPut("parametres/dates")]
    public Task<ResultatSaisieDto> DatesProjet(Guid id, ChampCommande c, CancellationToken a) => dispatcher.SendAsync(new DefinirDatesProjetCommand(id, c), a);

    [HttpPut("axes/{code}/nom")]
    public Task<ResultatSaisieDto> NomAxe(Guid id, string code, NomAxeCommande c, CancellationToken a) => dispatcher.SendAsync(new NommerAxeProjetCommand(id, code, c), a);

    [HttpPut("fiche")]
    public Task<ResultatSaisieDto> Fiche(Guid id, FicheCommande c, CancellationToken a) => dispatcher.SendAsync(new DefinirFicheProjetCommand(id, c), a);

    [HttpPost("types")]
    public Task<ResultatSaisieDto> AjouterType(Guid id, TypeCommande c, CancellationToken a) => dispatcher.SendAsync(new AjouterTypeProjetCommand(id, c), a);

    [HttpPut("types/{code}")]
    public Task<ResultatSaisieDto> RenommerType(Guid id, string code, TypeCommande c, CancellationToken a) => dispatcher.SendAsync(new RenommerTypeProjetCommand(id, code, c), a);

    [HttpDelete("types/{code}")]
    public Task<ResultatSaisieDto> SupprimerType(Guid id, string code, CancellationToken a) => dispatcher.SendAsync(new SupprimerTypeProjetCommand(id, code), a);

    // ---------------------------------------------------------------- exports

    /// <summary>Export COPIL (Excel) ; une photo des indicateurs est ajoutée à l'historique.</summary>
    [HttpPost("exports/copil")]
    public async Task<IActionResult> Copil(Guid id, CancellationToken a)
    {
        var f = await dispatcher.SendAsync(new ExporterCopilCommand(id), a);
        return File(f.Contenu, FichierExport.TypeXlsx, f.Nom);
    }

    /// <summary>Export COPROJ (Excel) sur une période ; la date du dernier COPROJ est retenue.</summary>
    [HttpPost("exports/coproj")]
    public async Task<IActionResult> Coproj(Guid id, CoprojCommande c, CancellationToken a)
    {
        var f = await dispatcher.SendAsync(new ExporterCoprojCommand(id, c), a);
        return File(f.Contenu, FichierExport.TypeXlsx, f.Nom);
    }
}
