using Franz.Common.EntityFramework.Auditing;
using PilotageForfaits.Application.Abstractions;

namespace PilotageForfaits.Infrastructure.Persistence;

/// <summary>
/// Supplies Franz persistence auditing with the stable identity from the authenticated Entra ID principal.
/// The framework falls back to "system" when no authenticated principal is available.
/// </summary>
internal sealed class CurrentUserService(IUtilisateurCourant? utilisateurCourant = null) : ICurrentUserService
{
    public string? UserId =>
        utilisateurCourant?.EstAuthentifie == true &&
        !string.IsNullOrWhiteSpace(utilisateurCourant.IdentifiantExterne)
            ? utilisateurCourant.IdentifiantExterne
            : null;
}
