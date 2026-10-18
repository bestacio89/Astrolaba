using System.Text.Json.Nodes;
using Franz.Common.Mediator.Messages;

namespace PilotageForfaits.Contracts.Brouillons;

/// <summary>Obtient un brouillon ouvert et son analyse de saisie.</summary>
public sealed record ObtenirBrouillonQuery(Guid BrouillonId) : IQuery<JsonObject>;
