using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
            ["Database:Ssl"] = "Disable"
        };

        var utilisateurs = new[]
        {
            ("chef", "oid-chef", "Camille", "Martin", "ChefDeProjet"),
            ("chef2", "oid-chef-2", "Alex", "Durand", "ChefDeProjet"),
            ("consultant", "oid-consultant", "Sam", "Petit", ""),
            ("pilotage", "oid-pilotage", "Lou", "Bernard", "Pilotage"),
        };

        for (var i = 0; i < utilisateurs.Length; i++)
        {
            var (cle, oid, prenom, nom, role) = utilisateurs[i];
            var prefixe = $"Authentification:UtilisateursDev:{i}:";
            configuration[prefixe + "Cle"] = cle;
            configuration[prefixe + "Oid"] = oid;
            configuration[prefixe + "Prenom"] = prenom;
            configuration[prefixe + "Nom"] = nom;
            configuration[prefixe + "Email"] = $"{cle}@exemple.fr";
            if (role.Length > 0)
                configuration[prefixe + "Roles:0"] = role;
        }

        return configuration;
    }

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
        client.DefaultRequestHeaders.Add("X-Utilisateur-Dev", "chef");
    }

    /// <summary>Client HTTP connecté sous une identité de test.</summary>
    public HttpClient ClientPour(string utilisateur)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Remove("X-Utilisateur-Dev");
        client.DefaultRequestHeaders.Add("X-Utilisateur-Dev", utilisateur);
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
