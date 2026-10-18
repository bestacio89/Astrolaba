using Franz.Common.Mediator.Handlers;
using PilotageForfaits.Application.Abstractions;
using PilotageForfaits.Contracts.Moi;

namespace PilotageForfaits.Application.Securite;

/// <summary>Construit le contexte utilisateur depuis les services d'authentification applicatifs.</summary>
public sealed class ObtenirMoiQueryHandler(ServiceAcces acces, IUtilisateurCourant courant)
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
            courant.Roles.ToList(),
            acces.PeutCreerUnProjet,
            acces.VoitTousLesProjets);
    }
}
