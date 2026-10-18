namespace PilotageForfaits.Domain.Entites;

// Listes de valeurs du métier. Elles sont enregistrées en base sous forme de texte (lisible),
// et leurs libellés affichés sont dans la classe Libelles.

/// <summary>Rôle d'un utilisateur sur un projet.</summary>
public enum RoleProjet { Consultant, ChefDeProjet, Pilotage, Lecteur }

/// <summary>
/// Statut d'un projet. Seul « Archivé » est un choix (du chef de projet) ; « En préparation » et « En cours » se déduisent
/// des dates et du temps saisi (voir <see cref="Projet.StatutAffiche"/>). Un projet terminé s'archive.
/// </summary>
public enum StatutProjet { EnPreparation, EnCours, Archive }

/// <summary>Comment le projet est né : import d'un TSP, création manuelle (coquille vide en V1) ou TSP rédigé dans l'application.</summary>
public enum OrigineProjet { ImportTsp, CreationManuelle, RedactionTsp }

/// <summary>Point de départ d'un brouillon de projet.</summary>
public enum OrigineBrouillon { Vierge, ImportTsp, Copie }

/// <summary>Élément vendu au TSP ou ajouté en cours de projet.</summary>
public enum OrigineElement { Tsp, Ajout }

/// <summary>Statut choisi pour une tâche. « Annulée » n'en fait pas partie : il se déduit des évolutions du périmètre.</summary>
public enum StatutTache { AFaire, EnCours, Bloque, EnRecette, Valide }

/// <summary>Réaffectation : une partie de la charge d'un profil (ProfilSourceId) est confiée à un autre (ProfilProjetId) ; le total ne change pas.</summary>
public enum NatureEvolution { NouvelleTache, AjoutCharge, Annulation, Retablissement, Reaffectation }

public enum StatutAction { AFaire, EnCours, Fait, Annulee }

public enum StatutDecision { Validee, AValider, Rejetee }

public enum ProbabiliteRisque { Faible, Moyenne, Forte }

public enum ImpactRisque { Faible, Moyen, Fort }

public enum StatutRisque { Ouvert, Maitrise, Survenu, Clos }

public enum StatutAchat { Commande, Recu, Facture }

public enum CategorieFrais { Transport, Hebergement, Repas, Autre }

public enum NaturePhoto { Copil, Coproj, Mensuelle }

public enum ListeIa { Levier, Cause }

/// <summary>Regroupement des tâches à l'écran : par axe (colonne A du TSP), par lot (B) ou par phase (C).</summary>
public enum Regroupement { Axe, Lot, Phase }

public enum NatureNotification { Action, Livrable, Atelier, Echeance }

public enum StatutEnvoi { AEnvoyer, Envoyee, Echec }

/// <summary>Libellés affichés (identiques à la version de référence).</summary>
public static class Libelles
{
    public static string De(StatutTache s) => s switch
    {
        StatutTache.AFaire => "À faire",
        StatutTache.EnCours => "En cours",
        StatutTache.Bloque => "Bloqué / En attente",
        StatutTache.EnRecette => "En recette",
        StatutTache.Valide => "Validé",
        _ => s.ToString(),
    };

    public static string De(StatutProjet s) => s switch
    {
        StatutProjet.EnPreparation => "En préparation",
        StatutProjet.EnCours => "En cours",
        StatutProjet.Archive => "Archivé",
        _ => s.ToString(),
    };

    public static string De(OrigineProjet o) => o switch
    {
        OrigineProjet.ImportTsp => "Import d'un TSP",
        OrigineProjet.CreationManuelle => "Création manuelle",
        OrigineProjet.RedactionTsp => "TSP rédigé dans l'application",
        _ => o.ToString(),
    };

    public static string De(OrigineBrouillon o) => o switch
    {
        OrigineBrouillon.Vierge => "Rédigé dans l'application",
        OrigineBrouillon.ImportTsp => "Importé d'un TSP Excel",
        OrigineBrouillon.Copie => "Repris d'un projet existant",
        _ => o.ToString(),
    };

    public static string De(RoleProjet r) => r switch
    {
        RoleProjet.Consultant => "Consultant",
        RoleProjet.ChefDeProjet => "Chef de projet",
        RoleProjet.Pilotage => "Pilotage",
        RoleProjet.Lecteur => "Lecteur",
        _ => r.ToString(),
    };

    public static string De(StatutAction s) => s switch
    {
        StatutAction.AFaire => "À faire",
        StatutAction.EnCours => "En cours",
        StatutAction.Fait => "Fait",
        StatutAction.Annulee => "Annulée",
        _ => s.ToString(),
    };

    public static string De(StatutDecision s) => s switch
    {
        StatutDecision.Validee => "Validée",
        StatutDecision.AValider => "À valider",
        StatutDecision.Rejetee => "Rejetée",
        _ => s.ToString(),
    };

    public static string De(StatutRisque s) => s switch
    {
        StatutRisque.Ouvert => "Ouvert",
        StatutRisque.Maitrise => "Maîtrisé",
        StatutRisque.Survenu => "Survenu",
        StatutRisque.Clos => "Clos",
        _ => s.ToString(),
    };

    public static string De(ProbabiliteRisque p) => p.ToString();

    public static string De(ImpactRisque i) => i.ToString();

    public static string De(StatutAchat s) => s switch
    {
        StatutAchat.Commande => "Commandé",
        StatutAchat.Recu => "Reçu",
        StatutAchat.Facture => "Facturé",
        _ => s.ToString(),
    };

    public static string De(CategorieFrais c) => c switch
    {
        CategorieFrais.Hebergement => "Hébergement",
        _ => c.ToString(),
    };

    /// <summary>Code de regroupement attendu par le moteur de calcul.</summary>
    public static string CodeMoteur(Regroupement r) => r switch
    {
        Regroupement.Lot => "lot",
        Regroupement.Phase => "phase",
        _ => "id",
    };

    public static Regroupement RegroupementDepuisCode(string? code) => code switch
    {
        "lot" => Regroupement.Lot,
        "phase" => Regroupement.Phase,
        _ => Regroupement.Axe,
    };
}
