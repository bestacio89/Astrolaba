using Franz.Common.Mediator.Handlers;
using PilotageForfaits.Contracts.Brouillons;

namespace PilotageForfaits.Application.Brouillons;

/// <summary>Route la liste des brouillons vers le service existant.</summary>
public sealed class ListerBrouillonsQueryHandler(BrouillonsService brouillons)
    : IQueryHandler<ListerBrouillonsQuery, IReadOnlyList<BrouillonResumeDto>>
{
    public Task<IReadOnlyList<BrouillonResumeDto>> Handle(ListerBrouillonsQuery query, CancellationToken cancellationToken) =>
        brouillons.ListerAsync(cancellationToken);
}
