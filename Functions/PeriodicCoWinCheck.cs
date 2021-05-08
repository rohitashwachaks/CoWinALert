using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;
using CoWinAlert.DTO;
using CoWinAlert.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoWinAlert.Function
{
    public static class PeriodicCoWinCheck
    {
        /// Function to periodically ceheck CoWin
        [FunctionName("cowin-check")]
        // [Disable]
        [OpenApiOperation]
        public static async Task<HttpResponseMessage> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "cowin/check")] HttpRequest req,
            ILogger log)
        // [FunctionName("PeriodicCoWinCheck")]
        // // [Disable]
        // public static async void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Cowin website pinged at: {DateTime.Now.ToShortDateString()}");
            IEnumerable<SessionCalendarDTO> result = new List<SessionCalendarDTO>();
            
            foreach(RegistrationDTO user in TableInfo.FetchUsers())
            {
                log.LogInformation(JsonConvert.SerializeObject(user, Formatting.Indented));
                
                IEnumerable<DateTime> calendarDates = new List<DateTime>();

                calendarDates = GetDateList(user.PeriodDate.StartDate, user.PeriodDate.EndDate);
                
                log.LogDebug(JsonConvert.SerializeObject(calendarDates));
                
                await foreach(SessionCalendarDTO center in PingCoWin.GetResultAsync(
                                                            calendarDates, 
                                                            user.Codes,
                                                            user.District))
                {
                    // ResponseDTO filteredResponse = new ResponseDTO();
                    // Process center iff Payment type matches
                    if(user.Payment == center.Fee_type.ToString()
                        ||user.Payment == FeeTypeDTO.ANY.ToString())
                    {
                        // List<SessionDTO> filteredSessions
                        center.Sessions = center.Sessions.Where(_session =>
                                                    // Minimum Age less than User Age
                                                    (_session.Min_age_limit <= (DateTime.Now.Year - user.YearofBirth))
                                                    // Session Date AFTER Start Date
                                                    &&(DateTime.Compare(user.PeriodDate.StartDate,_session.SessionDate) >= 0)
                                                    // Session Date BEFORE END Date
                                                    &&(DateTime.Compare(_session.SessionDate,user.PeriodDate.EndDate) <= 0)
                                                    // Available Capacity > 0
                                                    &&(_session.Available_capacity > 0)
                                                    // Vaccine of Choice
                                                    &&(user.Vaccine ==_session.Vaccine
                                                        ||user.Vaccine == DTO.Vaccine.ANY.ToString())
                                                ).Select(_session => _session)
                                                .ToList();
                        if(center.Sessions.Count > 0 )
                        {
                            result = result.Append(center);
                        }                        
                    }
                    // ResponseDTO response = PingCoWin.GetFilteredResult(centers, user);                    
                }
                
                log.LogInformation(JsonConvert.SerializeObject(result, Formatting.Indented));
            }
            return HttpResponseHandler.StructureResponse(content: result,
                                                        code: HttpStatusCode.OK 
                                                    );
        }

        private static IEnumerable<DateTime> GetDateList(DateTime startDate, DateTime endDate)
        {
            TimeSpan periodDifference = endDate - startDate;
            IEnumerable<DateTime> dateEnum = new List<DateTime>();
            // if start date = end date
                if(periodDifference == TimeSpan.FromDays(0))
                {
                    dateEnum = dateEnum.Append(startDate);
                }
                //else if start date - end date <= 7
                else if(periodDifference > TimeSpan.FromDays(0))
                {
                    for(TimeSpan i = TimeSpan.FromDays(0);
                        i <= periodDifference;
                        i = i.Add(TimeSpan.FromDays(7)))
                    {
                        dateEnum = dateEnum.Append((startDate + i));
                    }
                }
            return dateEnum;
        }
    }
}
