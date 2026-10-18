using System.Text.RegularExpressions;

namespace PilotageForfaits.Domain.Commun;

/// <summary>
/// Listes et règles de base du métier, reprises de la version de référence (POC v65).
/// </summary>
public static class ReferentielsMetier
{
    public const string StatutAFaire = "À faire";
    public const string StatutEnCours = "En cours";
    public const string StatutBloque = "Bloqué / En attente";
    public const string StatutEnRecette = "En recette";
    public const string StatutValide = "Validé";
    public const string StatutAnnulee = "Annulée";

    /// <summary>Statuts qu'on peut choisir pour une tâche (« Annulée » se déduit d'une annulation).</summary>
    public static readonly IReadOnlyList<string> Statuts = [StatutAFaire, StatutEnCours, StatutBloque, StatutEnRecette, StatutValide];

    /// <summary>Anciens libellés de statut (fichiers enregistrés avant le 01/10/2026).</summary>
    public static readonly IReadOnlyDictionary<string, string> AliasStatut = new Dictionary<string, string>
    {
        ["A faire"] = StatutAFaire,
        ["Test"] = StatutEnRecette,
    };

    /// <summary>Valeurs d'avancement proposées (0 %, 25 %, 50 %, 75 %, 100 %).</summary>
    public static readonly IReadOnlyList<decimal> Avancements = [0m, 0.25m, 0.5m, 0.75m, 1m];

    public static readonly IReadOnlyList<string> IaLeviers =
    [
        "Rédaction et génération de livrables", "Analyse et synthèse de données", "Scripts, paramétrage, automatisation",
        "Tests et recette automatisés", "Recherche et veille", "Réutilisation d’un actif existant", "Autre",
    ];

    public static readonly IReadOnlyList<string> IaCauses =
    [
        "Qualité insuffisante, reprise manuelle", "Outil ou licence indisponible", "Données client inexploitables",
        "Montée en compétence", "Périmètre modifié", "Autre",
    ];

    public static readonly IReadOnlyList<string> AchatStatuts = ["Commandé", "Reçu", "Facturé"];
    public static readonly IReadOnlyList<string> FraisCategories = ["Transport", "Hébergement", "Repas", "Autre"];
    public static readonly IReadOnlyList<string> Entites = ["FO", "CESSION", "ACHAT", "BO", "ACHAT BO"];

    /// <summary>Probabilité et impact d'un risque sur 3 niveaux ; criticité = probabilité × impact (1 à 9).</summary>
    public static readonly IReadOnlyList<string> RisqueProbabilites = ["Faible", "Moyenne", "Forte"];
    public static readonly IReadOnlyList<string> RisqueImpacts = ["Faible", "Moyen", "Fort"];

    /// <summary>Coût de revient journalier par défaut d'un profil BO.</summary>
    public const decimal PrcsDefaut = 450m;

    /// <summary>Phases par défaut quand le TSP n'a pas d'onglet REF.</summary>
    public static readonly IReadOnlyList<string> PhasesParDefaut =
        ["PILOTAGE", "CONCEPTION", "CONSTRUCTION", "VALIDATION", "FINALISATION", "GARANTIE", "TMA", "PROVISION"];

    public const string TypeAtelier = "Atelier";
    public const string TypeLivrable = "Livrable";

    /// <summary>Types de tâche « socle » : utilisés par le tableau de bord, le registre et les exports ; non supprimables.</summary>
    public static readonly IReadOnlyList<(string Id, string Nom)> TypesSocle = [(TypeAtelier, "Atelier"), (TypeLivrable, "Livrable")];

    /// <summary>
    /// Profils pouvant dépasser 1 JH par jour : un prestataire (ACHAT) ou une cession peut mobiliser plusieurs personnes.
    /// </summary>
    public static bool PeutDepasserUneJournee(string? entite) =>
        Regex.IsMatch((entite ?? "").ToUpperInvariant(), "^(ACHAT|CESSION)");

    /// <summary>Nom par défaut d'un axe d'après son code (colonne A du TSP).</summary>
    public static string NomAxeParDefaut(string code)
    {
        if (code == "0") return "Pilotage (transverse)";
        var m = Regex.Match(code, @"^axe\s*(\d+)", RegexOptions.IgnoreCase);
        return m.Success ? $"Axe {m.Groups[1].Value}" : code;
    }

    private const RegexOptions OptionsJs = RegexOptions.ECMAScript | RegexOptions.CultureInvariant;

    /// <summary>
    /// Devine le type d'une tâche (Atelier, Livrable ou aucun) d'après son intitulé et le marqueur « livrable » du TSP.
    /// </summary>
    public static string DetecterType(string libelle, string? marqueurLivrable)
    {
        var l = (libelle ?? "").ToLowerInvariant();
        if (Regex.IsMatch(l, @"^\s*\[\s*livrable", OptionsJs)) return TypeLivrable;
        if (Regex.IsMatch(l, @"^\s*\[\s*atelier", OptionsJs)) return TypeAtelier;
        if (Regex.IsMatch(l, @"^\s*(atelier|session|webinar|workshop)", OptionsJs)) return TypeAtelier;
        if (Regex.IsMatch(l, @"^\s*livrable", OptionsJs)) return TypeLivrable;
        if (marqueurLivrable == "oui") return TypeLivrable;
        if (Regex.IsMatch(l, @"\batelier", OptionsJs)) return TypeAtelier;
        if (Regex.IsMatch(l, @"\blivrable", OptionsJs)) return TypeLivrable;
        return "";
    }
}
