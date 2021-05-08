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
        [FunctionName("structure-input")]
        [OpenApiOperation]
        [OpenApiParameter("name", In = ParameterLocation.Query, Required = true, Type = typeof(string))]
        [OpenApiParameter("email", In = ParameterLocation.Query, Required = true, Type = typeof(string))]
        [OpenApiParameter("phone", In = ParameterLocation.Query, Required = true, Type = typeof(string))]
        [OpenApiParameter("birth-year", In = ParameterLocation.Query, Required = true, Type = typeof(int))]
        [OpenApiParameter("start-date", In = ParameterLocation.Query, Required = true, Type = typeof(string))]
        [OpenApiParameter("end-date", In = ParameterLocation.Query, Required = true, Type = typeof(string))]
        [OpenApiParameter("pincode", In = ParameterLocation.Query, Required = true, Type = typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(InputDTO))]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, "application/json", typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, "application/json", typeof(JObject))]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "step-1/structure-input")] HttpRequest req,
            ILogger log)
        {
            InputDTO inputResult = new InputDTO();
            try
            {
                inputResult.Name = HttpUtility.HtmlEncode(req.Query["name"]);
                inputResult.EmailID = HttpUtility.HtmlEncode(req.Query["email"]);
                inputResult.Phone = HttpUtility.HtmlEncode(req.Query["phone"]);
                inputResult.YearofBirth = Int16.Parse(HttpUtility.HtmlEncode(req.Query["birth-year"]));
                inputResult.PeriodDate = new DateRangeDTO(){
                                                        StartDate = DateTime.Parse(HttpUtility.HtmlEncode(req.Query["start-date"])),
                                                        EndDate = DateTime.Parse(HttpUtility.HtmlEncode(req.Query["end-date"]))
                                                    };
                inputResult.PinCode = JsonConvert.SerializeObject(HttpUtility.HtmlEncode(req.Query["pincode"]).Split(","));
                
                if(inputResult.isValid())
                {
                    return HttpResponseHandler.StructureResponse(content: inputResult,
                                                            code: HttpStatusCode.OK 
                                                        );
                }
                else
                {
                    return HttpResponseHandler.StructureResponse(reason: "Invalid Query Parameters",
                                                            content: inputResult,
                                                            code: HttpStatusCode.BadRequest 
                                                        );
                }
                
            }
            catch(Exception ex)
            {
                return HttpResponseHandler.StructureResponse(reason: "Invalid Query Parameters",
                                                        content: ex.StackTrace,
                                                        code: HttpStatusCode.InternalServerError 
                                                    );
            }
        }
    }
}