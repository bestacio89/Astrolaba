using PilotageForfaits.Domain.Entites;

namespace PilotageForfaits.Application.Securite;

/// <summary>
/// Maps immutable Entra group object IDs to Astrolaba's existing project-view roles.
/// Group IDs are supplied by deployment configuration; display names are not authorization inputs.
/// </summary>
public sealed class GroupesEntraConfiguration
{
    public List<string> Consultant { get; set; } = [];
    public List<string> ChefDeProjet { get; set; } = [];
    public List<string> Pilotage { get; set; } = [];
    public List<string> Lecteur { get; set; } = [];
    public List<string> Administrateur { get; set; } = [];

    public IReadOnlyList<RoleProjet> RolesPour(IEnumerable<string> groupesUtilisateur)
    {
        ArgumentNullException.ThrowIfNull(groupesUtilisateur);
        var groupes = groupesUtilisateur.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var roles = new List<RoleProjet>();

        if (Correspond(Consultant, groupes)) roles.Add(RoleProjet.Consultant);
        if (Correspond(ChefDeProjet, groupes)) roles.Add(RoleProjet.ChefDeProjet);
        if (Correspond(Pilotage, groupes)) roles.Add(RoleProjet.Pilotage);
        if (Correspond(Lecteur, groupes)) roles.Add(RoleProjet.Lecteur);

        return roles;
    }

    public bool EstAdministrateur(IEnumerable<string> groupesUtilisateur)
    {
        ArgumentNullException.ThrowIfNull(groupesUtilisateur);
        return Correspond(Administrateur, groupesUtilisateur.ToHashSet(StringComparer.OrdinalIgnoreCase));
    }

    private static bool Correspond(IEnumerable<string> groupesAutorises, HashSet<string> groupesUtilisateur) =>
        groupesAutorises.Any(groupe =>
            !string.IsNullOrWhiteSpace(groupe) && groupesUtilisateur.Contains(groupe));
}
