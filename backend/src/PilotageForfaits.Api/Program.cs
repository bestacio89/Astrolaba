using System.Text.Json.Serialization;
using Franz.Common.EntityFramework.Auditing;
using Franz.Common.EntityFramework.Extensions;
using Franz.Common.Http.Bootstrap.Extensions;
using Franz.Common.Http.EntityFramework.Extensions;
using Franz.Common.Logging.Extensions;
using Franz.Common.Mediator.Extensions;
using Franz.Common.Mediator.Polly;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using PilotageForfaits.Api.Authentification;
using PilotageForfaits.Api.Erreurs;
using PilotageForfaits.Application;
using PilotageForfaits.Application.Abstractions;
using PilotageForfaits.Application.Projets;
using PilotageForfaits.Infrastructure;
using PilotageForfaits.Infrastructure.Donnees;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;
var config = builder.Configuration;

// ---------- Logging ----------
builder.Host.UseLog();

// ---------- Application and persistence ----------
builder.Services.AjouterApplication();
builder.Services.AddRelationalDatabase<PilotageDbContext>(env, config);
builder.Services.AddFranzAuditing();
builder.Services.AjouterInfrastructure();
builder.Services.AddFranzMediatorV2(null, typeof(ArchiverProjetCommandHandler).Assembly)
    .AddFranzSerilogAuditPipeline()
    .AddFranzSerilogLoggingPipeline();
builder.Services.AddFranzResilience(config);

// ---------- HTTP architecture ----------
builder.Services.AddHttpArchitecture(env, config);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUtilisateurCourant, UtilisateurCourantHttp>();

// ---------- Authentication: Entra ID in hosted environments, dev identities locally ----------
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
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApi(config.GetSection("AzureAd"));
}
builder.Services.AddAuthorization();

// Preserve Astrolaba's API contract while using Franz's HTTP bootstrap.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GestionnaireErreurs>();
builder.Services.AddOpenApi();

var app = builder.Build();
app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

// Franz owns the shared HTTP middleware conventions; Astrolaba keeps its domain error handler and auth.
app.UseHttpArchitecture();
app.UseExceptionHandler();
app.UseStatusCodePages();

if (env.IsDevelopment())
{
    app.MapOpenApi();

    await using var scope = app.Services.CreateAsyncScope();
    var initializer = scope.ServiceProvider.GetRequiredService<IInitialiseurBaseDeDeveloppement>();
    await initializer.InitialiserAsync();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/api/sante", () => Results.Ok(new { statut = "ok" })).AllowAnonymous();

app.Run();

/// <summary>Rend la classe Program visible des tests d'intégration.</summary>
public partial class Program;
