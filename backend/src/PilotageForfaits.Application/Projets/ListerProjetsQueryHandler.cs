using Franz.Common.Mediator.Handlers;
using PilotageForfaits.Contracts.Projets;

namespace PilotageForfaits.Application.Projets;

/// <summary>Route la requête de liste vers le service projet existant.</summary>
public sealed class ListerProjetsQueryHandler(ProjetsService projets)
    : IQueryHandler<ListerProjetsQuery, IReadOnlyList<ProjetResumeDto>>
{
    public Task<IReadOnlyList<ProjetResumeDto>> Handle(ListerProjetsQuery query, CancellationToken cancellationToken) =>
        projets.ListerAsync(query.Archives, cancellationToken);
}
