using Franz.Common.Mediator.Handlers;
using PilotageForfaits.Contracts.Brouillons;

namespace PilotageForfaits.Application.Brouillons;

/// <summary>Route la création de brouillon vers le service applicatif existant.</summary>
public sealed class CreerBrouillonCommandHandler(BrouillonsService brouillons)
    : ICommandHandler<CreerBrouillonCommand, Guid>
{
    public Task<Guid> Handle(CreerBrouillonCommand command, CancellationToken cancellationToken) =>
        brouillons.CreerAsync(command.Donnees, cancellationToken);
}
