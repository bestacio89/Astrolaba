using PilotageForfaits.Contracts.Moi;

namespace PilotageForfaits.Client.Http.Clients;

public interface IMoiClient
{
    Task<MoiDto> ObtenirAsync(CancellationToken cancellationToken = default);
}
