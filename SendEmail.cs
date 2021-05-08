using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Web;
using CoWinAlert.Utils;
using System.Net;
using System.Net.Http;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

namespace CoWinAlert.Function
{
    public static class SendEmail
    {
        [FunctionName("send-Notification")]
        [OpenApiOperation]
        // [OpenApiParameter("name", In = ParameterLocation.Query, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(string))]
        public static async Task<HttpResponseMessage> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "backend/send-notif")] HttpRequest req,
            ILogger log)
        {
            string response = "Working properly";
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = "John Doe";
            string plainTextMessage = $"Hi {name}! This is a test message. Please Ignore";
            string htmlMessage = $"<strong>Hi {name}! Coding is easy to do anywhere!</strong>";
            // string response = Notifications.SendWhatsAppMessage($"Hi {name}! This is a test message. Please Ignore");
            response += await Notifications.SendEmail(
                                                            userEmail: "rohitashwachaks@outlook.com",
                                                            userName: name,
                                                            htmlContent: htmlMessage,
                                                            plainContent: plainTextMessage
                                                        );
            
            // string responseMessage = string.IsNullOrEmpty(name)
            //     ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //     : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return HttpResponseHandler.StructureResponse(content: response,
                                                        code: HttpStatusCode.OK 
                                                    );
        }
    }
}
