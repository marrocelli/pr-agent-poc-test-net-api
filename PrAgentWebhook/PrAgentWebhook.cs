using System.Net;
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

        string body = await req.ReadAsStringAsync();
        _logger.LogInformation("Request body: {Body}", body);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteStringAsync("Webhook received successfully.");
        return response;
    }
}
