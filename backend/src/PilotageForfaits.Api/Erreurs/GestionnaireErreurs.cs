using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PilotageForfaits.Application.Commun;
using PilotageForfaits.Domain.Commun;

namespace PilotageForfaits.Api.Erreurs;

/// <summary>
/// Transforme les erreurs en réponses HTTP claires au format ProblemDetails.
/// Règle métier → 422, introuvable → 404, accès refusé → 403, conflit → 409,
/// requête invalide → 400/413, sinon 500 sans détail technique.
/// </summary>
public sealed class GestionnaireErreurs(ILogger<GestionnaireErreurs> journal) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext contexte, Exception exception, CancellationToken annulation)
    {
        if (exception is OperationCanceledException && contexte.RequestAborted.IsCancellationRequested)
            return true;

        var (statut, titre, detail) = exception switch
        {
            RegleMetierException e => (StatusCodes.Status422UnprocessableEntity, "Action impossible", e.Message),
            IntrouvableException e => (StatusCodes.Status404NotFound, "Introuvable", e.Message),
            AccesRefuseException e => (StatusCodes.Status403Forbidden, "Accès refusé", e.Message),
            ConflitException e => (StatusCodes.Status409Conflict, "Modification simultanée", e.Message),
            BadHttpRequestException e => (e.StatusCode, "Requête invalide", e.Message),
            _ => (StatusCodes.Status500InternalServerError, "Erreur inattendue", "Une erreur inattendue s'est produite. Elle a été enregistrée dans le journal du serveur."),
        };

        if (statut == StatusCodes.Status500InternalServerError)
            journal.LogError(exception, "Erreur non gérée");
        else if (exception is ConflitException)
            journal.LogWarning(exception, "Conflit de persistance");

        // Always write ProblemDetails, even when the client sends no Accept header.
        contexte.Response.StatusCode = statut;
        contexte.Response.ContentType = "application/problem+json";
        await contexte.Response.WriteAsJsonAsync(
            new ProblemDetails { Status = statut, Title = titre, Detail = detail, Instance = contexte.Request.Path },
            options: null,
            contentType: "application/problem+json",
            cancellationToken: annulation);

        return true;
    }
}
