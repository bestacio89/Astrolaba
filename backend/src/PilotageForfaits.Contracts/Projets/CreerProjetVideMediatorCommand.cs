using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Projets;

/// <summary>Commande applicative de création d'un projet vide.</summary>
public sealed record CreerProjetVideMediatorCommand(CreerProjetVideCommande Donnees) : ICommand<Guid>;
