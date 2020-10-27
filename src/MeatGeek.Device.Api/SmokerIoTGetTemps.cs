using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

using Microsoft.Azure.Devices;

namespace Inferno.Functions
{

    public static class SmokerIoTGetTemps
    {
        private static ServiceClient IoTHubServiceClient;
        private static string ServiceConnectionString;

        [FunctionName("smokergettemps")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            ServiceConnectionString = Environment.GetEnvironmentVariable("InfernoIoTServiceConnection", EnvironmentVariableTarget.Process);
            IoTHubServiceClient = ServiceClient.CreateFromConnectionString(ServiceConnectionString);
            var methodInvocation = new CloudToDeviceMethod("SmokerGetTemps") { ResponseTimeout = TimeSpan.FromSeconds(30) };
            // Invoke the direct method asynchronously and get the response from the device.
            var response = await IoTHubServiceClient.InvokeDeviceMethodAsync("inferno1", methodInvocation);

            Console.WriteLine("Response status: {0}, payload:", response.Status);
            Console.WriteLine(response.GetPayloadAsJson());

            log.LogInformation("Response status: {0}, payload:", response.Status);
            log.LogInformation(response.GetPayloadAsJson());
            return new ObjectResult(response);
        }
    }
}


