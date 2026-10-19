using Franz.Common.Mediator.Handlers;
using PilotageForfaits.Contracts.Moi;

namespace PilotageForfaits.Application.Securite;

/// <summary>Construit le contexte utilisateur depuis les services d'authentification applicatifs.</summary>
public sealed class ObtenirMoiQueryHandler(ServiceAcces acces)
    : IQueryHandler<ObtenirMoiQuery, MoiDto>
{
    public async Task<MoiDto> Handle(ObtenirMoiQuery query, CancellationToken cancellationToken)
    {
        var utilisateur = await acces.UtilisateurAsync(cancellationToken);
        return new MoiDto(
            utilisateur.Id,
            utilisateur.Prenom,
            utilisateur.Nom,
            utilisateur.Email,
            acces.RolesCourants,
            acces.PeutCreerUnProjet,
            acces.VoitTousLesProjets);
    }
}
