namespace PilotageForfaits.Client.Http.Clients;

public sealed class SanteClient(HttpClient httpClient) : ApiClientBase(httpClient), ISanteClient
{
    public Task<SanteDto> ObtenirAsync(CancellationToken cancellationToken = default) =>
        GetAsync<SanteDto>("api/sante", cancellationToken);
}
