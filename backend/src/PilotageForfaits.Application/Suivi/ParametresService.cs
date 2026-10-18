using PilotageForfaits.Application.Commun;
using PilotageForfaits.Domain.Commun;
using PilotageForfaits.Domain.Entites;
using PilotageForfaits.Application.Projets;

namespace PilotageForfaits.Application.Suivi;

/// <summary>
/// Réglages du suivi (valables pour toute l'équipe du projet) : regroupement, alertes, optimisation IA, dates du projet,
/// noms des axes, fiche (client, projet, référence) et types de tâches. Règles du POC.
/// </summary>
public sealed class ParametresService(SaisieOutils o, UniciteProjet unicite, IParametresPersistence persistence)
{
    /// <summary>Délais d'alerte proposés (jours avant la date).</summary>
    public static readonly IReadOnlyList<int> DelaisAlerte = [0, 1, 2, 3, 5, 7, 10, 14, 21, 30];

    public async Task<ResultatSaisieDto> DefinirRegroupementAsync(Guid projetId, RegroupementCommande c, CancellationToken a)
    {
        var u = await o.ExigerChefAsync(projetId, a);
        if (c.GroupBy is not ("id" or "lot" or "phase")) throw new RegleMetierException("Regroupement inconnu.");
        var p = await ProjetAsync(projetId, a);
        var ancien = p.Parametres.Regroupement;
        p.Parametres.Regroupement = Libelles.RegroupementDepuisCode(c.GroupBy);
        o.Noter(projetId, u, "Paramètres", p.Id, "regrouper par", ancien.ToString(), c.GroupBy);
        var libelle = c.GroupBy switch { "lot" => "lot", "phase" => "phase", _ => "axe" };
        return await o.TerminerAsync(projetId, new List<MessageDto> { new($"Reporting regroupé par {libelle}.", "succes") }, a);
    }

    public async Task<ResultatSaisieDto> DefinirAlerteAsync(Guid projetId, AlerteCommande c, CancellationToken a)
    {
        var u = await o.ExigerChefAsync(projetId, a);
        if (!DelaisAlerte.Contains(c.Jours)) throw new RegleMetierException("Délai d’alerte non proposé.");
        var p = await ProjetAsync(projetId, a);
        var j = c.Jours;
        var avant = j == 0 ? null : $"{j} jour(s) avant";
        string message;
        switch (c.Champ)
        {
            case "alerteActionsJours":
                p.Parametres.AlerteActionsJours = j;
                message = $"Les actions apparaîtront sur le tableau de bord {(avant is null ? "le jour de leur échéance" : avant + " leur échéance")}.";
                break;
            case "alerteAtelierJours":
                p.Parametres.AlerteAteliersJours = j;
                message = $"Les ateliers sans invitation apparaîtront {avant ?? "le jour même"}.";
                break;
            case "alerteLivrableJours":
                p.Parametres.AlerteLivrablesJours = j;
                message = $"Les livrables à remettre apparaîtront {avant ?? "le jour même"}.";
                break;
            case "alerteEchJours":
                p.Parametres.AlerteEcheancesJours = j;
                message = $"Les échéances apparaîtront sur le tableau de bord {(avant is null ? "le jour de leur date prévue" : avant + " leur date prévue")}.";
                break;
            default:
                throw new RegleMetierException($"Réglage inconnu : « {c.Champ} ».");
        }
        o.Noter(projetId, u, "Paramètres", p.Id, c.Champ, null, j);
        return await o.TerminerAsync(projetId, new List<MessageDto> { new(message, "succes") }, a);
    }

    /// <summary>Profil dont la charge est l'enveloppe IA de chaque tâche (un seul ; aucun possible).</summary>
    public async Task<ResultatSaisieDto> DefinirProfilIaAsync(Guid projetId, ProfilIaCommande c, CancellationToken a)
    {
        var u = await o.ExigerChefAsync(projetId, a);
        var profils = (await persistence.ListerProfilsAsync(projetId, a)).ToList();
        if (c.ProfilId is not null && profils.All(p => p.Id != c.ProfilId)) throw new IntrouvableException("Profil introuvable.");
        foreach (var p in profils) p.EstAgentIa = p.Id == c.ProfilId;
        o.Noter(projetId, u, "Paramètres", projetId, "profil IA", null, profils.FirstOrDefault(p => p.Id == c.ProfilId)?.Nom ?? "aucun");
        return await o.TerminerAsync(projetId, new List<MessageDto>(), a);
    }

    /// <summary>Leviers d'optimisation IA ou causes de dépassement : un par ligne ; liste vide → liste par défaut.</summary>
    public async Task<ResultatSaisieDto> DefinirListeIaAsync(Guid projetId, ListeIa liste, ListeIaCommande c, CancellationToken a)
    {
        var u = await o.ExigerChefAsync(projetId, a);
        var valeurs = new List<string>();
        foreach (var v in c.Valeurs ?? new List<string>())
        {
            var x = SaisieOutils.Nettoyer(v);
            if (x.Length > 0 && !valeurs.Contains(x)) valeurs.Add(x.Length > 400 ? x[..400] : x);
        }
        persistence.SupprimerValeursIa(await persistence.ListerValeursIaAsync(projetId, liste, a));
        for (var i = 0; i < valeurs.Count; i++) persistence.AjouterValeurIa(new ValeurListeIa { ProjetId = projetId, Liste = liste, Libelle = valeurs[i], Ordre = i });
        o.Noter(projetId, u, "Paramètres", projetId, liste == ListeIa.Levier ? "leviers IA" : "causes de dépassement IA", null, string.Join(" ; ", valeurs));
        var message = valeurs.Count > 0 ? $"{valeurs.Count} type(s) enregistré(s)." : "Liste par défaut rétablie.";
        return await o.TerminerAsync(projetId, new List<MessageDto> { new(message, "succes") }, a);
    }

    /// <summary>
    /// Une date du projet (debut, finContrat, finPrevue). La fin prévue suit la fin contractuelle tant qu'elle n'a pas été modifiée ;
    /// aucune fin avant le début.
    /// </summary>
    public async Task<ResultatSaisieDto> DefinirDateAsync(Guid projetId, ChampCommande c, CancellationToken a)
    {
        var u = await o.ExigerChefAsync(projetId, a);
        var p = await ProjetAsync(projetId, a);
        SaisieOutils.VerifierVersion(p, c.Version, "Le projet");
        var v = Formats.LireDate(c.Valeur);
        DateOnly? debut = p.Debut, finContrat = p.FinContractuelle, finPrevue = p.FinPrevue;
        switch (c.Champ)
        {
            case "debut": debut = v; break;
            case "finContrat":
                if (finPrevue is null || finPrevue == finContrat) finPrevue = v; // la fin prévue suit tant qu'elle n'a pas été modifiée
                finContrat = v;
                break;
            case "finPrevue": finPrevue = v; break;
            default: throw new RegleMetierException($"Date inconnue : « {c.Champ} ».");
        }
        if (debut is not null && ((finContrat is not null && finContrat < debut) || (finPrevue is not null && finPrevue < debut)))
            throw new RegleMetierException("La date de fin doit être postérieure au début du projet.");
        o.Noter(projetId, u, "Projet", p.Id, c.Champ, c.Champ switch { "debut" => p.Debut, "finContrat" => p.FinContractuelle, _ => p.FinPrevue }, v);
        p.Debut = debut;
        p.FinContractuelle = finContrat;
        p.FinPrevue = finPrevue;
        return await o.TerminerAsync(projetId, new List<MessageDto>(), a);
    }

    /// <summary>Nom lisible d'un code d'axe (vide : nom par défaut).</summary>
    public async Task<ResultatSaisieDto> NommerAxeAsync(Guid projetId, string code, NomAxeCommande c, CancellationToken a)
    {
        var u = await o.ExigerChefAsync(projetId, a);
        var nom = SaisieOutils.Nettoyer(c.Nom);
        var axe = await persistence.ChargerAxeAsync(projetId, code, a);
        if (axe is null)
        {
            var ordre = await persistence.DernierOrdreAxeAsync(projetId, a);
            axe = new Axe { ProjetId = projetId, Code = code, Ordre = ordre + 1 };
            persistence.AjouterAxe(axe);
        }
        var ancien = axe.Nom;
        axe.Nom = nom;
        o.Noter(projetId, u, "Axe", axe.Id, $"nom de l’axe {code}", ancien, nom);
        return await o.TerminerAsync(projetId, new List<MessageDto>(), a);
    }

    /// <summary>Client, projet ou référence (fiche du projet).</summary>
    public async Task<ResultatSaisieDto> DefinirFicheAsync(Guid projetId, FicheCommande c, CancellationToken a)
    {
        var u = await o.ExigerChefAsync(projetId, a);
        var p = await ProjetAsync(projetId, a);
        SaisieOutils.VerifierVersion(p, c.Version, "Le projet");
        var v = SaisieOutils.Nettoyer(c.Valeur);
        string? ancien;
        switch (c.Champ)
        {
            case "client": ancien = p.Client; p.Client = v; break;
            case "projet": ancien = p.Nom; p.Nom = v; break;
            case "reference": ancien = p.Reference; p.Reference = v.Length == 0 ? null : v; break;
            default: throw new RegleMetierException($"Champ de la fiche inconnu : « {c.Champ} ».");
        }
        // le client, le nom ou la référence forment l'identité du forfait : elle ne doit pas rejoindre celle d'un autre projet
        await unicite.ExigerLibreAsync(p.Reference, p.Client, p.Nom, p.Id, a);
        p.CalculerCleUnicite();
        var champ = await persistence.ChargerChampFicheAsync(projetId, c.Champ, a);
        if (champ is not null) champ.Valeur = v;
        o.Noter(projetId, u, "Projet", p.Id, $"fiche · {c.Champ}", ancien, v);
        return await o.TerminerAsync(projetId, new List<MessageDto>(), a);
    }

    // ------------------------------------------------------------------ types de tâches

    public async Task<ResultatSaisieDto> AjouterTypeAsync(Guid projetId, TypeCommande c, CancellationToken a)
    {
        var u = await o.ExigerChefAsync(projetId, a);
        var nom = SaisieOutils.Nettoyer(c.Nom);
        if (nom.Length == 0) throw new RegleMetierException("Saisissez le nom du type à ajouter.");
        var types = (await persistence.ListerTypesAsync(projetId, a)).ToList();
        if (types.Any(x => string.Equals(x.Nom, nom, StringComparison.OrdinalIgnoreCase))) throw new RegleMetierException("Ce type existe déjà.");
        var type = new TypeTache
        {
            ProjetId = projetId, Code = "T" + Guid.NewGuid().ToString("N")[..10], Nom = nom, Socle = false,
            Ordre = types.Count == 0 ? 0 : types.Max(x => x.Ordre) + 1,
        };
        persistence.AjouterType(type);
        o.Noter(projetId, u, "Type de tâche", type.Id, "ajout", null, nom);
        return await o.TerminerAsync(projetId, new List<MessageDto>
        {
            new($"Type « {nom} » ajouté : affectez-le aux tâches dans Workload › Liste des tâches ou depuis leur panneau.", "succes"),
        }, a);
    }

    /// <summary>Renomme un type (les tâches gardent l'identifiant du type : le renommage s'applique partout). Types socle : non modifiables.</summary>
    public async Task<ResultatSaisieDto> RenommerTypeAsync(Guid projetId, string code, TypeCommande c, CancellationToken a)
    {
        var u = await o.ExigerChefAsync(projetId, a);
        var types = (await persistence.ListerTypesAsync(projetId, a)).ToList();
        var type = types.FirstOrDefault(x => x.Code == code) ?? throw new IntrouvableException("Type de tâche introuvable.");
        if (type.Socle) throw new RegleMetierException("Type socle : non modifiable.");
        var nom = SaisieOutils.Nettoyer(c.Nom);
        if (nom.Length == 0) throw new RegleMetierException("Le nom du type ne peut pas être vide.");
        if (types.Any(x => x != type && string.Equals(x.Nom, nom, StringComparison.OrdinalIgnoreCase))) throw new RegleMetierException("Ce type existe déjà.");
        var ancien = type.Nom;
        type.Nom = nom;
        o.Noter(projetId, u, "Type de tâche", type.Id, "nom", ancien, nom);
        return await o.TerminerAsync(projetId, new List<MessageDto>(), a);
    }

    /// <summary>Supprime un type ajouté : les tâches qui l'utilisaient n'ont plus de type. Types socle : non supprimables.</summary>
    public async Task<ResultatSaisieDto> SupprimerTypeAsync(Guid projetId, string code, CancellationToken a)
    {
        var u = await o.ExigerChefAsync(projetId, a);
        var type = await persistence.ChargerTypeAsync(projetId, code, a)
            ?? throw new IntrouvableException("Type de tâche introuvable.");
        if (type.Socle) throw new RegleMetierException("Type socle : non supprimable.");
        foreach (var t in await persistence.ChargerTachesParTypeAsync(type.Id, a))
        {
            t.TypeTacheId = null;
            t.TypeSaisi = true;
        }
        persistence.SupprimerType(type);
        o.Noter(projetId, u, "Type de tâche", type.Id, "suppression", type.Nom, null);
        return await o.TerminerAsync(projetId, new List<MessageDto> { new($"Type « {type.Nom} » supprimé.", "succes") }, a);
    }

    private async Task<Projet> ProjetAsync(Guid projetId, CancellationToken a) =>
        await persistence.ChargerProjetAsync(projetId, a) ?? throw new IntrouvableException("Projet introuvable.");
}
