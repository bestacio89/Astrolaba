using System.Text.Json.Serialization;
using Franz.Common.Mediator.Extensions;
using Franz.Common.EntityFramework.Auditing;
using Franz.Common.Http.Bootstrap.Extensions;
using Franz.Common.Logging.Extensions;
using Franz.Common.Mediator.Polly;
using PilotageForfaits.Application.Projets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using PilotageForfaits.Api.Authentification;
using PilotageForfaits.Api.Erreurs;
using PilotageForfaits.Application;
using PilotageForfaits.Application.Abstractions;
using PilotageForfaits.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseLog();
builder.Services.AddFranzTelemetry(builder.Environment, builder.Configuration);

// ---------- Couches de l'application ----------
builder.Services.AjouterApplication();
builder.Services.AddFranzMediatorV2(null, typeof(ArchiverProjetCommandHandler).Assembly)
    .AddFranzSerilogAuditPipeline()
    .AddFranzSerilogLoggingPipeline();
builder.Services.AjouterInfrastructure(builder.Configuration);
builder.Services.AddFranzAuditing();
builder.Services.AddFranzResilience(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUtilisateurCourant, UtilisateurCourantHttp>();

// ---------- Authentification : compte d'entreprise (Entra ID), ou utilisateurs fictifs en développement ----------
var mode = builder.Configuration["Authentification:Mode"] ?? "EntraId";
if (mode == "Dev")
{
    if (!builder.Environment.IsDevelopment() && !builder.Environment.IsEnvironment("Tests"))
        throw new InvalidOperationException("L'authentification de développement est interdite hors environnement de développement.");
    builder.Services.AddAuthentication(AuthentificationDev.Schema)
        .AddScheme<OptionsAuthentificationDev, AuthentificationDev>(AuthentificationDev.Schema,
            o => builder.Configuration.GetSection("Authentification:UtilisateursDev").Bind(o.Utilisateurs));
}
else
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
}
builder.Services.AddAuthorization();

// ---------- API REST ----------
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GestionnaireErreurs>();
builder.Services.AddOpenApi(); // description de l'API : /openapi/v1.json (sert à générer le client Angular)

var app = builder.Build();
app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

app.UseExceptionHandler();
app.UseStatusCodePages();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    await using var scope = app.Services.CreateAsyncScope();
    var initialiseur = scope.ServiceProvider.GetRequiredService<IInitialiseurBaseDeDeveloppement>();
    await initialiseur.InitialiserAsync();
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/api/sante", () => Results.Ok(new { statut = "ok" })).AllowAnonymous();

app.Run();

/// <summary>Rend la classe Program visible des tests d'intégration.</summary>
public partial class Program;
