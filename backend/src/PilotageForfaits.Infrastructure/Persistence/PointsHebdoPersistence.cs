using Microsoft.EntityFrameworkCore;
using PilotageForfaits.Application.Suivi;
using PilotageForfaits.Domain.Entites;
using PilotageForfaits.Infrastructure.Donnees;

namespace PilotageForfaits.Infrastructure.Persistence;

/// <summary>EF Core implementation for weekly and monthly tracking points.</summary>
internal sealed class PointsHebdoPersistence(PilotageDbContext db) : IPointsHebdoPersistence
{
    public Task<PointHebdo?> ChargerAsync(
        Guid projetId,
        string cle,
        CancellationToken annulation = default) =>
        db.PointsHebdo.FirstOrDefaultAsync(
            point => point.ProjetId == projetId && point.Cle == cle,
            annulation);

    public void Ajouter(PointHebdo point)
    {
        ArgumentNullException.ThrowIfNull(point);
        db.PointsHebdo.Add(point);
    }
}
