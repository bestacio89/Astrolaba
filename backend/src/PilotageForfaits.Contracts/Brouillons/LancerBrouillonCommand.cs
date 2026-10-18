using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Brouillons;

/// <summary>Commande de lancement d'un brouillon validé.</summary>
public sealed record LancerBrouillonCommand(Guid BrouillonId, LancementCommande Donnees) : ICommand<Guid>;
