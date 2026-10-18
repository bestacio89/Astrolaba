using Microsoft.EntityFrameworkCore;
using PilotageForfaits.Application.Exports;
using PilotageForfaits.Domain.Entites;
using PilotageForfaits.Infrastructure.Donnees;

namespace PilotageForfaits.Infrastructure.Persistence;

/// <summary>
/// Persists export snapshots and related project metadata. Journal entries added during the
/// same request are committed atomically with these changes.
/// </summary>
internal sealed class ExportsPersistence(PilotageDbContext db) : IExportsPersistence
{
    public async Task EnregistrerPhotoAsync(
        PhotoIndicateurs photo,
        DateOnly? auCoproj,
        CancellationToken annulation = default)
    {
        ArgumentNullException.ThrowIfNull(photo);

        var existante = await db.PhotosIndicateurs.FirstOrDefaultAsync(
            candidate => candidate.ProjetId == photo.ProjetId &&
                         candidate.Date == photo.Date &&
                         candidate.Nature == photo.Nature,
            annulation);

        if (existante is null)
        {
            db.PhotosIndicateurs.Add(photo);
        }
        else
        {
            existante.Prevu = photo.Prevu;
            existante.Consomme = photo.Consomme;
            existante.ResteAFaire = photo.ResteAFaire;
            existante.Atterrissage = photo.Atterrissage;
            existante.CoutFinal = photo.CoutFinal;
            existante.Marge = photo.Marge;
            existante.MargeNette = photo.MargeNette;
            existante.TauxMargeNette = photo.TauxMargeNette;
            existante.FinPrevue = photo.FinPrevue;
        }

        if (auCoproj is not null)
        {
            var projet = await db.Projets.FirstAsync(
                candidate => candidate.Id == photo.ProjetId,
                annulation);

            if (projet.Parametres.DernierCoproj is null ||
                auCoproj > projet.Parametres.DernierCoproj)
            {
                projet.Parametres.DernierCoproj = auCoproj;
            }
        }

        await db.SaveChangesAsync(annulation);
    }
}
