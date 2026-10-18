using PilotageForfaits.Domain.Entites;

namespace PilotageForfaits.Application.Abstractions;

/// <summary>
/// Query port for the project identity invariant. The implementation owns provider-specific
/// query composition and respects Franz's global soft-delete filter.
/// </summary>
public interface IRechercheIdentiteProjet
{
    Task<Projet?> ChercherParCleAsync(
        string cle,
        Guid? saufProjetId = null,
        CancellationToken annulation = default);
}
