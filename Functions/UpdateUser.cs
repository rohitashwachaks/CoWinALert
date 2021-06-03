using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CoWinAlert.DTO;
using CoWinAlert.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace CoWinAlert.Function
{
    public static class UpdateUser
    {
        [FunctionName("UpdateUser")]
        [Disable]
        [OpenApiOperation(tags: new[] { "API" }, Description = "Updating Existing Users")]
        //[OpenApiIgnore]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "vaccine", In = ParameterLocation.Query, Required = true, Type = typeof(List<Vaccine>), Description = "Vaccine Preference")]
        [OpenApiParameter(name: "payment", In = ParameterLocation.Query, Required = true, Type = typeof(FeeTypeDTO), Description = "Payment Preference")]
        [OpenApiRequestBody("application/json", typeof(RegistrationDTO))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(InputDTO), Description = "The OK Response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "The Invalid Input Response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Conflict, contentType: "text/plain", bodyType: typeof(string), Description = "The Duplicate Entry Response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "text/plain", bodyType: typeof(string), Description = "The InternalServerError Response")]
        public static async Task<HttpResponseMessage> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "registration")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return HttpResponseHandler.StructureResponse(content: responseMessage,
                                                            code: HttpStatusCode.Conflict 
                                                        );
        }
    }
}

