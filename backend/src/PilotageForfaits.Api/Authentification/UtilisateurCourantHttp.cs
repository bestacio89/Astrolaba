using System.Security.Claims;
using PilotageForfaits.Application.Abstractions;

namespace PilotageForfaits.Api.Authentification;

/// <summary>
/// Lit l'utilisateur connecté dans le jeton de la requête. Les noms des informations (« claims ») diffèrent selon la
/// configuration du jeton Entra ID : on accepte les deux formes (courte et longue).
/// </summary>
public sealed class UtilisateurCourantHttp(IHttpContextAccessor accesseur) : IUtilisateurCourant
{
    private ClaimsPrincipal? Utilisateur => accesseur.HttpContext?.User;

    private string Valeur(params string[] types) =>
        types.Select(t => Utilisateur?.FindFirst(t)?.Value).FirstOrDefault(v => !string.IsNullOrEmpty(v)) ?? "";

    public bool EstAuthentifie => Utilisateur?.Identity?.IsAuthenticated == true;

    public string IdentifiantExterne => Valeur("oid", "http://schemas.microsoft.com/identity/claims/objectidentifier");

    public string Prenom => Valeur("given_name", ClaimTypes.GivenName);

    public string Nom
    {
        get
        {
            var nom = Valeur("family_name", ClaimTypes.Surname);
            return nom.Length > 0 ? nom : Valeur("name");
        }
    }

    public string Email => Valeur("preferred_username", "email", ClaimTypes.Email, ClaimTypes.Upn);

    /// <summary>
    /// Identifiants immuables des groupes Entra ID présents dans le jeton d'accès.
    /// Les noms d'affichage des groupes ne sont jamais utilisés comme autorité d'accès.
    /// </summary>
    public IReadOnlyCollection<string> Groupes =>
        (Utilisateur?.Claims ?? [])
            .Where(c => c.Type is "groups"
                or "http://schemas.microsoft.com/ws/2008/06/identity/claims/groups"
                or ClaimTypes.GroupSid)
            .Select(c => c.Value)
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
}
