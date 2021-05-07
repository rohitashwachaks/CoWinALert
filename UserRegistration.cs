using System;
using System.IO;
using System.Threading.Tasks;
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
using System.Web;
using System.Net.Http;

namespace CoWinAlert.Function
{
    public static class UserRegistration
    {
        [FunctionName("UserRegistration")]
        [OpenApiOperation]
        [OpenApiParameter("vaccine", In = ParameterLocation.Query, Required = true, Type = typeof(Vaccine))]
        [OpenApiRequestBody("application/json", typeof(Registration))]
        [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(string))]
        public static async Task<HttpResponseMessage> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "user/registration")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation(HttpUtility.HtmlEncode("C# HTTP trigger function processed a request."));

            Vaccine vaccineName;
            try{
                string vaccine = HttpUtility.HtmlEncode(req.Query["vaccine"].ToString());
                vaccineName = (Vaccine)Enum.Parse(typeof(Vaccine), vaccine);
            }
            catch(Exception ex){
                return HttpResponseHandler.StructureResponse(reason: "Invalid Vaccine Name",
                                                        content: ex.StackTrace,
                                                        code: HttpStatusCode.InternalServerError 
                                                    );
            }
            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Registration registrationData = JsonConvert.DeserializeObject<Registration>(requestBody);
            registrationData.Initialise(vaccineName);
            // registrationData.PinCode = "[\"120\",\"123d\",\"123456\",\"`12345\"]";
            log.LogInformation(JsonConvert.SerializeObject(registrationData, Formatting.Indented));            

            try{
                // Check if it exists in table
                if(TableInfo.isUserExisting(registrationData)){
                    // Add to Table
                    string x = TableInfo.AddRowtoTable(registrationData);
                    log.LogInformation(x);
                }
            }
            catch(Exception ex){
                return HttpResponseHandler.StructureResponse(reason: ex.Message,
                                                        content: ex.StackTrace,
                                                        code: HttpStatusCode.InternalServerError 
                                                    );
            }            
            
            if(registrationData == null){
                return HttpResponseHandler.StructureResponse(content: "No data",
                                                        code: HttpStatusCode.BadRequest 
                                                    );
            }
            string responseMessage = $"Hello {registrationData.Name}, ";
            responseMessage += string.IsNullOrEmpty(registrationData.EmailID)
                            ? $"Invalid Email. "
                            : $"{JsonConvert.SerializeObject(registrationData, Formatting.None)}";

            return HttpResponseHandler.StructureResponse(content: responseMessage,
                                                        code: HttpStatusCode.OK 
                                                    );
        }
    }
}