using System.Text.Json.Nodes;
using PilotageForfaits.Contracts.Exports;
using PilotageForfaits.Contracts.Suivi;

namespace PilotageForfaits.Client.Http.Clients;

/// <summary>
/// Tracking operations share a large route surface and evolving request payloads. This client
/// keeps transport concerns centralized while callers use the published route templates and API contracts.
/// </summary>
public interface ISuiviClient
{
    Task<JsonObject> ObtenirAsync(Guid projetId, CancellationToken cancellationToken = default);
    Task<ResultatSaisieDto> EnvoyerCommandeAsync(
        HttpMethod methode,
        Guid projetId,
        string routeRelative,
        object? commande = null,
        CancellationToken cancellationToken = default);
    Task<DownloadedFile> ExporterCopilAsync(Guid projetId, CancellationToken cancellationToken = default);
    Task<DownloadedFile> ExporterCoprojAsync(
        Guid projetId,
        CoprojCommande commande,
        CancellationToken cancellationToken = default);
}
