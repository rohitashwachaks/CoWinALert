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

namespace CoWinAlert.Function
{
    public static class PeriodicCoWinCheck
    {
        /// Function to periodically ceheck CoWin
        [FunctionName("PeriodicCoWinCheck")]
        public static async void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now.ToShortDateString()}");
            foreach(Registration user in TableInfo.FetchUsers()){
                log.LogInformation(JsonConvert.SerializeObject(user, Formatting.Indented));
                IEnumerable<HttpResponseMessage> response = await PingCoWin.GetResultAsync(new List<DateTime>(){DateTime.Now}, 
                                                                                    user.Codes,
                                                                                    user.District);
                foreach(HttpResponseMessage message in response){
                    log.LogInformation(JsonConvert.SerializeObject(message, Formatting.Indented));
                }
            }
        }
    }
}
