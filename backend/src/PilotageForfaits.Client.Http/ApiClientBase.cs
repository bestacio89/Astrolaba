using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PilotageForfaits.Client.Http;

/// <summary>Shared HTTP transport for typed Astrolaba API clients.</summary>
public abstract class ApiClientBase(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    protected HttpClient Client { get; } = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    protected async Task<TResponse> GetAsync<TResponse>(
        string relativeUri,
        CancellationToken cancellationToken = default) =>
        await SendForResponseAsync<TResponse>(HttpMethod.Get, relativeUri, null, cancellationToken);

    protected async Task<TResponse> SendForResponseAsync<TResponse>(
        HttpMethod method,
        string relativeUri,
        object? request = null,
        CancellationToken cancellationToken = default)
    {
        using var message = CreateRequest(method, relativeUri, request);
        using var response = await Client.SendAsync(
            message,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
            return default!;

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var result = await JsonSerializer.DeserializeAsync<TResponse>(stream, JsonOptions, cancellationToken);
        return result ?? throw new JsonException(
            $"The Astrolaba API returned an empty JSON response for {method} {relativeUri}.");
    }

    protected async Task SendWithoutResponseAsync(
        HttpMethod method,
        string relativeUri,
        object? request = null,
        CancellationToken cancellationToken = default)
    {
        using var message = CreateRequest(method, relativeUri, request);
        using var response = await Client.SendAsync(
            message,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);
    }

    protected async Task<TResponse> SendContentForResponseAsync<TResponse>(
        HttpMethod method,
        string relativeUri,
        HttpContent content,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(content);

        using var message = new HttpRequestMessage(method, relativeUri) { Content = content };
        using var response = await Client.SendAsync(
            message,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var result = await JsonSerializer.DeserializeAsync<TResponse>(stream, JsonOptions, cancellationToken);
        return result ?? throw new JsonException(
            $"The Astrolaba API returned an empty JSON response for {method} {relativeUri}.");
    }

    protected async Task<DownloadedFile> DownloadAsync(
        HttpMethod method,
        string relativeUri,
        object? request = null,
        CancellationToken cancellationToken = default)
    {
        using var message = CreateRequest(method, relativeUri, request);
        using var response = await Client.SendAsync(
            message,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);

        var disposition = response.Content.Headers.ContentDisposition;
        var fileName = disposition?.FileNameStar ?? disposition?.FileName;
        if (!string.IsNullOrWhiteSpace(fileName))
            fileName = fileName.Trim('"');

        return new DownloadedFile(
            await response.Content.ReadAsByteArrayAsync(cancellationToken),
            string.IsNullOrWhiteSpace(fileName) ? "download.bin" : fileName,
            response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream");
    }

    private static HttpRequestMessage CreateRequest(HttpMethod method, string relativeUri, object? request)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativeUri);

        var message = new HttpRequestMessage(method, relativeUri);
        if (request is not null)
        {
            message.Content = new StringContent(
                JsonSerializer.Serialize(request, JsonOptions),
                Encoding.UTF8,
                "application/json");
        }

        return message;
    }

    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
            return;

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        var detail = string.IsNullOrWhiteSpace(body) ? response.ReasonPhrase : body;
        throw new ApiClientException(
            $"Astrolaba API returned HTTP {(int)response.StatusCode} ({response.StatusCode}): {detail}",
            response.StatusCode,
            body);
    }

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }
}

/// <summary>Downloaded file content and response metadata.</summary>
public sealed record DownloadedFile(byte[] Content, string FileName, string ContentType);

/// <summary>HTTP failure returned by the Astrolaba API, preserving its ProblemDetails payload.</summary>
public sealed class ApiClientException(
    string message,
    HttpStatusCode statusCode,
    string responseBody) : HttpRequestException(message, null, statusCode)
{
    public string ResponseBody { get; } = responseBody;
}
