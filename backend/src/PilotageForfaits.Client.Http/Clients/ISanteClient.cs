namespace PilotageForfaits.Client.Http.Clients;

public interface ISanteClient
{
    Task<SanteDto> ObtenirAsync(CancellationToken cancellationToken = default);
}

public sealed record SanteDto(string Statut);
