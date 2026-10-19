using System.Net;
using System.Net.Http.Json;

namespace PilotageForfaits.Api.Tests;

/// <summary>Vérifie la frontière d'authentification réelle de l'API sans remplacer son handler.</summary>
public sealed class AuthentificationApiTests : IClassFixture<FabriqueApi>
{
    private readonly FabriqueApi _api;

    public AuthentificationApiTests(FabriqueApi api)
    {
        _api = api;
        _api.PreparerBase();
    }

    private static object ProjetValide() => new
    {
        client = "Client test",
        nom = "Projet d'authentification",
        reference = "AUTH-001",
        debut = "2026-11-02",
        finContractuelle = "2027-02-26",
        typeContrat = "Forfait"
    };

    [Fact]
    public async Task Une_requete_sans_identite_est_refusee_avec_401()
    {
        var reponse = await _api.ClientAnonyme()
            .PostAsJsonAsync("/api/v1/projets", ProjetValide());

        Assert.Equal(HttpStatusCode.Unauthorized, reponse.StatusCode);
    }

    [Fact]
    public async Task Une_cle_utilisateur_inconnue_est_refusee_avec_401()
    {
        var reponse = await _api.ClientPour("utilisateur-inconnu")
            .PostAsJsonAsync("/api/v1/projets", ProjetValide());

        Assert.Equal(HttpStatusCode.Unauthorized, reponse.StatusCode);
    }

    [Fact]
    public async Task Une_identite_authentifiee_et_habilitee_atteint_le_controleur()
    {
        var reponse = await _api.ClientPour("chef")
            .PostAsJsonAsync("/api/v1/projets", ProjetValide());

        Assert.Equal(HttpStatusCode.Created, reponse.StatusCode);
    }
}
