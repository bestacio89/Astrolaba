using Franz.Common.EntityFramework;
using Franz.Common.EntityFramework.Auditing;
using Franz.Common.EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PilotageForfaits.Application.Abstractions;
using PilotageForfaits.Application.Calcul;
using PilotageForfaits.Application.Brouillons;
using PilotageForfaits.Application.Exports;
using PilotageForfaits.Application.Projets;
using PilotageForfaits.Application.Suivi;
using PilotageForfaits.Infrastructure.Donnees;
using PilotageForfaits.Infrastructure.Excel;
using PilotageForfaits.Infrastructure.Persistence;

namespace PilotageForfaits.Infrastructure;

public static class DependencyInjection
{
    /// <summary>Déclare la base de données, la lecture des TSP, l'écriture des exports Excel et l'horloge.</summary>
    public static IServiceCollection AjouterInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var chaine = configuration.GetConnectionString("Pilotage")
            ?? throw new InvalidOperationException("Chaîne de connexion « ConnectionStrings:Pilotage » absente de la configuration.");
        services.AddDbContext<PilotageDbContext>(o => o.UseSqlServer(chaine, sql => sql.EnableRetryOnFailure()));
        services.AddScoped<IInitialiseurBaseDeDeveloppement, InitialiseurBaseDeDeveloppement>();
        services.AddUnitOfWork<PilotageDbContext>();
        services.AddScoped<DbContextBase>(sp => sp.GetRequiredService<PilotageDbContext>());
        services.AddEntityRepositories<PilotageDbContext>();
        services.AddBehaviors();
        services.AddScoped<IContexteDonnees>(sp => sp.GetRequiredService<PilotageDbContext>());
        services.AddScoped<ICurrentUserService, AstrolabaCurrentUserService>();
        services.AddScoped<IRechercheIdentiteProjet, RechercheIdentiteProjet>();
        services.AddScoped<IJournalWriter, JournalWriter>();
        services.AddScoped<IAccesRepository, AccesRepository>();
        services.AddScoped<IChargeurDonneesProjet, ChargeurDonneesProjet>();
        services.AddScoped<IProjetsPersistence, ProjetsPersistence>();
        services.AddScoped<IBrouillonsPersistence, BrouillonsPersistence>();
        services.AddScoped<ISuiviPersistence, SuiviPersistence>();
        services.AddScoped<IExportsPersistence, ExportsPersistence>();
        services.AddScoped<IAteliersLivrablesPersistence, AteliersLivrablesPersistence>();
        services.AddScoped<IDepensesPersistence, DepensesPersistence>();
        services.AddScoped<IFacturationPersistence, FacturationPersistence>();
        services.AddScoped<IPointsHebdoPersistence, PointsHebdoPersistence>();
        services.AddScoped<IReferencesPersistence, ReferencesPersistence>();
        services.AddScoped<IRegistresPersistence, RegistresPersistence>();
        services.AddScoped<IParametresPersistence, ParametresPersistence>();
        services.AddSingleton<IHorloge, HorlogeSysteme>();
        services.AddScoped<ILecteurTsp, LecteurTsp>();
        services.AddSingleton<IEcrivainClasseur, EcrivainXlsx>();
        return services;
    }
}
