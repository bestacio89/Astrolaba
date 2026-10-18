using Franz.Common.Mediator.Handlers;
using PilotageForfaits.Contracts.Exports;
using PilotageForfaits.Contracts.Suivi;

namespace PilotageForfaits.Application.Suivi;

/// <summary>Route l'export COPROJ vers le service applicatif existant.</summary>
public sealed class ExporterCoprojCommandHandler(ExportsService exports)
    : ICommandHandler<ExporterCoprojCommand, FichierExport>
{
    public Task<FichierExport> Handle(ExporterCoprojCommand command, CancellationToken cancellationToken) =>
        exports.CoprojAsync(command.ProjetId, command.Donnees, cancellationToken);
}
