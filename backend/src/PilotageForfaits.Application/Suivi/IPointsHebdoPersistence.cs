using PilotageForfaits.Domain.Entites;

namespace PilotageForfaits.Application.Suivi;

/// <summary>Persistence operations for weekly and monthly tracking points.</summary>
public interface IPointsHebdoPersistence
{
    Task<PointHebdo?> ChargerAsync(Guid projetId, string cle, CancellationToken annulation = default);
    void Ajouter(PointHebdo point);
}
