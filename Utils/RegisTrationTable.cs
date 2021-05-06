using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace CoWinAlert.Utils
{
    public static class TableInfo
    {
        private static CloudTable registrationTable;
        private static TableQuery tableQuery;
        public static void InitialiseConfig(){
            CloudStorageAccount account = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            var client = account.CreateCloudTableClient();
            registrationTable = client.GetTableReference("UserRegistration");
        }
    }
}