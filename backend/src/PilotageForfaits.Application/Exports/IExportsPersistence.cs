using PilotageForfaits.Domain.Entites;

namespace PilotageForfaits.Application.Exports;

/// <summary>
/// Persistence boundary for export snapshots. The implementation persists the snapshot,
/// project COPROJ date and pending journal entry in the same unit of work.
/// </summary>
public interface IExportsPersistence
{
    Task EnregistrerPhotoAsync(
        PhotoIndicateurs photo,
        DateOnly? auCoproj,
        CancellationToken annulation = default);
}
