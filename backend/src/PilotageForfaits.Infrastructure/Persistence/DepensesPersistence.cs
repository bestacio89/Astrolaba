using Microsoft.EntityFrameworkCore;
using PilotageForfaits.Application.Suivi;
using PilotageForfaits.Domain.Entites;
using PilotageForfaits.Infrastructure.Donnees;

namespace PilotageForfaits.Infrastructure.Persistence;

/// <summary>EF Core implementation for actual purchase and expense workflows.</summary>
internal sealed class DepensesPersistence(PilotageDbContext db) : IDepensesPersistence
{
    public Task<AchatReel?> ChargerAchatAsync(Guid projetId, Guid achatId, CancellationToken annulation = default) =>
        db.AchatsReels.FirstOrDefaultAsync(
            achat => achat.ProjetId == projetId && achat.Id == achatId,
            annulation);

    public Task<FraisReel?> ChargerFraisAsync(Guid projetId, Guid fraisId, CancellationToken annulation = default) =>
        db.FraisReels.FirstOrDefaultAsync(
            frais => frais.ProjetId == projetId && frais.Id == fraisId,
            annulation);

    public async Task<IReadOnlyList<Guid>> ListerLignesBudgetAchatAsync(
        Guid projetId,
        CancellationToken annulation = default) =>
        await db.BudgetsAchats
            .Where(ligne => ligne.ProjetId == projetId)
            .OrderBy(ligne => ligne.Ordre)
            .Select(ligne => ligne.Id)
            .ToListAsync(annulation);

    public void AjouterAchat(AchatReel achat)
    {
        ArgumentNullException.ThrowIfNull(achat);
        db.AchatsReels.Add(achat);
    }

    public void SupprimerAchat(AchatReel achat)
    {
        ArgumentNullException.ThrowIfNull(achat);
        db.AchatsReels.Remove(achat);
    }

    public void AjouterFrais(FraisReel frais)
    {
        ArgumentNullException.ThrowIfNull(frais);
        db.FraisReels.Add(frais);
    }

    public void SupprimerFrais(FraisReel frais)
    {
        ArgumentNullException.ThrowIfNull(frais);
        db.FraisReels.Remove(frais);
    }
}
