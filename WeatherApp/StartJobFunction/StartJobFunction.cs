using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StartJobFunction;

public class StartJobFunction
{
    private readonly ILogger<StartJobFunction> _logger;

    public StartJobFunction(ILogger<StartJobFunction> logger)
    {
        _logger = logger;
    }

    [Function("StartJobFunction")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
        
    }

}