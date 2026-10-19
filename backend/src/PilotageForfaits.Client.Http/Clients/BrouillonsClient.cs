using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using PilotageForfaits.Contracts.Brouillons;
using PilotageForfaits.Contracts.Projets;

namespace PilotageForfaits.Client.Http.Clients;

public sealed class BrouillonsClient(HttpClient httpClient) : ApiClientBase(httpClient), IBrouillonsClient
{
    public Task<IReadOnlyList<BrouillonResumeDto>> ListerAsync(CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<BrouillonResumeDto>>("api/v1/brouillons", cancellationToken);

    public Task<BrouillonCreeReponse> CreerAsync(
        CreationBrouillonCommande commande,
        CancellationToken cancellationToken = default) =>
        SendForResponseAsync<BrouillonCreeReponse>(HttpMethod.Post, "api/v1/brouillons", commande, cancellationToken);

    public Task<BrouillonCreeReponse> ImporterTspAsync(
        Stream fichier,
        string nomFichier,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fichier);
        ArgumentException.ThrowIfNullOrWhiteSpace(nomFichier);

        var contenu = new StreamContent(fichier);
        contenu.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        var formulaire = new MultipartFormDataContent();
        formulaire.Add(contenu, "fichier", Path.GetFileName(nomFichier));
        return SendContentForResponseAsync<BrouillonCreeReponse>(
            HttpMethod.Post, "api/v1/brouillons/import-tsp", formulaire, cancellationToken);
    }

    public Task<JsonObject> ObtenirAsync(Guid id, CancellationToken cancellationToken = default) =>
        GetAsync<JsonObject>($"api/v1/brouillons/{id:D}", cancellationToken);

    public Task<JsonObject> EnregistrerAsync(
        Guid id,
        JsonObject commande,
        CancellationToken cancellationToken = default) =>
        SendForResponseAsync<JsonObject>(HttpMethod.Put, $"api/v1/brouillons/{id:D}", commande, cancellationToken);

    public Task<CoRedacteurReponse> DefinirCoRedacteurAsync(
        Guid id,
        CoRedacteurCommande commande,
        CancellationToken cancellationToken = default) =>
        SendForResponseAsync<CoRedacteurReponse>(
            HttpMethod.Put, $"api/v1/brouillons/{id:D}/co-redacteur", commande, cancellationToken);

    public Task SupprimerAsync(Guid id, CancellationToken cancellationToken = default) =>
        SendWithoutResponseAsync(HttpMethod.Delete, $"api/v1/brouillons/{id:D}", cancellationToken: cancellationToken);

    public Task<DownloadedFile> ExporterAsync(Guid id, CancellationToken cancellationToken = default) =>
        DownloadAsync(HttpMethod.Get, $"api/v1/brouillons/{id:D}/export", cancellationToken: cancellationToken);

    public Task<ProjetCreeReponse> LancerAsync(
        Guid id,
        LancementCommande commande,
        CancellationToken cancellationToken = default) =>
        SendForResponseAsync<ProjetCreeReponse>(
            HttpMethod.Post, $"api/v1/brouillons/{id:D}/lancement", commande, cancellationToken);
}
