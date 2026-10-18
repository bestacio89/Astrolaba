using PilotageForfaits.Domain.Entites;

namespace PilotageForfaits.Application.Abstractions;

/// <summary>
/// Persistence operations for project aggregates that require a tracked entity.
/// The repository does not commit; the caller owns the unit-of-work boundary.
/// </summary>
public interface IProjetRepository
{
    /// <summary>Loads a project for a state change within the current scoped unit of work.</summary>
    Task<Projet> ObtenirPourModificationAsync(Guid projetId, CancellationToken cancellationToken = default);
}
