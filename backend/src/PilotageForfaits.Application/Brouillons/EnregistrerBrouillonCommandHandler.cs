using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Franz.Common.Mediator.Handlers;
using PilotageForfaits.Contracts.Brouillons;

namespace PilotageForfaits.Application.Brouillons;

/// <summary>Adapte le contrat JSON stable au modèle de saisie interne puis délègue au service applicatif.</summary>
public sealed class EnregistrerBrouillonCommandHandler(BrouillonsService brouillons)
    : ICommandHandler<EnregistrerBrouillonCommand, JsonObject>
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    public async Task<JsonObject> Handle(EnregistrerBrouillonCommand command, CancellationToken cancellationToken)
    {
        var donnees = command.Donnees.Deserialize<EnregistrementBrouillonCommande>(Json)
            ?? throw new InvalidOperationException("La commande d'enregistrement du brouillon est invalide.");
        var resultat = await brouillons.EnregistrerAsync(command.BrouillonId, donnees, cancellationToken);
        return JsonSerializer.SerializeToNode(resultat, Json)!.AsObject();
    }
}
