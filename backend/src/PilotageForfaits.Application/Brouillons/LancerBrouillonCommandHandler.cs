using Franz.Common.Mediator.Handlers;
using PilotageForfaits.Contracts.Brouillons;

namespace PilotageForfaits.Application.Brouillons;

/// <summary>Route le lancement d'un brouillon vers le service applicatif existant.</summary>
public sealed class LancerBrouillonCommandHandler(BrouillonsService brouillons)
    : ICommandHandler<LancerBrouillonCommand, Guid>
{
    public Task<Guid> Handle(LancerBrouillonCommand command, CancellationToken cancellationToken) =>
        brouillons.LancerAsync(command.BrouillonId, command.Donnees, cancellationToken);
}
