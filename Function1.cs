using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public static class MyAzureFunction
{
    private static readonly HttpClient httpClient = new HttpClient();

    [FunctionName("HttpRequestMiddleware")]
    public static async Task<HttpResponseMessage> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "middleware")] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("Processing request in middleware function.");

        // 1. Extract data from the incoming request (headers, body, etc.)
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var requestHeaders = req.Headers;

        // Log incoming request details for debugging
        log.LogInformation($"Request Body: {requestBody}");
        log.LogInformation($"Request Headers: {requestHeaders}");

        // 2. Modify the request or perform actions as middleware (e.g., logging, validation, etc.)

        // Example: Forward the request to another API
        var forwardedResponse = await ForwardRequestToAnotherApi(requestBody);

        // 3. Return a response back to the client
        var responseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent(JsonConvert.SerializeObject(new
            {
                Status = "Success",
                ForwardedData = forwardedResponse
            }))
        };

        return responseMessage;
    }

    private static async Task<string> ForwardRequestToAnotherApi(string requestData)
    {
        // Example: Send the request data to another API (middleware forward)
        var forwardUrl = "https://example.com/api"; // Replace with actual URL

        var response = await httpClient.PostAsync(forwardUrl, new StringContent(requestData));
        return await response.Content.ReadAsStringAsync();
    }
}
