namespace PilotageForfaits.Client.Http.Clients;

/// <summary>Canonical relative route templates exposed by the project tracking API.</summary>
public static class SuiviRoutes
{
    public const string StatutTache = "taches/{uid}/statut";
    public const string AvancementTache = "taches/{uid}/avancement";
    public const string ResteAFaireTache = "taches/{uid}/reste-a-faire";
    public const string DatesTache = "taches/{uid}/dates";
    public const string LibelleTache = "taches/{uid}/libelle";
    public const string CommentaireTache = "taches/{uid}/commentaire";
    public const string ClassementTache = "taches/{uid}/classement";
    public const string BilanIaTache = "taches/{uid}/bilan-ia";
    public const string EcheanceTache = "taches/{uid}/echeance";
    public const string ConsommeTache = "taches/{uid}/consomme";
    public const string Taches = "taches";
    public const string Tache = "taches/{uid}";
    public const string AnnulationTache = "taches/{uid}/annulation";
    public const string ProfilsTache = "taches/{uid}/profils";
    public const string ProfilTache = "taches/{uid}/profils/{profilId}";
    public const string ReaffectationsTache = "taches/{uid}/reaffectations";
    public const string ReaffectationTache = "taches/{uid}/reaffectations/{reaffectationId}";
    public const string DateAtelier = "taches/{uid}/atelier/date";
    public const string HoraireAtelier = "taches/{uid}/atelier/horaire";
    public const string InvitationAtelier = "taches/{uid}/atelier/invitation";
    public const string DateInvitationAtelier = "taches/{uid}/atelier/invitation/date";
    public const string DestinatairesAtelier = "taches/{uid}/atelier/destinataires";
    public const string Livrable = "taches/{uid}/livrable";
    public const string LienLivrable = "taches/{uid}/livrable/lien";
    public const string PointHebdoSemaine = "points-hebdo/semaines/{lundi}";
    public const string PointHebdoMois = "points-hebdo/mois/{mois}";
    public const string ReglagePointHebdo = "points-hebdo/reglages/{champ}";
    public const string MisesAJourTache = "taches/{uid}/mises-a-jour";
    public const string References = "references";
    public const string DeriveReference = "references/{referenceId}/taches/{uid}/derive";
    public const string DateEcheance = "echeances/{numero}/dates";
    public const string PerimetreEcheance = "echeances/{numero}/perimetre";
    public const string EcheancePhase = "phases/{code}/echeance";
    public const string Actions = "actions";
    public const string Action = "actions/{actionId}";
    public const string RestaurerAction = "actions/{actionId}/restaurer";
    public const string Decisions = "decisions";
    public const string Decision = "decisions/{decisionId}";
    public const string Risques = "risques";
    public const string Risque = "risques/{risqueId}";
    public const string Achats = "achats";
    public const string Achat = "achats/{achatId}";
    public const string Frais = "frais";
    public const string FraisReel = "frais/{fraisId}";
    public const string Regroupement = "parametres/regroupement";
    public const string Alertes = "parametres/alertes";
    public const string ProfilIa = "parametres/profil-ia";
    public const string LeviersIa = "parametres/leviers-ia";
    public const string CausesIa = "parametres/causes-ia";
    public const string DatesProjet = "parametres/dates";
    public const string NomAxe = "axes/{code}/nom";
    public const string Fiche = "fiche";
    public const string Types = "types";
    public const string Type = "types/{code}";
    public const string ExportCopil = "exports/copil";
    public const string ExportCoproj = "exports/coproj";

    public static string Formater(string modele, params (string Parametre, string Valeur)[] valeurs)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(modele);
        ArgumentNullException.ThrowIfNull(valeurs);

        foreach (var (parametre, valeur) in valeurs)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(parametre);
            ArgumentNullException.ThrowIfNull(valeur);
            modele = modele.Replace("{" + parametre + "}", Uri.EscapeDataString(valeur), StringComparison.Ordinal);
        }

        if (modele.Contains('{') || modele.Contains('}'))
            throw new ArgumentException("Every route parameter must be supplied.", nameof(valeurs));

        return modele;
    }
}
