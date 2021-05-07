using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace CoWinAlert.Function
{
    public static class PeriodicCoWinCheck
    {
        /// Function to periodically ceheck CoWin
        [FunctionName("PeriodicCoWinCheck")]
        public static void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
        }
    }
}
