using System.Text.Json.Nodes;
using PilotageForfaits.Contracts.Brouillons;
using PilotageForfaits.Contracts.Projets;

namespace PilotageForfaits.Client.Http.Clients;

public interface IBrouillonsClient
{
    Task<IReadOnlyList<BrouillonResumeDto>> ListerAsync(CancellationToken cancellationToken = default);
    Task<BrouillonCreeReponse> CreerAsync(CreationBrouillonCommande commande, CancellationToken cancellationToken = default);
    Task<BrouillonCreeReponse> ImporterTspAsync(Stream fichier, string nomFichier, CancellationToken cancellationToken = default);
    Task<JsonObject> ObtenirAsync(Guid id, CancellationToken cancellationToken = default);
    Task<JsonObject> EnregistrerAsync(Guid id, JsonObject commande, CancellationToken cancellationToken = default);
    Task<CoRedacteurReponse> DefinirCoRedacteurAsync(Guid id, CoRedacteurCommande commande, CancellationToken cancellationToken = default);
    Task SupprimerAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DownloadedFile> ExporterAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProjetCreeReponse> LancerAsync(Guid id, LancementCommande commande, CancellationToken cancellationToken = default);
}
