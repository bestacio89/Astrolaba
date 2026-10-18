using System.Text.Json.Nodes;
using Franz.Common.Mediator.Handlers;
using PilotageForfaits.Contracts.Suivi;

namespace PilotageForfaits.Application.Suivi;

/// <summary>Route la lecture du suivi vers le service de lecture existant.</summary>
public sealed class ObtenirSuiviQueryHandler(SuiviService suivi)
    : IQueryHandler<ObtenirSuiviQuery, JsonObject>
{
    public Task<JsonObject> Handle(ObtenirSuiviQuery query, CancellationToken cancellationToken) =>
        suivi.ObtenirAsync(query.ProjetId, cancellationToken);
}
