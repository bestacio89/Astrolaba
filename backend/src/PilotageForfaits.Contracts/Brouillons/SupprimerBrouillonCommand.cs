using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Brouillons;

/// <summary>Commande de suppression d'un brouillon.</summary>
public sealed record SupprimerBrouillonCommand(Guid BrouillonId) : ICommand<bool>;
