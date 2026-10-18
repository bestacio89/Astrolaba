using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Brouillons;

/// <summary>Définit ou retire le co-rédacteur d'un brouillon.</summary>
public sealed record DefinirCoRedacteurBrouillonCommand(Guid BrouillonId, CoRedacteurCommande Donnees) : ICommand<CoRedacteurReponse>;
