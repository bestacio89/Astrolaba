using Franz.Common.Mediator.Messages;
using PilotageForfaits.Contracts.Exports;

namespace PilotageForfaits.Contracts.Suivi;

/// <summary>Produit l'export COPIL et conserve la photo associée si les droits l'autorisent.</summary>
public sealed record ExporterCopilCommand(Guid ProjetId) : ICommand<FichierExport>;
