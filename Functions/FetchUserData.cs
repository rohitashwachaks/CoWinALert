using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using CoWinAlert.DTO;
using System.Net.Http;
using CoWinAlert.Utils;
using System.Web;
using System;

namespace CoWinAlert.Function
{
    public static class FetchUserData
    {
        [FunctionName("api-fetch-data")]
        [OpenApiOperation(tags: new[] { "API" }, Description = "Fetch Data of Registered Users")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "email-Id", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "Registered Email-ID of user")]
        [OpenApiParameter(name: "phone", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "Registered Phone Number")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(InputDTO), Description = "The OK Response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "The BadRequest Response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "text/plain", bodyType: typeof(string), Description = "The InternalServerError Response")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "fetch-data")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Fetch User Data HTTP trigger function processed a request.");
            string responseMessage = "";

            string emailId = HttpUtility.HtmlEncode(req.Query["email-Id"]);
            string phoneNumber = HttpUtility.HtmlEncode(req.Query["phone"]);

            log.LogInformation($"Querying data for:\n User = {emailId}\n Phone Number = {phoneNumber}");

            if (string.IsNullOrEmpty(emailId) || string.IsNullOrEmpty(phoneNumber))
            {
                responseMessage = "Empty Parameters. Retry with Valid Arguments";
                log.LogInformation(responseMessage);
                return HttpResponseHandler.StructureResponse(content: responseMessage,
                                                        code: HttpStatusCode.BadRequest
                                                    );
            }

            RegistrationDTO queriedUser = new RegistrationDTO();
            try
            {
                queriedUser = TableInfo.FetchUser(emailId: emailId, phone: phoneNumber);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                log.LogError(ex.StackTrace);
                return HttpResponseHandler.StructureResponse(reason: ex.Message,
                                                        content: ex.StackTrace,
                                                        code: HttpStatusCode.InternalServerError
                                                    );
            }

            if (string.IsNullOrEmpty(queriedUser.EmailID))
            {
                responseMessage = "No User Found. Please Check email Id and Phone Number";
                log.LogError(responseMessage);
                return HttpResponseHandler.StructureResponse(content: responseMessage,
                                                        code: HttpStatusCode.BadRequest
                                                    );
            }
            responseMessage = $"Record found for {queriedUser.Name}";
            log.LogInformation(responseMessage);
            return HttpResponseHandler.StructureResponse(content: queriedUser,
                                                        code: HttpStatusCode.OK
                                                    );
        }
    }
}

