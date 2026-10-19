using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PilotageForfaits.Api.Authentification;
using PilotageForfaits.Application.Abstractions;
using PilotageForfaits.Infrastructure.Donnees;

namespace PilotageForfaits.Api.Tests;

/// <summary>
/// Héberge l'API complète avec les services Franz réels, une base SQLite isolée pour les tests HTTP,
/// des identités de développement déterministes et une horloge fixe.
/// La configuration est injectée dans l'hôte de test : aucun état d'environnement global partagé entre tests.
/// </summary>
public sealed class FabriqueApi : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connexion = new("DataSource=:memory:");

    private static readonly Dictionary<string, string?> ConfigurationTests = CreerConfigurationTests();

    public FabriqueApi()
    {
        // La base SQLite en mémoire vit tant que la connexion reste ouverte.
        _connexion.Open();
    }

    private static Dictionary<string, string?> CreerConfigurationTests()
    {
        var configuration = new Dictionary<string, string?>
        {
            ["Authentification:Mode"] = "Dev",
            // Franz configure la base relationnelle au démarrage. Le DbContext est ensuite remplacé
            // dans ConfigureTestServices par SQLite, sans dépendance à PostgreSQL pour les tests HTTP.
            ["ASTROLABA_POSTGRES"] = "Host=localhost;Database=unused;Username=postgres;Password=unused",
            ["ConnectionStrings:Pilotage"] = "Host=localhost;Database=unused;Username=postgres;Password=unused",
            ["Database:Provider"] = "Postgres",
            ["Database:ServerName"] = "localhost",
            ["Database:DatabaseName"] = "unused",
            ["Database:UserName"] = "postgres",
            ["Database:Password"] = "unused",
            ["Database:Port"] = "5432",
            ["Database:Ssl"] = "Disable",
            ["Authentification:GroupesEntra:Consultant:0"] = "dev-group-consultant",
            ["Authentification:GroupesEntra:ChefDeProjet:0"] = "dev-group-chef-de-projet",
            ["Authentification:GroupesEntra:Pilotage:0"] = "dev-group-pilotage"
        };

        var utilisateurs = CreerUtilisateursTests();
        for (var i = 0; i < utilisateurs.Count; i++)
        {
            var utilisateur = utilisateurs[i];
            var prefixe = $"Authentification:UtilisateursDev:{i}:";
            configuration[prefixe + "Cle"] = utilisateur.Cle;
            configuration[prefixe + "Oid"] = utilisateur.Oid;
            configuration[prefixe + "Prenom"] = utilisateur.Prenom;
            configuration[prefixe + "Nom"] = utilisateur.Nom;
            configuration[prefixe + "Email"] = utilisateur.Email;
            for (var j = 0; j < utilisateur.Groupes.Count; j++)
                configuration[$"{prefixe}Roles:{j}"] = utilisateur.Groupes[j];
        }

        return configuration;
    }

    private static List<UtilisateurDev> CreerUtilisateursTests() =>
    [
        new() { Cle = "chef", Oid = "oid-chef", Prenom = "Camille", Nom = "Martin", Email = "chef@exemple.fr", Groupes = ["dev-group-chef-de-projet"] },
        new() { Cle = "chef2", Oid = "oid-chef-2", Prenom = "Alex", Nom = "Durand", Email = "chef2@exemple.fr", Roles = ["ChefDeProjet"] },
        new() { Cle = "consultant", Oid = "oid-consultant", Prenom = "Sam", Nom = "Petit", Email = "consultant@exemple.fr", Groupes = ["dev-group-consultant"] },
        new() { Cle = "pilotage", Oid = "oid-pilotage", Prenom = "Lou", Nom = "Bernard", Email = "pilotage@exemple.fr", Groupes = ["dev-group-pilotage"] }
    ];

    /// <summary>
    /// Date du jour fixe (06/10/2026). L'heure peut être réglée par un test pour dater des saisies.
    /// </summary>
    public sealed class HorlogeFixe : IHorloge
    {
        public static readonly DateTimeOffset ParDefaut = new(2026, 10, 6, 10, 0, 0, TimeSpan.Zero);
        public DateTimeOffset Maintenant { get; set; } = ParDefaut;
        public DateOnly Aujourdhui => new(2026, 10, 6);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Tests");
        builder.ConfigureAppConfiguration((_, configuration) =>
            configuration.AddInMemoryCollection(ConfigurationTests));

        builder.ConfigureTestServices(services =>
        {
            // Configure explicit defaults and the real development scheme used by Program.cs.
            // This avoids relying on configuration binding order for the test identity catalogue.
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = AuthentificationDev.Schema;
                options.DefaultAuthenticateScheme = AuthentificationDev.Schema;
                options.DefaultChallengeScheme = AuthentificationDev.Schema;
                options.DefaultForbidScheme = AuthentificationDev.Schema;
            });
            services.PostConfigure<OptionsAuthentificationDev>(options =>
                options.Utilisateurs = CreerUtilisateursTests());

            // Franz reste dans le pipeline HTTP et le conteneur applicatif ; seul le provider de données
            // est remplacé pour que les tests soient rapides, déterministes et indépendants de PostgreSQL.
            services.RemoveAll<DbContextOptions<PilotageDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<PilotageDbContext>>();
            services.AddDbContext<PilotageDbContext>(options => options.UseSqlite(_connexion));

            services.RemoveAll<IHorloge>();
            services.AddSingleton<IHorloge, HorlogeFixe>();
        });
    }

    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);
        client.DefaultRequestHeaders.Add(AuthentificationDev.EnTete, "chef");
    }

    /// <summary>Client HTTP connecté sous une identité de test.</summary>
    public HttpClient ClientPour(string utilisateur)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Remove(AuthentificationDev.EnTete);
        client.DefaultRequestHeaders.Add(AuthentificationDev.EnTete, utilisateur);
        return client;
    }

    /// <summary>Client HTTP sans identité, pour vérifier le challenge d'authentification.</summary>
    public HttpClient ClientAnonyme()
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Remove(AuthentificationDev.EnTete);
        return client;
    }

    /// <summary>Recrée la base SQLite pour isoler chaque test de l'état laissé par les précédents.</summary>
    public void PreparerBase()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PilotageDbContext>();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
            _connexion.Dispose();
    }
}
