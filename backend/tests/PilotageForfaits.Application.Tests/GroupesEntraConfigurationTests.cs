using PilotageForfaits.Application.Securite;
using PilotageForfaits.Domain.Entites;

namespace PilotageForfaits.Application.Tests;

public sealed class GroupesEntraConfigurationTests
{
    [Fact]
    public void Un_groupe_entra_est_converti_en_role_de_vue_configure()
    {
        var configuration = new GroupesEntraConfiguration
        {
            Consultant = ["11111111-1111-1111-1111-111111111111"],
            ChefDeProjet = ["22222222-2222-2222-2222-222222222222"]
        };

        var roles = configuration.RolesPour(["22222222-2222-2222-2222-222222222222"]);

        Assert.Equal(new[] { RoleProjet.ChefDeProjet }, roles);
    }

    [Fact]
    public void La_comparaison_des_identifiants_de_groupe_est_insensible_a_la_casse()
    {
        var configuration = new GroupesEntraConfiguration
        {
            Consultant = ["ABCDEFAB-CDEF-ABCD-EFAB-CDEFABCDEFAB"]
        };

        var roles = configuration.RolesPour(["abcdefab-cdef-abcd-efab-cdefabcdefab"]);

        Assert.Equal(new[] { RoleProjet.Consultant }, roles);
    }

    [Fact]
    public void Un_groupe_non_configure_ne_donne_aucun_role()
    {
        var configuration = new GroupesEntraConfiguration
        {
            ChefDeProjet = ["known-group-id"]
        };

        Assert.Empty(configuration.RolesPour(["unknown-group-id"]));
        Assert.False(configuration.EstAdministrateur(["unknown-group-id"]));
    }

    [Fact]
    public void Plusieurs_groupes_de_vue_configures_sont_signales_comme_ambigus()
    {
        var configuration = new GroupesEntraConfiguration
        {
            Consultant = ["group-consultant"],
            ChefDeProjet = ["group-chef"]
        };

        var roles = configuration.RolesPour(["group-consultant", "group-chef"]);

        Assert.Equal(2, roles.Count);
        Assert.Contains(RoleProjet.Consultant, roles);
        Assert.Contains(RoleProjet.ChefDeProjet, roles);
    }

    [Fact]
    public void Le_groupe_administrateur_ne_donne_pas_a_lui_seul_un_role_de_projet()
    {
        var configuration = new GroupesEntraConfiguration
        {
            Administrateur = ["group-admin"]
        };

        Assert.True(configuration.EstAdministrateur(["group-admin"]));
        Assert.Empty(configuration.RolesPour(["group-admin"]));
    }
}
