using PilotageForfaits.Domain.Commun;
using PilotageForfaits.Domain.Entites;

namespace PilotageForfaits.Application.Projets;

/// <summary>
/// Persistence port for project workflows. It returns domain data needed by the use cases
/// while keeping EF query composition and explicit physical deletion in Infrastructure.
/// </summary>
public interface IProjetsPersistence
{
    Task<ProjetListePersistence> ListerAsync(
        Guid utilisateurId,
        bool voitTousLesProjets,
        bool archives,
        CancellationToken annulation = default);

    void Ajouter(Projet projet);

    Task<ProjetSuppressionPersistence> ChargerPourSuppressionAsync(
        Guid projetId,
        CancellationToken annulation = default);

    Task SupprimerDefinitivementAsync(Guid projetId, CancellationToken annulation = default);

    Task<ProjetDetailPersistence> ChargerDetailAsync(
        Guid projetId,
        CancellationToken annulation = default);
}

public sealed record ProjetListePersistence(
    IReadOnlyList<Projet> Projets,
    IReadOnlyDictionary<Guid, RoleProjet> Roles,
    IReadOnlyDictionary<Guid, string> Chefs);

public sealed record ProjetSuppressionPersistence(
    Projet Projet,
    bool AConsomme,
    bool AAchatsReels,
    bool AFraisReels,
    bool AFactureEmise);

public sealed record ProjetDetailPersistence(
    Projet Projet,
    IReadOnlyList<ChampFiche> ChampsFiche,
    ImportTsp? DernierImport);
