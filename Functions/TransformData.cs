using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CoWinAlert.DTO;
using System.Collections.Generic;
using System;

namespace CoWinAlert.Function
{
    public static class TransformData
    {
        [FunctionName("TransformData")]
        [Disable]
        [Obsolete]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "backend/migrate")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            TransformDTO transform = new TransformDTO();
            IEnumerable<string> response = transform.Transfer();

            return new OkObjectResult(JsonConvert.SerializeObject(response, Formatting.Indented));
        }
    }
}
