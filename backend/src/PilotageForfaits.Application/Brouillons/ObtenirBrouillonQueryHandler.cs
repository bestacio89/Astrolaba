using System.Text.Json;
using System.Text.Json.Nodes;
using Franz.Common.Mediator.Handlers;
using PilotageForfaits.Contracts.Brouillons;

namespace PilotageForfaits.Application.Brouillons;

/// <summary>Expose la lecture du brouillon via le contrat de transport sans déplacer les modèles métier existants.</summary>
public sealed class ObtenirBrouillonQueryHandler(BrouillonsService brouillons)
    : IQueryHandler<ObtenirBrouillonQuery, JsonObject>
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public async Task<JsonObject> Handle(ObtenirBrouillonQuery query, CancellationToken cancellationToken)
    {
        var resultat = await brouillons.LireAsync(query.BrouillonId, cancellationToken);
        return JsonSerializer.SerializeToNode(resultat, Json)!.AsObject();
    }
}
