using PilotageForfaits.Domain.Tsp;

namespace PilotageForfaits.Application.Abstractions;

/// <summary>La personne connectée, telle que la décrit son jeton (compte d'entreprise).</summary>
public interface IUtilisateurCourant
{
    bool EstAuthentifie { get; }
    /// <summary>Identifiant du compte d'entreprise (« oid » Entra ID).</summary>
    string IdentifiantExterne { get; }
    string Prenom { get; }
    string Nom { get; }
    string Email { get; }
    /// <summary>Identifiants des groupes Entra ID présents dans le jeton d’accès.</summary>
    IReadOnlyCollection<string> Groupes { get; }
}

/// <summary>Date et heure du jour. Remplaçable dans les tests pour obtenir des résultats reproductibles.</summary>
public interface IHorloge
{
    DateTimeOffset Maintenant { get; }
    /// <summary>Date du jour à l'heure de Paris (et non UTC : entre 0 h et 2 h, l'UTC donne encore la veille).</summary>
    DateOnly Aujourdhui { get; }
}

/// <summary>Lit un fichier TSP (.xlsx / .xlsm).</summary>
public interface ILecteurTsp
{
    /// <exception cref="TspInvalideException">Le fichier n'est pas un TSP exploitable.</exception>
    TspLu Lire(Stream fichier, string nomFichier);
}
