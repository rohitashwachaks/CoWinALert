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
using CoWinAlert.DTO;
using CoWinAlert.Utils;
using System.Net;
using System.Net.Http;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;

namespace CoWinAlert.Function
{
    public static class StructureInput
    {
        [FunctionName("user-registration")]
        [OpenApiOperation]
        [OpenApiParameter("name", In = ParameterLocation.Query, Required = true, Type = typeof(string))]
        [OpenApiParameter("email", In = ParameterLocation.Query, Required = true, Type = typeof(string))]
        [OpenApiParameter("phone", In = ParameterLocation.Query, Required = true, Type = typeof(string))]
        [OpenApiParameter("birth-year", In = ParameterLocation.Query, Required = true, Type = typeof(int))]
        [OpenApiParameter("search-start-date-yyyy-mm-dd", In = ParameterLocation.Query, Required = true, Type = typeof(string))]
        [OpenApiParameter("search-end-date-yyyy-mm-dd", In = ParameterLocation.Query, Required = true, Type = typeof(string))]
        [OpenApiParameter("pincode", In = ParameterLocation.Query, Required = true, Type = typeof(string))]
        [OpenApiParameter("vaccine", In = ParameterLocation.Query, Required = true, Type = typeof(Vaccine))]
        [OpenApiParameter("payment", In = ParameterLocation.Query, Required = true, Type = typeof(FeeTypeDTO))]
        [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(InputDTO))]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, "application/json", typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, "application/json", typeof(JObject))]
        public static async Task<HttpResponseMessage> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/registration")] HttpRequest req,
            ILogger log)
        {
            InputDTO inputResult = new InputDTO();
            try
            {
                #region Structute Inputs
                inputResult.Name = HttpUtility.HtmlEncode(req.Query["name"]);
                inputResult.EmailID = HttpUtility.HtmlEncode(req.Query["email"]);
                inputResult.Phone = HttpUtility.HtmlEncode(req.Query["phone"]);
                inputResult.YearofBirth = Int16.Parse(HttpUtility.HtmlEncode(req.Query["birth-year"]));
                inputResult.PeriodDate = new DateRangeDTO(){
                                                        StartDate = DateTime.Parse(HttpUtility.HtmlEncode(req.Query["search-start-date-yyyy-mm-dd"])),
                                                        EndDate = DateTime.Parse(HttpUtility.HtmlEncode(req.Query["search-end-date-yyyy-mm-dd"]))
                                                    };
                inputResult.PinCode = JsonConvert.SerializeObject(HttpUtility.HtmlEncode(req.Query["pincode"]).Split(","));
                inputResult.Vaccine = HttpUtility.HtmlEncode(req.Query["vaccine"].ToString());
                inputResult.Payment = HttpUtility.HtmlEncode(req.Query["payment"].ToString());
                #endregion Structute Inputs
                
                if(inputResult.isValid())
                {
                    RegistrationDTO registrationData = new RegistrationDTO(){
                                                                Name = inputResult.Name,
                                                                EmailID = inputResult.EmailID,
                                                                Phone = inputResult.Phone,
                                                                YearofBirth = inputResult.YearofBirth,
                                                                PeriodDate = inputResult.PeriodDate,
                                                                PinCode = inputResult.PinCode,
                                                                Vaccine = inputResult.Vaccine,
                                                                Payment = inputResult.Payment
                                                            };
                                                            
                    string body = "User Already Signed up";
                    // Check if it exists in table
                    if(TableInfo.isUserExisting(registrationData) &&
                        registrationData.isValid()
                    )
                    {
                        // Add to Table
                        body = TableInfo.AddRowtoTable(registrationData);
                        
                        // Send Email
                        body += await Notifications.RegisterEmailAsync(registrationData);
                        log.LogInformation(body);
                    }
                    return HttpResponseHandler.StructureResponse(content: body,
                                                            code: HttpStatusCode.OK 
                                                        );
                }
                else
                {
                    return HttpResponseHandler.StructureResponse(reason: inputResult.InvalidReason(),
                                                            content: inputResult,
                                                            code: HttpStatusCode.BadRequest 
                                                        );
                }
                
            }
            catch(Exception ex)
            {
                return HttpResponseHandler.StructureResponse(reason: $"Invalid Query Parameters+{ex.Message}",
                                                        content: ex.StackTrace,
                                                        code: HttpStatusCode.InternalServerError 
                                                    );
            }
        }
    }
}