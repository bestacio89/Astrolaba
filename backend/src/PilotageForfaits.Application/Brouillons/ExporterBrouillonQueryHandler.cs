using Franz.Common.Mediator.Handlers;
using PilotageForfaits.Contracts.Brouillons;
using PilotageForfaits.Contracts.Exports;

namespace PilotageForfaits.Application.Brouillons;

/// <summary>Route l'export d'un brouillon vers le service applicatif existant.</summary>
public sealed class ExporterBrouillonQueryHandler(BrouillonsService brouillons)
    : IQueryHandler<ExporterBrouillonQuery, FichierExport>
{
    public Task<FichierExport> Handle(ExporterBrouillonQuery query, CancellationToken cancellationToken) =>
        brouillons.ExporterAsync(query.BrouillonId, cancellationToken);
}
