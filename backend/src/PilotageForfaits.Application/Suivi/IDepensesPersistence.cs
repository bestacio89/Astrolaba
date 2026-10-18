using PilotageForfaits.Domain.Entites;

namespace PilotageForfaits.Application.Suivi;

/// <summary>Persistence operations for actual purchase and expense workflows.</summary>
public interface IDepensesPersistence
{
    Task<AchatReel?> ChargerAchatAsync(Guid projetId, Guid achatId, CancellationToken annulation = default);
    Task<FraisReel?> ChargerFraisAsync(Guid projetId, Guid fraisId, CancellationToken annulation = default);
    Task<IReadOnlyList<Guid>> ListerLignesBudgetAchatAsync(Guid projetId, CancellationToken annulation = default);
    void AjouterAchat(AchatReel achat);
    void SupprimerAchat(AchatReel achat);
    void AjouterFrais(FraisReel frais);
    void SupprimerFrais(FraisReel frais);
}
