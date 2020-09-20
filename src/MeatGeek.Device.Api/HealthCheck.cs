using System;
using System.Collections.Generic;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Inferno.Functions
{
    public static class HealthCheck
    {
        [FunctionName("healthcheck")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("Performing health check on the Cosmos DB processing Function App.");

            // TODO: This is a very simple health check that ensures each configuration setting exists and has a value.
            // More thorough checks would validate each value against an expected format or by connecting to each service as required.
            // The function will return an HTTP status of 200 (OK) if all values contain non-zero strings.
            // If any are null or empty, the function will return an error, indicating which values are missing.

            var RelayNamespace = Environment.GetEnvironmentVariable("RelayNamespace", EnvironmentVariableTarget.Process);
            var RelayConnectionName = Environment.GetEnvironmentVariable("RelayConnectionName", EnvironmentVariableTarget.Process);
            var RelayKeyName = Environment.GetEnvironmentVariable("RelayKeyName", EnvironmentVariableTarget.Process);
            var RelayKey = Environment.GetEnvironmentVariable("RelayKey", EnvironmentVariableTarget.Process);

            var variableList = new List<string>();
            if (string.IsNullOrWhiteSpace(RelayNamespace)) variableList.Add("RelayNamespace");
            if (string.IsNullOrWhiteSpace(RelayConnectionName)) variableList.Add("RelayConnectionName");
            if (string.IsNullOrWhiteSpace(RelayKeyName)) variableList.Add("RelayKeyName");
            if (string.IsNullOrWhiteSpace(RelayKey)) variableList.Add("RelayKey");

            if (variableList.Count > 0)
            {
                return new BadRequestObjectResult($"The service is missing one or more application settings: {string.Join(", ", variableList)}");
            }

            return new OkObjectResult($"The service contains expected application settings");
        }
    }
}
