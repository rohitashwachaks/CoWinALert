using System;
using System.Collections.Generic;
using System.Linq;
using CoWinAlert.DTO;
using CoWinAlert.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
        public static async void Run([TimerTrigger("0 2-59/5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Cowin website pinged at: {DateTime.Now.ToString("dd\\-MM\\-yyyy HH:MM:ss")}");
            IEnumerable<SessionCalendarDTO> result = new List<SessionCalendarDTO>();

            int batchCount = DateTime.Now.Minute / 5;

            foreach(RegistrationDTO user in TableInfo.FetchUsers(batchCount.ToString()))
            {
                log.LogInformation(JsonConvert.SerializeObject(user, Formatting.Indented));
                
                IEnumerable<DateTime> calendarDates = new List<DateTime>();

                calendarDates = GetDateList(user.PeriodDate.StartDate, user.PeriodDate.EndDate, log);
                
                log.LogInformation("Calendar Dates: \n"+JsonConvert.SerializeObject(calendarDates, Formatting.Indented));
                
                await foreach(SessionCalendarDTO center in PingCoWin.GetResultAsync(
                                                            calendarDates, 
                                                            user.Codes,
                                                            user.District,
                                                            log))
                {
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
                    string htmlBody = Notifications.StructureSessionEmailBody(result);
                    // log.LogInformation(htmlBody);
                    string response = await Notifications.SendEmail(
                                                    userEmail: user.EmailID,
                                                    userName: user.Name,
                                                    htmlContent: htmlBody);
                    log.LogInformation(response);
                }
                log.LogInformation("Result"+JsonConvert.SerializeObject(result, Formatting.Indented));
            }
        }

        private static IEnumerable<DateTime> GetDateList(DateTime startDate, DateTime endDate, ILogger log)
        {
            int weekSpan = int.Parse(Environment.GetEnvironmentVariable("WEEK_LOOK_AHEAD"));
            TimeSpan windowPeriod = TimeSpan.FromDays(7*weekSpan); // Look Ahead 7 weeks.
            DateTime lastDate = DateTime.Now + windowPeriod;
            
            // Start Date is User Start Date or Current date, whichever is later.
            startDate = (DateTime.Compare(DateTime.Now.Date, startDate.Date) < 0)? startDate : DateTime.Now;
            // End Date is User End Date or Last date, whichever is earlier.
            endDate = (DateTime.Compare(lastDate, endDate.Date) > 0)? endDate : lastDate;

            IEnumerable<DateTime> datelst = new List<DateTime>();
            for(DateTime _date = startDate;
                _date <= endDate;   // End Date will be included
                _date = _date.Add(TimeSpan.FromDays(7)))
            {
                datelst = datelst.Append(_date);
            }
            return datelst;
        }
    }
}
