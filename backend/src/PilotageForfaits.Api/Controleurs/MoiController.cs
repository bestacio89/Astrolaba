using Franz.Common.Mediator.Dispatchers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PilotageForfaits.Contracts.Moi;

namespace PilotageForfaits.Api.Controleurs;

/// <summary>La personne connectée et ce qu'elle peut faire (sert à adapter l'interface).</summary>
[ApiController]
[Authorize]
[Route("api/moi")]
public sealed class MoiController(IDispatcher dispatcher) : ControllerBase
{
    [HttpGet]
    public Task<MoiDto> Obtenir(CancellationToken annulation) =>
        dispatcher.SendAsync(new ObtenirMoiQuery(), annulation);
}
