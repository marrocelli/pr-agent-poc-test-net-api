
using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace PrAgentWebhook;

public class PrWebhook
{
    private readonly ILogger _logger;

    public PrWebhook(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<PrWebhook>();
    }

    [Function("PrWebhook")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        _logger.LogInformation("PR webhook received at {Time}", DateTime.UtcNow);

        var body = await req.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(body))
        {
            _logger.LogWarning("Empty request body.");
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Empty body.");
            return badResponse;
        }

        // GitHub always sends JSON
        JsonDocument doc;
        try
        {
            doc = JsonDocument.Parse(body);
        }
        catch (JsonException)
        {
            _logger.LogWarning("Invalid JSON payload.");
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Invalid JSON.");
            return badResponse;
        }

        using (doc)
        {
            var root = doc.RootElement;

            // GitHub PR webhooks have "action" and "pull_request"
            if (!root.TryGetProperty("action", out var actionProp) ||
                !root.TryGetProperty("pull_request", out var prProp))
            {
                _logger.LogWarning("Not a PR event.");
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Not a pull_request event.");
                return badResponse;
            }

            var action = actionProp.GetString() ?? "unknown";

            // Extract repo + PR number
            var number = prProp.GetProperty("number").GetInt32();

            string repoFullName = "";
            if (root.TryGetProperty("repository", out var repoProp) &&
                repoProp.TryGetProperty("full_name", out var fullNameProp))
            {
                repoFullName = fullNameProp.GetString() ?? "";
            }

            _logger.LogInformation(
                "PR event: action={Action}, pr={Number}, repo={Repo}",
                action, number, repoFullName
            );

            // For now, just echo a simple message.
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync(
                $"Received PR event: action={action}, pr={number}, repo={repoFullName}"
            );
            return response;
        }
    }
}
