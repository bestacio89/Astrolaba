using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Projets;

/// <summary>Commande de suppression définitive d'un projet.</summary>
public sealed record SupprimerProjetCommand(Guid ProjetId, string? Confirmation) : ICommand<bool>;
