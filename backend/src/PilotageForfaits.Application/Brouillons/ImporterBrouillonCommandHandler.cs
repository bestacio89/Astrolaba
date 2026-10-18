using Franz.Common.Mediator.Handlers;
using PilotageForfaits.Contracts.Brouillons;

namespace PilotageForfaits.Application.Brouillons;

/// <summary>Route l'import de brouillon vers le service existant.</summary>
public sealed class ImporterBrouillonCommandHandler(BrouillonsService brouillons)
    : ICommandHandler<ImporterBrouillonCommand, Guid>
{
    public Task<Guid> Handle(ImporterBrouillonCommand command, CancellationToken cancellationToken) =>
        brouillons.ImporterAsync(command.Fichier, command.NomFichier, cancellationToken);
}
