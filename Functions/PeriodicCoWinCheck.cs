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
        // [FunctionName("cowin-check")]
        // [OpenApiOperation]
        // public static async Task<HttpResponseMessage> RunAsync(
        //     [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "cowin/check")] HttpRequest req,
        //     ILogger log)
        [FunctionName("PeriodicCoWinCheck")]
        // [Disable]
        public static async void Run([TimerTrigger("0 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Cowin website pinged at: {DateTime.Now.ToShortDateString()}");
            IEnumerable<SessionCalendarDTO> result = new List<SessionCalendarDTO>();
            
            foreach(RegistrationDTO user in TableInfo.FetchUsers())
            {
                log.LogInformation(JsonConvert.SerializeObject(user, Formatting.Indented));
                
                IEnumerable<DateTime> calendarDates = new List<DateTime>();

                calendarDates = GetDateList(user.PeriodDate.StartDate, user.PeriodDate.EndDate);
                
                log.LogInformation(JsonConvert.SerializeObject(calendarDates));
                
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
                        center.Sessions = PingCoWin.GetFilteredSessions(center.Sessions, user);

                        if(center.Sessions.Count > 0 )
                        {
                            result = result.Append(center);
                        }
                    }
                }

                if(result.ToList().Count > 0)
                {
                    string response = await Notifications.SendEmail(
                                                    userEmail: user.EmailID,
                                                    userName: user.Name,
                                                    htmlContent: JsonConvert.SerializeObject(result,
                                                                                            Formatting.Indented
                                                                                            )
                                                );
                    log.LogInformation(response);
                }

                log.LogInformation(JsonConvert.SerializeObject(result, Formatting.Indented));
            }
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
