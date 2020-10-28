using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

using Microsoft.Azure.Devices;

namespace Inferno.Functions
{

    public static class SetPoint
    {
        private static ServiceClient IoTHubServiceClient;
        private static string ServiceConnectionString;

        [FunctionName("relaysetpoint")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)][FromBody] string value,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            ServiceConnectionString = Environment.GetEnvironmentVariable("InfernoIoTServiceConnection", EnvironmentVariableTarget.Process);
            IoTHubServiceClient = ServiceClient.CreateFromConnectionString(ServiceConnectionString);
            log.LogInformation("value = " + value);
          

            if (string.IsNullOrEmpty(value)) // TODO: Check value for vaild range... 180 - 400 or whatever.
            {
                return new BadRequestObjectResult("Missing body value. Body should be a single integer.");
            }
            
            var methodInvocation = new CloudToDeviceMethod("SmokerSetPoint") { ResponseTimeout = TimeSpan.FromSeconds(30) };
            methodInvocation.SetPayloadJson(value);

            // Invoke the direct method asynchronously and get the response from the simulated device.
            var response = await IoTHubServiceClient.InvokeDeviceMethodAsync("inferno1", methodInvocation);

            Console.WriteLine("Response status: {0}, payload:", response.Status);
            Console.WriteLine(response.GetPayloadAsJson());

            log.LogInformation("Response status: {0}, payload:", response.Status);
            log.LogInformation(response.GetPayloadAsJson());
            return new ObjectResult(response);

            // if (response.Status)
            // {
            //     log.LogInformation("response.IsSuccessStatusCode PASSED");
            //     return new OkObjectResult(response.GetPayloadAsJson());
            // }
            // else
            // {
            //     log.LogInformation("response.IsSuccessStatusCode FAILED");
            //     return new BadRequestObjectResult(response.ReasonPhrase);
            // }

        }

    }
}


