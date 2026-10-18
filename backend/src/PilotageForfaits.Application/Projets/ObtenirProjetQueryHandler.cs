using Franz.Common.Mediator.Handlers;
using PilotageForfaits.Contracts.Projets;

namespace PilotageForfaits.Application.Projets;

/// <summary>Route la requête de détail vers le service projet existant.</summary>
public sealed class ObtenirProjetQueryHandler(ProjetsService projets)
    : IQueryHandler<ObtenirProjetQuery, ProjetDetailDto>
{
    public Task<ProjetDetailDto> Handle(ObtenirProjetQuery query, CancellationToken cancellationToken) =>
        projets.ObtenirAsync(query.ProjetId, cancellationToken);
}
