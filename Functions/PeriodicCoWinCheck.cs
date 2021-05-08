using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoWinAlert.DTO;
using CoWinAlert.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoWinAlert.Function
{
    public static class PeriodicCoWinCheck
    {
        /// Function to periodically ceheck CoWin
        [FunctionName("PeriodicCoWinCheck")]
        // [Disable]
        public static async void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Cowin website pinged at: {DateTime.Now.ToShortDateString()}");
            
            foreach(RegistrationDTO user in TableInfo.FetchUsers())
            {
                log.LogInformation(JsonConvert.SerializeObject(user, Formatting.Indented));
                IEnumerable<JObject> response = new List<JObject>();
                // IEnumerable<DateTime> singleDates = new List<DateTime>();
                IEnumerable<DateTime> calendarDates = new List<DateTime>();

                DateTime startDate = user.PeriodDate.StartDate;
                DateTime endDate = user.PeriodDate.EndDate;
                TimeSpan periodDifference = endDate - startDate;

                log.LogDebug($"Start Date: {startDate.ToShortDateString()}\n"+
                                $"End Date:{endDate.ToShortDateString()}\n"+
                                $"PeriodDuration:{periodDifference.TotalDays.ToString()}"
                            );
                // if start date = end date
                if(periodDifference == TimeSpan.FromDays(0))
                {
                    calendarDates = calendarDates.Append(startDate);
                }
                //else if start date - end date <= 7
                else if(periodDifference > TimeSpan.FromDays(0))
                {
                    for(TimeSpan i = TimeSpan.FromDays(0);
                        i <= periodDifference;
                        i = i.Add(TimeSpan.FromDays(7)))
                    {
                        DateTime tempDate = startDate + i;
                        calendarDates = calendarDates.Append(tempDate);
                    }
                }
                log.LogDebug(JsonConvert.SerializeObject(calendarDates));
                
                await foreach(string message in PingCoWin.GetResultAsync(
                                                            new List<DateTime>(){DateTime.Now}, 
                                                            user.Codes,
                                                            user.District)){
                    response = response.Append(JsonConvert.DeserializeObject<JObject>(message));
                }
                
                log.LogInformation(JsonConvert.SerializeObject(response, Formatting.Indented));
            }
        }
    }
}
