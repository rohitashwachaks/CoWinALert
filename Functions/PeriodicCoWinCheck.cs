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
        [FunctionName("PeriodicCoWinCheck")]
        public static async void Run([TimerTrigger("0 2-59/5 */4 * * *")]TimerInfo myTimer, ILogger log)
        {
            IEnumerable<SessionCalendarDTO> result = new List<SessionCalendarDTO>();
            int batchCount = DateTime.Now.Minute / 5;

            log.LogInformation($"Cowin website pinged at: {DateTime.Now.ToString("dd\\-MM\\-yyyy HH:mm:ss")}\nFetching Batch: {batchCount}");

            IEnumerable<RegistrationDTO> userList = TableInfo.FetchUsers(batchCount.ToString());
            log.LogInformation($"Users in Batch:\n{JsonConvert.SerializeObject(userList.Select(x => x.Name), Formatting.Indented)}");

            foreach(RegistrationDTO user in userList)
            {
                IEnumerable<DateTime> calendarDates = new List<DateTime>();

                calendarDates = GetDateList(user.PeriodDate.StartDate, user.PeriodDate.EndDate, log);
                
                log.LogInformation($"{user.Name}'s Calendar Dates: \n"+JsonConvert.SerializeObject(calendarDates, Formatting.Indented));
                
                try
                {
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
                }
                catch(Exception ex)
                {
                    log.LogError(ex.Message);
                    log.LogError(JsonConvert.SerializeObject(ex.InnerException, Formatting.Indented));
                    log.LogError(ex.StackTrace);
                }

                log.LogInformation($"{user.Name}'s Result\n"+JsonConvert.SerializeObject(result, Formatting.Indented));

                try
                {
                    if(result.ToList().Count > 0)
                    {
                        string htmlBody = Notifications.StructureSessionEmailBody(user.Name, result);
                        
                        string response = await Notifications.SendEmail(
                                                        userEmail: user.EmailID,
                                                        userName: user.Name,
                                                        htmlContent: htmlBody);
                        log.LogInformation($"{user.Name}'s Response:\n"+response);
                    }
                }
                catch(Exception ex)
                {
                    log.LogError(ex.Message);
                }
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
                datelst = datelst.Append(_date.Date);
            }
            return datelst;
        }
    }
}
