using Franz.Common.Mediator.Messages;
using PilotageForfaits.Contracts.Exports;

namespace PilotageForfaits.Contracts.Brouillons;

/// <summary>Exporte le brouillon en classeur Excel.</summary>
public sealed record ExporterBrouillonQuery(Guid BrouillonId) : IQuery<FichierExport>;
