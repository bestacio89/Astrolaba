using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Projets;

/// <summary>Commande d'archivage ou de restauration d'un projet.</summary>
public sealed record ArchiverProjetCommand(Guid ProjetId, bool? Archive) : ICommand<bool>;
