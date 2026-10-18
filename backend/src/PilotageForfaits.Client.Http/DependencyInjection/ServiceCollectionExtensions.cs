using Microsoft.Extensions.DependencyInjection;
using PilotageForfaits.Client.Http.Clients;

namespace PilotageForfaits.Client.Http.DependencyInjection;

/// <summary>Registration helpers for consuming the Astrolaba HTTP API.</summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPilotageForfaitsClients(
        this IServiceCollection services,
        string baseUrl,
        Action<HttpClient>? configureHttpClient = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(baseUrl);

        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri) ||
            baseUri.Scheme is not ("http" or "https"))
        {
            throw new ArgumentException("The API base URL must be an absolute HTTP or HTTPS URL.", nameof(baseUrl));
        }

        var normalizedBaseUri = new Uri(baseUrl.EndsWith("/", StringComparison.Ordinal)
            ? baseUrl
            : baseUrl + "/", UriKind.Absolute);

        void Configure(HttpClient client)
        {
            client.BaseAddress = normalizedBaseUri;
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-Client", "PilotageForfaits.Client.Http");
            configureHttpClient?.Invoke(client);
        }

        services.AddHttpClient<IProjetsClient, ProjetsClient>(Configure);
        services.AddHttpClient<IBrouillonsClient, BrouillonsClient>(Configure);
        services.AddHttpClient<IMoiClient, MoiClient>(Configure);
        services.AddHttpClient<ISuiviClient, SuiviClient>(Configure);

        return services;
    }
}
