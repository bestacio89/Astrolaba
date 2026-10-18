using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Brouillons;

/// <summary>Commande de création d'un brouillon.</summary>
public sealed record CreerBrouillonCommand(CreationBrouillonCommande Donnees) : ICommand<Guid>;
