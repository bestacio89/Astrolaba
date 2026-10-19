using System.Text.Json.Nodes;
using PilotageForfaits.Contracts.Exports;
using PilotageForfaits.Contracts.Suivi;

namespace PilotageForfaits.Client.Http.Clients;

public sealed class SuiviClient(HttpClient httpClient) : ApiClientBase(httpClient), ISuiviClient
{
    public Task<JsonObject> ObtenirAsync(Guid projetId, CancellationToken cancellationToken = default) =>
        GetAsync<JsonObject>($"api/v1/projets/{projetId:D}/suivi", cancellationToken);

    public Task<ResultatSaisieDto> EnvoyerCommandeAsync(
        HttpMethod methode,
        Guid projetId,
        string routeRelative,
        object? commande = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(methode);
        ArgumentException.ThrowIfNullOrWhiteSpace(routeRelative);

        if (methode == HttpMethod.Get || methode == HttpMethod.Head)
            throw new ArgumentException("Tracking mutations must use POST, PUT, or DELETE.", nameof(methode));

        var route = $"api/v1/projets/{projetId:D}/{routeRelative.TrimStart('/')}";
        return SendForResponseAsync<ResultatSaisieDto>(methode, route, commande, cancellationToken);
    }

    public Task<DownloadedFile> ExporterCopilAsync(Guid projetId, CancellationToken cancellationToken = default) =>
        DownloadAsync(HttpMethod.Post, $"api/v1/projets/{projetId:D}/exports/copil", cancellationToken: cancellationToken);

    public Task<DownloadedFile> ExporterCoprojAsync(
        Guid projetId,
        CoprojCommande commande,
        CancellationToken cancellationToken = default) =>
        DownloadAsync(HttpMethod.Post, $"api/v1/projets/{projetId:D}/exports/coproj", commande, cancellationToken);
}
