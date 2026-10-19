using System.Globalization;
using Franz.Common.Business.Repositories;
using Microsoft.Extensions.Logging;
using PilotageForfaits.Application.Abstractions;
using PilotageForfaits.Application.Calcul;
using PilotageForfaits.Application.Commun;
using PilotageForfaits.Application.Securite;
using PilotageForfaits.Domain.Calcul;
using PilotageForfaits.Domain.Commun;
using PilotageForfaits.Domain.Entites;

namespace PilotageForfaits.Application.Projets;

/// <summary>Cas d'usage autour des projets : liste selon les habilitations, création d'un projet vide, vue d'ensemble.</summary>
public sealed class ProjetsService(
    IProjetsPersistence persistence, IEntityRepository<Projet, Guid> projetsRepository,
    ServiceAcces acces, ConstructeurProjetCalcul constructeur, IHorloge horloge, ServiceJournal journal, ILogger<ProjetsService> traces,
    UniciteProjet unicite)
{
    /// <summary>Projets visibles par l'utilisateur connecté, avec leurs indicateurs principaux ; « archives » : seulement les projets archivés.</summary>
    public async Task<IReadOnlyList<ProjetResumeDto>> ListerAsync(bool archives, CancellationToken annulation = default)
    {
        var utilisateur = await acces.UtilisateurAsync(annulation);
        var donneesListe = await persistence.ListerAsync(
            utilisateur.Id, acces.VoitTousLesProjets, archives, annulation);
        var projets = donneesListe.Projets;
        var chefs = donneesListe.Chefs;

        // The persistence query scopes candidates to database assignments. Recheck each candidate
        // against Entra group membership before returning any project data, including financial metrics.
        // Indicators are recalculated per visible project; no result is persisted.
        var liste = new List<ProjetResumeDto>();
        foreach (var projet in projets)
        {
            var roleAutorise = await acces.RoleSurProjetAsync(projet.Id, annulation);
            if (roleAutorise is null)
                continue;

            var role = roleAutorise.Value;
            var calcul = MoteurCalcul.Calculer(await constructeur.ChargerAsync(projet.Id, annulation), horloge.Aujourdhui);
            var voitMontants = ServiceAcces.VoitMontants(role);
            var aTraiter = PointsATraiter(calcul);
            if (calcul.Tasks.Count == 0)
                aTraiter.Add(new PointATraiterDto("preparation", "Projet vide : aucune tâche saisie", "neutre",
                    projet.Debut is { } debut ? $"démarre le {Calendrier.Fr(debut)}" : null, "", null));
            var ventesVisibles = voitMontants && calcul.Financier.Ca != 0;
            var statut = projet.StatutAffiche(horloge.Aujourdhui, calcul.Total.Conso > 0);
            liste.Add(new ProjetResumeDto(
                projet.Id, projet.Nom, projet.Client, projet.Reference, Libelles.De(statut), statut.ToString(),
                Libelles.De(role), role.ToString(), voitMontants, calcul.Tasks.Count == 0,
                calcul.Total.Avancement, calcul.Total.Prevu, calcul.Total.Conso, calcul.Total.Atterrissage, calcul.Total.Ecart,
                // marge NETTE (production + achats + frais), comme l'écran Rentabilité : un seul sens de « marge » dans les listes
                ventesVisibles ? calcul.Financier.TauxMargeNette : null, ventesVisibles ? calcul.Financier.TauxMargeNetteVendue : null,
                projet.Debut, projet.FinPrevue, projet.FinContractuelle, calcul.Cal.JoursRestants, chefs.GetValueOrDefault(projet.Id),
                projet.CreeLe, aTraiter.Count, aTraiter.Take(3).ToList()));
        }
        return liste;
    }

    /// <summary>
    /// Crée un projet vide (V1) : la fiche seule, sans vendu. Le créateur en devient chef de projet.
    /// </summary>
    public async Task<Guid> CreerVideAsync(CreerProjetVideCommande commande, CancellationToken annulation = default)
    {
        var utilisateur = await acces.UtilisateurAsync(annulation);
        if (!acces.PeutCreerUnProjet) throw new AccesRefuseException("Vous n'avez pas le droit de créer un projet.");
        await unicite.ExigerLibreAsync(commande.Reference, commande.Client, commande.Nom, null, annulation);
        var projet = Projet.CreerVide(commande.Client, commande.Nom, commande.Reference, commande.Debut, commande.FinContractuelle,
            commande.TypeContrat, utilisateur.Id, horloge.Maintenant);
        projet.Habilitations.Add(new Habilitation
        {
            ProjetId = projet.Id, UtilisateurId = utilisateur.Id, Role = RoleProjet.ChefDeProjet, Depuis = horloge.Maintenant, AccordeeParId = utilisateur.Id,
        });
        persistence.Ajouter(projet);
        journal.Noter(projet.Id, utilisateur.Id, "Projet", projet.Id, "Création", null, "Projet vide");
        // The Franz mediator persistence behavior commits the command after successful completion.
        return projet.Id;
    }

    /// <summary>
    /// Archive un projet (il quitte les listes et passe en lecture seule ; rien n'est perdu) ou le restaure.
    /// Réservé au chef de projet. Restauré, il reprend le statut déduit de ses dates et du temps saisi.
    /// </summary>
    public async Task ArchiverAsync(Guid projetId, ArchivageCommande commande, CancellationToken annulation = default)
    {
        var role = await acces.ExigerAccesAsync(projetId, annulation);
        if (!ServiceAcces.PeutModifier(role)) throw new AccesRefuseException("Seul le chef de projet peut archiver ou restaurer ce projet.");
        var u = await acces.UtilisateurAsync(annulation);
        var projet = await projetsRepository.GetByIdAsync(projetId, annulation);
        var avant = projet.Statut;
        if (commande.Archive is null) throw new RegleMetierException("Indiquez s'il faut archiver ou restaurer le projet.");
        if (commande.Archive == true) projet.Statut = StatutProjet.Archive;
        else if (projet.Statut == StatutProjet.Archive)
            projet.Statut = StatutProjet.EnCours; // seul « Archivé » compte : le statut affiché se recalcule
        if (projet.Statut == avant) return;
        journal.Noter(projetId, u.Id, "Projet", projetId, "statut", avant == StatutProjet.Archive ? "Archivé" : "Actif",
            projet.Statut == StatutProjet.Archive ? "Archivé" : "Restauré");
        // The Franz mediator persistence behavior commits the archive and journal entry atomically.
    }

    /// <summary>
    /// Supprime DÉFINITIVEMENT un projet et toutes ses données. Réservé au chef de projet, et seulement pour un projet créé
    /// par erreur : s'il a déjà un suivi (temps passé, achats ou frais réels, facture émise), la suppression est refusée,
    /// il faut l'archiver (« rien ne se supprime sans trace »). Le nom du projet doit être retapé pour confirmer.
    /// Le journal du projet disparaît avec lui : la suppression est tracée dans le journal du serveur.
    /// </summary>
    public async Task SupprimerAsync(Guid projetId, SuppressionCommande commande, CancellationToken annulation = default)
    {
        var role = await acces.ExigerAccesAsync(projetId, annulation);
        if (!ServiceAcces.PeutModifier(role)) throw new AccesRefuseException("Seul le chef de projet peut supprimer ce projet.");
        var u = await acces.UtilisateurAsync(annulation);
        var suppression = await persistence.ChargerPourSuppressionAsync(projetId, annulation);
        var projet = suppression.Projet;
        if (!string.Equals(SaisieTexte(commande.Confirmation), SaisieTexte(projet.Nom), StringComparison.OrdinalIgnoreCase))
            throw new RegleMetierException($"Pour confirmer, tapez exactement le nom du projet : « {projet.Nom} ».");
        var suivi = new List<string>();
        if (suppression.AConsomme) suivi.Add("du temps passé");
        if (suppression.AAchatsReels) suivi.Add("des achats");
        if (suppression.AFraisReels) suivi.Add("des frais");
        if (suppression.AFactureEmise) suivi.Add("une facture émise");
        if (suivi.Count > 0)
            throw new RegleMetierException($"Ce projet a déjà un suivi ({string.Join(", ", suivi)}) : il ne peut pas être supprimé. Archivez-le : il quitte les listes et rien n'est perdu.");
        await persistence.SupprimerDefinitivementAsync(projetId, annulation);
        traces.LogWarning("Projet supprimé définitivement : {ProjetId} « {Client} — {Nom} » par {Utilisateur} ({UtilisateurId}).",
            projetId, projet.Client, projet.Nom, u.NomComplet, u.Id);
    }

    private static string SaisieTexte(string? s) => string.Join(' ', (s ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));

    /// <summary>Vue d'ensemble d'un projet.</summary>
    public async Task<ProjetDetailDto> ObtenirAsync(Guid projetId, CancellationToken annulation = default)
    {
        var role = await acces.ExigerAccesAsync(projetId, annulation);
        var voitMontants = ServiceAcces.VoitMontants(role);
        var donneesDetail = await persistence.ChargerDetailAsync(projetId, annulation);
        var projet = donneesDetail.Projet;
        var fiche = donneesDetail.ChampsFiche
            .Select(champ => new ChampFicheDto(champ.Libelle, champ.Valeur))
            .ToList();
        var dernierImport = donneesDetail.DernierImport;
        var donnees = await constructeur.ChargerAsync(projetId, annulation);
        var c = MoteurCalcul.Calculer(donnees, horloge.Aujourdhui);
        decimal? Montant(decimal v) => voitMontants ? v : null;
        var f = c.Financier;

        var statut = projet.StatutAffiche(horloge.Aujourdhui, c.Total.Conso > 0);
        return new ProjetDetailDto(
            projet.Id, projet.Nom, projet.Client, projet.Reference, Libelles.De(statut), statut.ToString(),
            Libelles.De(projet.Origine),
            Libelles.De(role), role.ToString(), voitMontants, ServiceAcces.PeutModifier(role) && projet.Statut != StatutProjet.Archive, c.Tasks.Count == 0,
            projet.Debut, projet.FinContractuelle, projet.FinPrevue, fiche,
            new IndicateursDto(
                c.Total.Actives, c.Total.Valides, c.Total.Avancement, c.Total.Prevu, c.Total.PrevuTSP, c.Total.Conso, c.Total.Raf,
                c.Total.Atterrissage, c.Total.Ecart,
                Montant(f.Ca), Montant(f.CoutFinal), Montant(f.Marge), voitMontants && f.Ca != 0 ? f.TauxMarge : null,
                Montant(f.MargeNette), voitMontants && f.Ca != 0 ? f.TauxMargeNette : null),
            new CalendrierDto(c.Cal.JoursTotal, c.Cal.JoursEcoules, c.Cal.TempsPct, c.Cal.ConsoPct, c.Cal.GlissementJours),
            PointsATraiter(c),
            c.Axes.Select(a => new GroupeDto(a.Id, a.Nom, a.N, a.Prevu, a.Conso, a.Raf, a.Atterrissage, a.Ecart, a.Avancement, a.Statut)).ToList(),
            c.Dim.Label,
            c.Echeancier.Select(e => new EcheanceDto(e.N, e.Nom, e.Pct, Montant(e.Montant), e.DatePrevue, e.RealiseLe, e.Statut, e.RealiseLe is null)).ToList(),
            dernierImport is null ? null : new ImportDto(dernierImport.NomFichier, dernierImport.ImporteLe,
                dernierImport.PointsReleves.Split('\n', StringSplitOptions.RemoveEmptyEntries)));
    }

    /// <summary>Points de vigilance qui demandent une action : les critiques d'abord, puis ceux à surveiller.</summary>
    public static List<PointATraiterDto> PointsATraiter(ResultatCalcul calcul) => calcul.Vigilance
        .Where(v => v.Niveau != "ok")
        .OrderBy(v => v.Niveau == "critique" ? 0 : 1)
        .Select(v => new PointATraiterDto(v.Cle, v.Label, v.Niveau, v.Detail, Convert.ToString(v.Valeur, CultureInfo.InvariantCulture) ?? "", v.Unite))
        .ToList();
}
