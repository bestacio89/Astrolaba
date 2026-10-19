using PilotageForfaits.Contracts.Import;
using PilotageForfaits.Contracts.Projets;

namespace PilotageForfaits.Client.Http.Clients;

public interface IProjetsClient
{
    Task<IReadOnlyList<ProjetResumeDto>> ListerAsync(bool archives = false, CancellationToken cancellationToken = default);
    Task<ProjetDetailDto> ObtenirAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProjetCreeReponse> CreerVideAsync(CreerProjetVideRequete requete, CancellationToken cancellationToken = default);
    Task ArchiverAsync(Guid id, ArchivageCommande commande, CancellationToken cancellationToken = default);
    Task SupprimerAsync(Guid id, SuppressionCommande commande, CancellationToken cancellationToken = default);
    Task<ProjetCreeReponse> ImporterTspAsync(Stream fichier, string nomFichier, CancellationToken cancellationToken = default);
    Task<ApercuImportDto> AnalyserTspAsync(Stream fichier, string nomFichier, CancellationToken cancellationToken = default);
}
