using CoWinAlert.Utils;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(CoWinAlert.Startup))]
namespace CoWinAlert
{
    public class Startup: FunctionsStartup{
        public override void Configure(IFunctionsHostBuilder builder){
            TableInfo.InitialiseConfig();
        }
    }
}