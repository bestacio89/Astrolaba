using Microsoft.EntityFrameworkCore;
using PilotageForfaits.Application.Suivi;
using PilotageForfaits.Domain.Entites;
using PilotageForfaits.Infrastructure.Donnees;

namespace PilotageForfaits.Infrastructure.Persistence;

/// <summary>
/// EF Core implementation for workshop scheduling and deliverable persistence.
/// </summary>
internal sealed class AteliersLivrablesPersistence(PilotageDbContext db) : IAteliersLivrablesPersistence
{
    public Task<Atelier?> ChargerAtelierAsync(Guid tacheId, CancellationToken annulation = default) =>
        db.Ateliers
            .Include(atelier => atelier.Destinataires)
            .FirstOrDefaultAsync(atelier => atelier.TacheId == tacheId, annulation);

    public Task<Livrable?> ChargerLivrableAsync(Guid tacheId, CancellationToken annulation = default) =>
        db.Livrables.FirstOrDefaultAsync(livrable => livrable.TacheId == tacheId, annulation);

    public void AjouterAtelier(Atelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        db.Ateliers.Add(atelier);
    }

    public void SupprimerAtelier(Atelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        db.Ateliers.Remove(atelier);
    }

    public void AjouterLivrable(Livrable livrable)
    {
        ArgumentNullException.ThrowIfNull(livrable);
        db.Livrables.Add(livrable);
    }

    public void SupprimerLivrable(Livrable livrable)
    {
        ArgumentNullException.ThrowIfNull(livrable);
        db.Livrables.Remove(livrable);
    }

    public void RemplacerDestinataires(Atelier atelier, IReadOnlyList<Destinataire> destinataires)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(destinataires);

        db.Destinataires.RemoveRange(atelier.Destinataires);
        atelier.Destinataires.Clear();

        foreach (var destinataire in destinataires)
        {
            atelier.Destinataires.Add(destinataire);
            db.Destinataires.Add(destinataire);
        }
    }
}
