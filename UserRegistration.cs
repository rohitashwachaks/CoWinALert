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
using CoWinAlert.Utils;
using System.Collections.Generic;
using System.Web;
using System.Net.Http;

namespace CoWinAlert.Function
{
    public static class UserRegistration
    {
        [FunctionName("UserRegistration")]
        [OpenApiOperation]
        [OpenApiParameter("vaccine", In = ParameterLocation.Query, Required = true, Type = typeof(string))]
        [OpenApiRequestBody("application/json", typeof(Registration))]
        [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(string))]
        public static async Task<HttpResponseMessage> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "user/registration")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation(HttpUtility.HtmlEncode("C# HTTP trigger function processed a request."));

            string vaccine = HttpUtility.HtmlEncode(req.Query["vaccine"].ToString());
            Vaccine vaccineName = Vaccine.Invalid;
            try{
                vaccineName = (Vaccine)Enum.Parse(typeof(Vaccine), req.Query["vaccine"]);
            }
            catch(Exception ex){
                return HttpResponseHandler.StructureResponse(content: ex,
                                                        code: HttpStatusCode.InternalServerError 
                                                    );
            }
            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Registration registrationData = JsonConvert.DeserializeObject<Registration>(requestBody);
            
            //registrationData.PinCode = new List<string>{"120","123d","123456","`12345"};
            log.LogInformation(JsonConvert.SerializeObject(registrationData, Formatting.Indented));            

            if(registrationData == null){
                return HttpResponseHandler.StructureResponse(content: "No data",
                                                        code: HttpStatusCode.BadRequest 
                                                    );
            }
            string responseMessage = $"Hello {registrationData.Name}, ";
            responseMessage += string.IsNullOrEmpty(registrationData.EmailID)
                            ? $"Invalid Email. "
                            : $"{JsonConvert.SerializeObject(registrationData, Formatting.Indented)}";

            return HttpResponseHandler.StructureResponse(content: responseMessage,
                                                        code: HttpStatusCode.OK 
                                                    );
        }
    }
}
