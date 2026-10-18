using Franz.Common.Mediator.Messages;
using PilotageForfaits.Contracts.Exports;

namespace PilotageForfaits.Contracts.Suivi;

/// <summary>Produit l'export COPROJ sur la période demandée.</summary>
public sealed record ExporterCoprojCommand(Guid ProjetId, CoprojCommande Donnees) : ICommand<FichierExport>;
