namespace PilotageForfaits.Application.Calcul;

/// <summary>
/// Loads a project's persisted inputs for calculation. Provider-specific query composition
/// belongs in Infrastructure; the calculation builder remains independent of EF Core.
/// </summary>
public interface IChargeurDonneesProjet
{
    Task<ConstructeurProjetCalcul.DonneesProjet> ChargerDonneesAsync(
        Guid projetId,
        CancellationToken annulation = default);
}
