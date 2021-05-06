using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CoWinAlert.DTO;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Collections.Generic;

namespace CoWinAlert.Function
{
    public static class UserRegistration
    {
        [FunctionName("UserRegistration")]
        // [OpenApiOperation("list", "sample")]
        // [OpenApiParameter("Registration Details", In = ParameterLocation., Required = false,Type = typeof(Registration))]
        [OpenApiRequestBody("application/json", typeof(Registration))]
        [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(string))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "user/registration")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Registration registrationData = JsonConvert.DeserializeObject<Registration>(requestBody);
            
            //registrationData.PinCode = new List<string>{"120","123d","123456","`12345"};
            log.LogInformation(JsonConvert.SerializeObject(registrationData, Formatting.Indented));
            

            if(registrationData == null){
                return new BadRequestObjectResult("No data");
            }
            string responseMessage = $"Hello {registrationData.Name}, ";
            responseMessage += string.IsNullOrEmpty(registrationData.EmailID)
                            ? $"Invalid Email. "
                            : $"{JsonConvert.SerializeObject(registrationData, Formatting.Indented)}";

            return new OkObjectResult(responseMessage);
        }
    }
}
