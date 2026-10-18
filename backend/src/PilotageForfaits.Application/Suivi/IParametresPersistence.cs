using PilotageForfaits.Domain.Entites;

namespace PilotageForfaits.Application.Suivi;

/// <summary>Persistence operations for project settings, task types and project identity fields.</summary>
public interface IParametresPersistence
{
    Task<IReadOnlyList<ProfilProjet>> ListerProfilsAsync(Guid projetId, CancellationToken annulation = default);
    Task<IReadOnlyList<ValeurListeIa>> ListerValeursIaAsync(Guid projetId, ListeIa liste, CancellationToken annulation = default);
    Task<Axe?> ChargerAxeAsync(Guid projetId, string code, CancellationToken annulation = default);
    Task<int> DernierOrdreAxeAsync(Guid projetId, CancellationToken annulation = default);
    Task<ChampFiche?> ChargerChampFicheAsync(Guid projetId, string cle, CancellationToken annulation = default);
    Task<IReadOnlyList<TypeTache>> ListerTypesAsync(Guid projetId, CancellationToken annulation = default);
    Task<TypeTache?> ChargerTypeAsync(Guid projetId, string code, CancellationToken annulation = default);
    Task<IReadOnlyList<Tache>> ChargerTachesParTypeAsync(Guid typeId, CancellationToken annulation = default);
    Task<Projet?> ChargerProjetAsync(Guid projetId, CancellationToken annulation = default);
    void SupprimerValeursIa(IReadOnlyList<ValeurListeIa> valeurs);
    void AjouterValeurIa(ValeurListeIa valeur);
    void AjouterAxe(Axe axe);
    void AjouterType(TypeTache type);
    void SupprimerType(TypeTache type);
}
