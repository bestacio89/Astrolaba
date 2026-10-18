using Franz.Common.Mediator.Handlers;
using PilotageForfaits.Contracts.Brouillons;

namespace PilotageForfaits.Application.Brouillons;

/// <summary>Route la gestion du co-rédacteur vers le service existant.</summary>
public sealed class DefinirCoRedacteurBrouillonCommandHandler(BrouillonsService brouillons)
    : ICommandHandler<DefinirCoRedacteurBrouillonCommand, CoRedacteurReponse>
{
    public Task<CoRedacteurReponse> Handle(DefinirCoRedacteurBrouillonCommand command, CancellationToken cancellationToken) =>
        brouillons.DefinirCoRedacteurAsync(command.BrouillonId, command.Donnees, cancellationToken);
}
