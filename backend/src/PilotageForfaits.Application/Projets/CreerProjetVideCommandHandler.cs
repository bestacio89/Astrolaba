using Franz.Common.Mediator.Handlers;
using PilotageForfaits.Contracts.Projets;

namespace PilotageForfaits.Application.Projets;

/// <summary>Route la création de projet vers le cas d'usage existant.</summary>
public sealed class CreerProjetVideCommandHandler(ProjetsService projets)
    : ICommandHandler<CreerProjetVideMediatorCommand, Guid>
{
    public Task<Guid> Handle(CreerProjetVideMediatorCommand command, CancellationToken cancellationToken) =>
        projets.CreerVideAsync(command.Donnees, cancellationToken);
}
