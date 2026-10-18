using Franz.Common.Mediator.Handlers;
using PilotageForfaits.Application.Exports;
using PilotageForfaits.Contracts.Exports;
using PilotageForfaits.Contracts.Suivi;

namespace PilotageForfaits.Application.Suivi;

/// <summary>Route l'export COPIL vers le service applicatif existant.</summary>
public sealed class ExporterCopilCommandHandler(ExportsService exports)
    : ICommandHandler<ExporterCopilCommand, FichierExport>
{
    public Task<FichierExport> Handle(ExporterCopilCommand command, CancellationToken cancellationToken) =>
        exports.CopilAsync(command.ProjetId, cancellationToken);
}
