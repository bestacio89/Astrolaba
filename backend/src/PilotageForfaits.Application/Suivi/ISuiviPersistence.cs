using PilotageForfaits.Domain.Entites;

namespace PilotageForfaits.Application.Suivi;

/// <summary>
/// Read port for the project tracking view. Query composition and database-provider details
/// remain in Infrastructure; Application owns the view-model transformation.
/// </summary>
public interface ISuiviPersistence
{
    Task<JournalSuiviPersistence> ChargerJournalAsync(
        Guid projetId,
        int limite,
        CancellationToken annulation = default);

    Task<PointHebdoSuiviPersistence> ChargerPointsHebdoAsync(
        Guid projetId,
        CancellationToken annulation = default);

    Task<ComplementsSuiviPersistence> ChargerComplementsAsync(
        Guid projetId,
        CancellationToken annulation = default);
}

public sealed record JournalSuiviPersistence(
    IReadOnlyList<EntreeJournal> Entrees,
    IReadOnlyDictionary<Guid, string> NomsUtilisateurs);

public sealed record PointHebdoSuiviPersistence(
    IReadOnlyList<PointHebdo> Points,
    IReadOnlyDictionary<Guid, string> NomsUtilisateurs);

public sealed record ComplementsSuiviPersistence(
    IReadOnlyList<PlanReference> Plans,
    IReadOnlyList<LignePlanReference> Lignes,
    IReadOnlyList<MiseAJourTache> MisesAJour,
    IReadOnlyDictionary<Guid, string> NomsUtilisateurs);
