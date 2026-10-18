using Franz.Common.Mediator.Handlers;
using PilotageForfaits.Contracts.Projets;

namespace PilotageForfaits.Application.Projets;

/// <summary>Route la suppression vers le cas d'usage existant et conserve ses contrôles métier.</summary>
public sealed class SupprimerProjetCommandHandler(ProjetsService projets)
    : ICommandHandler<SupprimerProjetCommand, bool>
{
    public async Task<bool> Handle(SupprimerProjetCommand command, CancellationToken cancellationToken)
    {
        await projets.SupprimerAsync(command.ProjetId, new SuppressionCommande(command.Confirmation), cancellationToken);
        return true;
    }
}
