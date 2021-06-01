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
        ///Function to Register Users and their Preferences
        [FunctionName("api-registration")]
        [OpenApiOperation]
        //[OpenApiIgnore]
        [OpenApiParameter("vaccine", In = ParameterLocation.Query, Required = true, Type = typeof(Vaccine))]
        [OpenApiParameter("payment", In = ParameterLocation.Query, Required = true, Type = typeof(FeeTypeDTO))]
        [OpenApiRequestBody("application/json", typeof(RegistrationDTO))]
        [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(InputDTO))]
        public static async Task<HttpResponseMessage> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "api/user-registration")] HttpRequest req,
            ILogger log)
        {
            string responseMessage = $"Hello ";
            string vaccineName = "";
            string payment = "";

            try{
                vaccineName = HttpUtility.HtmlEncode(req.Query["vaccine"].ToString());
                payment = HttpUtility.HtmlEncode(req.Query["payment"].ToString());
            }
            catch(Exception ex){
                return HttpResponseHandler.StructureResponse(reason: "Invalid Query Parameters",
                                                        content: ex.StackTrace,
                                                        code: HttpStatusCode.InternalServerError 
                                                    );
            }
            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            
            if(requestBody == null){
                log.LogError("No Data in request Body");
                return HttpResponseHandler.StructureResponse(content: "No data",
                                                        code: HttpStatusCode.BadRequest 
                                                    );
            }
            
            RegistrationDTO registrationData = new RegistrationDTO();
            
            try{
                registrationData = JsonConvert.DeserializeObject<RegistrationDTO>(requestBody);
                registrationData.Vaccine = vaccineName;
                registrationData.Payment = payment;

                log.LogInformation(JsonConvert.SerializeObject(registrationData, Formatting.Indented));
                responseMessage += $"{registrationData.Name}, ";

                // Check if it exists in table
                if(TableInfo.isUserExisting(registrationData) &&
                    registrationData.isValid()
                ){
                    // Add to Table
                    string x = TableInfo.AddRowtoTable(registrationData);
                    log.LogInformation(x);

                    // Send Email
                    x = await Notifications.RegisterEmailAsync(registrationData);
                    log.LogInformation(x);
                }
            }
            catch(Exception ex){
                log.LogError(ex.Message);
                return HttpResponseHandler.StructureResponse(reason: ex.Message,
                                                        content: ex.StackTrace,
                                                        code: HttpStatusCode.InternalServerError 
                                                    );
            }            
            
            
            responseMessage += registrationData.isValid()
                            ? $"Your details have been registered. You will recieve notifications on {registrationData.EmailID},"
                                +" Remember to check the SPAM FOLDER for mails from captain.nemo.github@gmail.com"
                            : "Invalid Data";

            return HttpResponseHandler.StructureResponse(content: responseMessage,
                                                        code: HttpStatusCode.OK 
                                                    );
        }
    }
}