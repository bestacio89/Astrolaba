using PilotageForfaits.Contracts.Moi;

namespace PilotageForfaits.Client.Http.Clients;

public sealed class MoiClient(HttpClient httpClient) : ApiClientBase(httpClient), IMoiClient
{
    public Task<MoiDto> ObtenirAsync(CancellationToken cancellationToken = default) =>
        GetAsync<MoiDto>("api/moi", cancellationToken);
}
