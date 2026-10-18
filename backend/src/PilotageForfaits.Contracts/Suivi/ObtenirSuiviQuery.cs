using Franz.Common.Mediator.Messages;
using System.Text.Json.Nodes;

namespace PilotageForfaits.Contracts.Suivi;

/// <summary>Obtient le modèle de lecture du suivi d'un projet avec les montants filtrés selon les droits.</summary>
public sealed record ObtenirSuiviQuery(Guid ProjetId) : IQuery<JsonObject>;
