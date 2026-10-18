using Microsoft.EntityFrameworkCore;
using PilotageForfaits.Application.Abstractions;
using PilotageForfaits.Domain.Entites;
using PilotageForfaits.Infrastructure.Donnees;

namespace PilotageForfaits.Infrastructure.Persistence;

/// <summary>
/// EF Core implementation of the project identity lookup. The query remains read-only and
/// uses the model's Franz soft-delete filter so deleted projects no longer reserve an identity.
/// </summary>
internal sealed class RechercheIdentiteProjet(PilotageDbContext db) : IRechercheIdentiteProjet
{
    public Task<Projet?> ChercherParCleAsync(
        string cle,
        Guid? saufProjetId = null,
        CancellationToken annulation = default) =>
        db.Projets
            .AsNoTracking()
            .FirstOrDefaultAsync(
                projet => projet.CleUnicite == cle &&
                          (saufProjetId == null || projet.Id != saufProjetId),
                annulation);
}
