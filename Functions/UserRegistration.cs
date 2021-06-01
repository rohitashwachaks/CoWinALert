using System;
using System.Threading.Tasks;
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
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;

namespace CoWinAlert.Function
{
    public static class UserRegistration
    {
        [FunctionName("user-registration")]
        [OpenApiOperation(tags: new[] { "User" }, Description = "Registration for Users")]
        //[OpenApiIgnore]
        [OpenApiParameter(name:"name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "Name")]
        [OpenApiParameter(name:"email", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "Contanct Email-Id")]
        [OpenApiParameter(name:"phone", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "**10 digit** Phone Number")]
        [OpenApiParameter(name:"birth-year", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "Year of Birth")]
        [OpenApiParameter(name:"start-date", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "Search Start Date ( Format: **_YYYY-MM-DD_**)")]
        [OpenApiParameter(name:"end-date", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "Search End Date ( Format: **_YYYY-MM-DD_**)")]
        [OpenApiParameter(name:"pincode", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "Select Pincode(s)")]
        [OpenApiParameter(name:"vaccine", In = ParameterLocation.Query, Required = true, Type = typeof(Vaccine), Description = "Select Vaccine Preference")]
        [OpenApiParameter(name:"payment", In = ParameterLocation.Query, Required = true, Type = typeof(FeeTypeDTO), Description = "Select Payment Preference")]
        [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(InputDTO))]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, "text/plain", typeof(string))]
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
                                                        StartDate = DateTime.Parse(HttpUtility.HtmlEncode(req.Query["start-date"])),
                                                        EndDate = DateTime.Parse(HttpUtility.HtmlEncode(req.Query["end-date"]))
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