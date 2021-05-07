using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public static void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
            foreach(Registration user in TableInfo.FetchUsers()){
                log.LogInformation(JsonConvert.SerializeObject(user));
            }
        }
    }
}
