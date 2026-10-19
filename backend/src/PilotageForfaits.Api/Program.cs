using System.Text.Json.Serialization;
using Franz.Common.Business.Extensions;
using Franz.Common.EntityFramework.Auditing;
using Franz.Common.EntityFramework.Extensions;
using Franz.Common.Http.Bootstrap.Extensions;
using Franz.Common.Http.EntityFramework.Extensions;
using Franz.Common.Logging.Extensions;
using Franz.Common.Mediator.Bootstrap;
using Franz.Common.Mediator.Extensions;
using Franz.Common.Mediator.Polly;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using PilotageForfaits.Api.Authentification;
using PilotageForfaits.Api.Erreurs;
using PilotageForfaits.Application;
using PilotageForfaits.Application.Abstractions;
using PilotageForfaits.Application.Projets;
using PilotageForfaits.Application.Securite;
using PilotageForfaits.Domain.Entites;
using PilotageForfaits.Infrastructure;
using PilotageForfaits.Infrastructure.Donnees;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;
var config = builder.Configuration;

builder.Host.UseLog();

builder.Services.AjouterApplication();
builder.Services.AddBusiness(typeof(Projet).Assembly);

builder.Services.AddRelationalDatabase<PilotageDbContext>(env, config);
builder.Services.AddFranzAuditing();
builder.Services.AjouterInfrastructure();

builder.Services.AddFranzMediatorV2Default(null, typeof(ArchiverProjetCommandHandler).Assembly)
    .AddFranzSerilogAuditPipeline()
    .AddFranzSerilogLoggingPipeline();
builder.Services.AddFranzResilience(config);
builder.Services.AddHttpArchitecture(env, config);

builder.Services.Configure<GroupesEntraConfiguration>(config.GetSection("Authentification:GroupesEntra"));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUtilisateurCourant, UtilisateurCourantHttp>();

var mode = config["Authentification:Mode"] ?? "EntraId";
if (mode == "Dev")
{
    if (!env.IsDevelopment() && !env.IsEnvironment("Tests"))
        throw new InvalidOperationException("L'authentification de développement est interdite hors environnement de développement.");

    builder.Services.AddAuthentication(AuthentificationDev.Schema)
        .AddScheme<OptionsAuthentificationDev, AuthentificationDev>(
            AuthentificationDev.Schema,
            options => config.GetSection("Authentification:UtilisateursDev").Bind(options.Utilisateurs));
}
else
{
    // Entra ID is the only supported non-development authentication provider.
    // Fail at startup rather than running with template values that can never validate a real token.
    if (!env.IsEnvironment("Tests"))
    {
        var tenantId = config["AzureAd:TenantId"];
        var clientId = config["AzureAd:ClientId"];
        var audience = config["AzureAd:Audience"];
        if (string.IsNullOrWhiteSpace(tenantId) || tenantId.Contains("RENSEIGNER", StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(clientId) || clientId.Contains("RENSEIGNER", StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(audience) || audience.Contains("RENSEIGNER", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "La configuration Entra ID est incomplète. Configurez AzureAd:TenantId, AzureAd:ClientId et AzureAd:Audience via la configuration de déploiement.");
        }
    }

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApi(config.GetSection("AzureAd"));
}
builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GestionnaireErreurs>();
builder.Services.AddOpenApi();

var app = builder.Build();
app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

app.UseHttpArchitecture();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseAuthentication();
app.UseAuthorization();

if (env.IsDevelopment())
{
    app.MapOpenApi();

    await using var scope = app.Services.CreateAsyncScope();
    var initializer = scope.ServiceProvider.GetRequiredService<IInitialiseurBaseDeDeveloppement>();
    await initializer.InitialiserAsync();
}

app.MapControllers();
app.MapGet("/api/v1/sante", () => Results.Ok(new { statut = "ok" })).AllowAnonymous();

app.Run();

/// <summary>Rend la classe Program visible des tests d'intégration.</summary>
public partial class Program;
