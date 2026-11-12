using CAMT054.Process;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using static Grpc.Core.Metadata;

namespace CAMT054FunctionApp;

public class ProcessXmlFunction
{
    private readonly ILogger<ProcessXmlFunction> _logger;

    private readonly string _xsdPath;
    public ProcessXmlFunction(ILogger<ProcessXmlFunction> logger)
    {
        _logger = logger;
        _xsdPath = Path.Combine(AppContext.BaseDirectory, "camt.054.001.08.xsd");
    }

    [Function("ProcessData")]
    public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        try
        {
            string aesKey = Environment.GetEnvironmentVariable("AESKEY");
            string aesIv = Environment.GetEnvironmentVariable("AESIV");

            string xsd8Path = Path.Combine(AppContext.BaseDirectory, "camt.054.001.08.xsd");
            string xsd4Path = Path.Combine(AppContext.BaseDirectory, "camt.054.001.04.xsd");

            string json = await new StreamReader(req.Body).ReadToEndAsync();
            CAMT054.Process.ProcessXML processXml = new CAMT054.Process.ProcessXML(json, xsd4Path, xsd8Path,aesKey,aesIv);
            var result = processXml.FetchCompiledResult();
            var jsonresult = JsonSerializer.Serialize(result);
            jsonresult = Utils.EncodeToBase64(jsonresult, aesKey, aesIv);
            // 5. Return success
            // 🔹 Prepare the HTTP response
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");

            // Optional: Wrap result in standard JSON response
            var output = new
            {
                Status = "success",
                Count = result?.Count ?? 0,
                Data = jsonresult
            };

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await response.WriteStringAsync(JsonSerializer.Serialize(output, jsonOptions));
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching data from Business Central");
            var errorRes = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorRes.WriteStringAsync($"Error: {ex.Message}");
            return errorRes;
        }

    }
}