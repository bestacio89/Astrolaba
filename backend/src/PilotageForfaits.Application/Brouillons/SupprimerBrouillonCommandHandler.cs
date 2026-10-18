using Franz.Common.Mediator.Handlers;
using PilotageForfaits.Contracts.Brouillons;

namespace PilotageForfaits.Application.Brouillons;

/// <summary>Route la suppression d'un brouillon vers le service applicatif existant.</summary>
public sealed class SupprimerBrouillonCommandHandler(BrouillonsService brouillons)
    : ICommandHandler<SupprimerBrouillonCommand, bool>
{
    public async Task<bool> Handle(SupprimerBrouillonCommand command, CancellationToken cancellationToken)
    {
        await brouillons.SupprimerAsync(command.BrouillonId, cancellationToken);
        return true;
    }
}
