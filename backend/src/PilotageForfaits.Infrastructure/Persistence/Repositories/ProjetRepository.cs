using Microsoft.EntityFrameworkCore;
using PilotageForfaits.Application.Abstractions;
using PilotageForfaits.Domain.Entites;
using PilotageForfaits.Infrastructure.Donnees;

namespace PilotageForfaits.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation for project state changes.
/// Queries are tracked; commits are deliberately delegated to Franz's unit of work.
/// </summary>
public sealed class ProjetRepository(PilotageDbContext db) : IProjetRepository
{
    public Task<Projet> ObtenirPourModificationAsync(
        Guid projetId,
        CancellationToken cancellationToken = default) =>
        db.Projets.FirstAsync(projet => projet.Id == projetId, cancellationToken);
}
