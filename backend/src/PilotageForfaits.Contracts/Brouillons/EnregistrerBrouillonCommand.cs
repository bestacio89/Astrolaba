using System.Text.Json.Nodes;
using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Brouillons;

/// <summary>Enregistre le TSP complet du brouillon et renvoie la nouvelle version et son analyse.</summary>
/// <remarks>Le document JSON préserve le contrat TSP existant sans faire dépendre Contracts des modèles internes de l'application.</remarks>
public sealed record EnregistrerBrouillonCommand(Guid BrouillonId, JsonObject Donnees) : ICommand<JsonObject>;
