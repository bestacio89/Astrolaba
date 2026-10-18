using System.Text.Json.Nodes;
using PilotageForfaits.Application.Suivi;

namespace PilotageForfaits.Application.Tests;

/// <summary>Les règles de confidentialité financière sont testées avec le service qui les implémente.</summary>
public sealed class FiltreMontantsTests
{
    [Theory]
    [InlineData("Atelier décalé, budget déplacement 1 200 €", "Atelier décalé")]
    [InlineData("Décision : annulation, coût évité 1 200 €", "Décision : annulation")]
    [InlineData("Atelier 3 décalé 1 200 €", "Atelier 3 décalé [montant]")]
    [InlineData("Coût € 1 500 validé", "Coût [montant] validé")]
    [InlineData("1500 euros de frais", "[montant] de frais")]
    [InlineData("Réunion en Europe le 3", "Réunion en Europe le 3")]
    public void Les_montants_sont_retires_des_textes_libres(string texte, string attendu) =>
        Assert.Equal(attendu, FiltreMontants.SansMontant(texte));

    [Fact]
    public void Appliquer_retire_les_champs_financiers_a_tous_les_niveaux()
    {
        var document = JsonNode.Parse("""
            {
              "tjm": 650,
              "libelle": "atelier",
              "details": {
                "montant": 1200,
                "commentaire": "Atelier reporté, coût évité 1 200 €"
              },
              "synthese": {
                "ca": 5000
              }
            }
            """);

        FiltreMontants.Appliquer(document);

        Assert.NotNull(document);
        Assert.Null(document["tjm"]);
        Assert.Equal("atelier", document["libelle"]!.GetValue<string>());
        Assert.Null(document["details"]!["montant"]);
        Assert.Null(document["synthese"]);
        Assert.DoesNotContain("€", document["details"]!["commentaire"]!.GetValue<string>());
    }

    [Fact]
    public void Appliquer_retire_les_entrees_de_journal_financieres_et_conserve_les_autres()
    {
        var document = JsonNode.Parse("""
            {
              "journal": [
                { "objet": "Achat", "texte": "Commande fournisseur" },
                { "objet": "Projet", "texte": "Atelier déplacé" },
                { "objet": "Projet", "texte": "Budget 1 200 € validé" }
              ]
            }
            """);

        FiltreMontants.Appliquer(document);

        var journal = document!["journal"]!.AsArray();
        var entree = Assert.Single(journal);
        Assert.Equal("Projet", entree!["objet"]!.GetValue<string>());
        Assert.Equal("Atelier déplacé", entree["texte"]!.GetValue<string>());
    }
}
