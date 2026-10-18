using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Import;

/// <summary>Analyse un TSP en lecture seule avant confirmation de l'import.</summary>
public sealed record AnalyserTspQuery(Stream Fichier, string NomFichier) : IQuery<ApercuImportDto>;
