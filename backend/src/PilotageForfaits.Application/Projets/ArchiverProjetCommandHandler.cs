using Franz.Common.Mediator.Handlers;
using PilotageForfaits.Application.Projets;
using PilotageForfaits.Contracts.Projets;

namespace PilotageForfaits.Application.Projets;

/// <summary>Dispatche l'archivage vers le cas d'usage existant pendant la migration progressive.</summary>
public sealed class ArchiverProjetCommandHandler(ProjetsService projets)
    : ICommandHandler<ArchiverProjetCommand, bool>
{
    public async Task<bool> Handle(ArchiverProjetCommand command, CancellationToken cancellationToken)
    {
        await projets.ArchiverAsync(
            command.ProjetId,
            new ArchivageCommande(command.Archive),
            cancellationToken);

        return true;
    }
}
