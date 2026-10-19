using System.Net.Http.Headers;
using PilotageForfaits.Contracts.Import;
using PilotageForfaits.Contracts.Projets;

namespace PilotageForfaits.Client.Http.Clients;

public sealed class ProjetsClient(HttpClient httpClient) : ApiClientBase(httpClient), IProjetsClient
{
    public Task<IReadOnlyList<ProjetResumeDto>> ListerAsync(
        bool archives = false,
        CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<ProjetResumeDto>>($"api/v1/projets?archives={archives.ToString().ToLowerInvariant()}", cancellationToken);

    public Task<ProjetDetailDto> ObtenirAsync(Guid id, CancellationToken cancellationToken = default) =>
        GetAsync<ProjetDetailDto>($"api/v1/projets/{id:D}", cancellationToken);

    public Task<ProjetCreeReponse> CreerVideAsync(
        CreerProjetVideRequete requete,
        CancellationToken cancellationToken = default) =>
        SendForResponseAsync<ProjetCreeReponse>(HttpMethod.Post, "api/v1/projets", requete, cancellationToken);

    public Task ArchiverAsync(Guid id, ArchivageCommande commande, CancellationToken cancellationToken = default) =>
        SendWithoutResponseAsync(HttpMethod.Put, $"api/v1/projets/{id:D}/archivage", commande, cancellationToken);

    public Task SupprimerAsync(Guid id, SuppressionCommande commande, CancellationToken cancellationToken = default) =>
        SendWithoutResponseAsync(HttpMethod.Post, $"api/v1/projets/{id:D}/suppression", commande, cancellationToken);

    public Task<ProjetCreeReponse> ImporterTspAsync(
        Stream fichier,
        string nomFichier,
        CancellationToken cancellationToken = default) =>
        EnvoyerFichierAsync<ProjetCreeReponse>("api/v1/projets/import-tsp", fichier, nomFichier, cancellationToken);

    public Task<ApercuImportDto> AnalyserTspAsync(
        Stream fichier,
        string nomFichier,
        CancellationToken cancellationToken = default) =>
        EnvoyerFichierAsync<ApercuImportDto>("api/v1/projets/import-tsp/apercu", fichier, nomFichier, cancellationToken);

    private Task<TResponse> EnvoyerFichierAsync<TResponse>(
        string route,
        Stream fichier,
        string nomFichier,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(fichier);
        ArgumentException.ThrowIfNullOrWhiteSpace(nomFichier);

        var contenu = new StreamContent(fichier);
        contenu.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        var formulaire = new MultipartFormDataContent();
        formulaire.Add(contenu, "fichier", Path.GetFileName(nomFichier));
        return SendContentForResponseAsync<TResponse>(HttpMethod.Post, route, formulaire, cancellationToken);
    }
}
