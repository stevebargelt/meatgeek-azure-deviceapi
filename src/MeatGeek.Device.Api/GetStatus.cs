using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Relay;
using Microsoft.Extensions.Configuration;


namespace Infero.Function
{
    public static class GetStatus
    {
        private static IConfiguration Configuration { set; get; }
        private static string RelayNamespace;
        private static string ConnectionName;
        private static string KeyName;
        private static string Key;

        [FunctionName("status")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            
            log.LogInformation("C# HTTP trigger function processed a request.");
            RelayNamespace = Environment.GetEnvironmentVariable("RelayNamespace", EnvironmentVariableTarget.Process);
            ConnectionName = Environment.GetEnvironmentVariable("RelayConnectionName", EnvironmentVariableTarget.Process);
            KeyName = Environment.GetEnvironmentVariable("RelayKeyName", EnvironmentVariableTarget.Process);
            Key = Environment.GetEnvironmentVariable("RelayKey", EnvironmentVariableTarget.Process);

            // Begin
            HttpClient client = HttpClientFactory.Create();
            var baseUri = new Uri(string.Format("https://{0}/{1}/", RelayNamespace, ConnectionName));
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(baseUri, "status"),
                Method = HttpMethod.Get
            };

            await AddAuthToken(request);

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string payload = await response.Content.ReadAsStringAsync();

                return new OkObjectResult(payload);
            }
            else
            {
                return new BadRequestObjectResult(response.ReasonPhrase);
            }

        }

        private static async Task AddAuthToken(HttpRequestMessage request)
        {
            TokenProvider tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(KeyName, Key);
            string token = (await tokenProvider.GetTokenAsync(request.RequestUri.AbsoluteUri, TimeSpan.FromHours(1))).TokenString;

            request.Headers.Add("ServiceBusAuthorization", token);
        }

    }
}
