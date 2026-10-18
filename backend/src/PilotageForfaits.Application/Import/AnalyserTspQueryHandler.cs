using Franz.Common.Mediator.Handlers;
using PilotageForfaits.Contracts.Import;

namespace PilotageForfaits.Application.Import;

/// <summary>Route l'analyse préliminaire d'un TSP vers le service d'import existant.</summary>
public sealed class AnalyserTspQueryHandler(ImportTspService import)
    : IQueryHandler<AnalyserTspQuery, ApercuImportDto>
{
    public Task<ApercuImportDto> Handle(AnalyserTspQuery query, CancellationToken cancellationToken) =>
        import.AnalyserAsync(query.Fichier, query.NomFichier, cancellationToken);
}
