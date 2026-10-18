using Franz.Common.Mediator.Handlers;
using PilotageForfaits.Contracts.Projets;

namespace PilotageForfaits.Application.Import;

/// <summary>Route l'import TSP vers le service métier existant.</summary>
public sealed class ImporterTspCommandHandler(ImportTspService import)
    : ICommandHandler<ImporterTspCommand, Guid>
{
    public Task<Guid> Handle(ImporterTspCommand command, CancellationToken cancellationToken) =>
        import.ImporterAsync(command.Fichier, command.NomFichier, cancellationToken);
}
