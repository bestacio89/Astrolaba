using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using PilotageForfaits.Client.Http;
using PilotageForfaits.Client.Http.Clients;
using PilotageForfaits.Contracts.Projets;
using Xunit;

namespace PilotageForfaits.Client.Http.Tests;

public sealed class ApiClientTests
{
    [Fact]
    public async Task Projects_client_uses_expected_route_and_deserializes_empty_list()
    {
        string? requestedUri = null;
        using var http = CreateClient((request, _) =>
        {
            requestedUri = request.RequestUri?.ToString();
            return Task.FromResult(JsonResponse(HttpStatusCode.OK, "[]"));
        });

        var result = await new ProjetsClient(http).ListerAsync(archives: true);

        Assert.Empty(result);
        Assert.EndsWith("api/projets?archives=true", requestedUri, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Projects_client_sends_archiving_command_as_json()
    {
        HttpMethod? method = null;
        string? requestedUri = null;
        string? requestBody = null;
        using var http = CreateClient(async (request, cancellationToken) =>
        {
            method = request.Method;
            requestedUri = request.RequestUri?.ToString();
            requestBody = request.Content is null
                ? null
                : await request.Content.ReadAsStringAsync(cancellationToken);
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        });

        var id = Guid.NewGuid();
        await new ProjetsClient(http).ArchiverAsync(id, new ArchivageCommande(true));

        Assert.Equal(HttpMethod.Put, method);
        Assert.EndsWith($"api/projets/{id:D}/archivage", requestedUri, StringComparison.Ordinal);
        using var json = JsonDocument.Parse(requestBody!);
        Assert.True(json.RootElement.GetProperty("archive").GetBoolean());
    }

    [Fact]
    public async Task Api_errors_preserve_status_and_problem_details_body()
    {
        const string problem = """{"title":"Modification simultanée","status":409,"detail":"Conflict"}""";
        using var http = CreateClient((_, _) =>
            Task.FromResult(JsonResponse(HttpStatusCode.Conflict, problem)));

        var exception = await Assert.ThrowsAsync<ApiClientException>(() =>
            new ProjetsClient(http).ObtenirAsync(Guid.NewGuid()));

        Assert.Equal(HttpStatusCode.Conflict, exception.StatusCode);
        Assert.Contains("Modification simultanée", exception.ResponseBody, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Draft_export_preserves_file_name_content_and_media_type()
    {
        using var http = CreateClient((_, _) =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(Encoding.UTF8.GetBytes("workbook"))
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "\"coproj.xlsx\""
            };
            return Task.FromResult(response);
        });

        var file = await new BrouillonsClient(http).ExporterAsync(Guid.NewGuid());

        Assert.Equal("coproj.xlsx", file.FileName);
        Assert.Equal("workbook", Encoding.UTF8.GetString(file.Content));
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file.ContentType);
    }

    private static HttpClient CreateClient(
        Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> send) =>
        new(new StubHandler(send))
        {
            BaseAddress = new Uri("https://astrolaba.example.test/")
        };

    private static HttpResponseMessage JsonResponse(HttpStatusCode status, string json) =>
        new(status)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

    private sealed class StubHandler(
        Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> send) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) =>
            send(request, cancellationToken);
    }
}
