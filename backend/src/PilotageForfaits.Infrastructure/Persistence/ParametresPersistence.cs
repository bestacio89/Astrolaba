using Microsoft.EntityFrameworkCore;
using PilotageForfaits.Application.Suivi;
using PilotageForfaits.Domain.Entites;
using PilotageForfaits.Infrastructure.Donnees;

namespace PilotageForfaits.Infrastructure.Persistence;

/// <summary>EF Core implementation for project settings, task types and project identity fields.</summary>
internal sealed class ParametresPersistence(PilotageDbContext db) : IParametresPersistence
{
    public async Task<IReadOnlyList<ProfilProjet>> ListerProfilsAsync(Guid projetId, CancellationToken annulation = default) =>
        await db.Profils.Where(profil => profil.ProjetId == projetId).ToListAsync(annulation);

    public async Task<IReadOnlyList<ValeurListeIa>> ListerValeursIaAsync(
        Guid projetId,
        ListeIa liste,
        CancellationToken annulation = default) =>
        await db.ValeursIa
            .Where(valeur => valeur.ProjetId == projetId && valeur.Liste == liste)
            .ToListAsync(annulation);

    public Task<Axe?> ChargerAxeAsync(Guid projetId, string code, CancellationToken annulation = default) =>
        db.Axes.FirstOrDefaultAsync(axe => axe.ProjetId == projetId && axe.Code == code, annulation);

    public async Task<int> DernierOrdreAxeAsync(Guid projetId, CancellationToken annulation = default) =>
        (await db.Axes
            .Where(axe => axe.ProjetId == projetId)
            .Select(axe => (int?)axe.Ordre)
            .MaxAsync(annulation)) ?? -1;

    public Task<ChampFiche?> ChargerChampFicheAsync(Guid projetId, string cle, CancellationToken annulation = default) =>
        db.ChampsFiche.FirstOrDefaultAsync(champ => champ.ProjetId == projetId && champ.Cle == cle, annulation);

    public async Task<IReadOnlyList<TypeTache>> ListerTypesAsync(Guid projetId, CancellationToken annulation = default) =>
        await db.TypesTache.Where(type => type.ProjetId == projetId).ToListAsync(annulation);

    public Task<TypeTache?> ChargerTypeAsync(Guid projetId, string code, CancellationToken annulation = default) =>
        db.TypesTache.FirstOrDefaultAsync(type => type.ProjetId == projetId && type.Code == code, annulation);

    public async Task<IReadOnlyList<Tache>> ChargerTachesParTypeAsync(Guid typeId, CancellationToken annulation = default) =>
        await db.Taches.Where(tache => tache.TypeTacheId == typeId).ToListAsync(annulation);

    public Task<Projet?> ChargerProjetAsync(Guid projetId, CancellationToken annulation = default) =>
        db.Projets.FirstOrDefaultAsync(projet => projet.Id == projetId, annulation);

    public void SupprimerValeursIa(IReadOnlyList<ValeurListeIa> valeurs)
    {
        ArgumentNullException.ThrowIfNull(valeurs);
        db.ValeursIa.RemoveRange(valeurs);
    }

    public void AjouterValeurIa(ValeurListeIa valeur)
    {
        ArgumentNullException.ThrowIfNull(valeur);
        db.ValeursIa.Add(valeur);
    }

    public void AjouterAxe(Axe axe)
    {
        ArgumentNullException.ThrowIfNull(axe);
        db.Axes.Add(axe);
    }

    public void AjouterType(TypeTache type)
    {
        ArgumentNullException.ThrowIfNull(type);
        db.TypesTache.Add(type);
    }

    public void SupprimerType(TypeTache type)
    {
        ArgumentNullException.ThrowIfNull(type);
        db.TypesTache.Remove(type);
    }
}
