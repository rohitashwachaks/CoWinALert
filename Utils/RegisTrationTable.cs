using System;
using System.Collections.Generic;
using System.Linq;
using CoWinAlert.DTO;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;

namespace CoWinAlert.Utils
{
    public static class TableInfo
    {
        private static CloudTable registrationTable;
        public static void InitialiseConfig()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            
            var client = account.CreateCloudTableClient();
            var x = account.TableEndpoint;
            
            registrationTable = client.GetTableReference("UserRegistration");
        }
        public static bool isUserExisting(Registration user)
        {
            string vaccineFilter = TableQuery.GenerateFilterCondition("PartitionKey",
                                                                QueryComparisons.Equal,
                                                                user.Vaccine.ToString()
                                                            );
            string emailFilter = TableQuery.GenerateFilterCondition("RowKey",
                                                                QueryComparisons.Equal,
                                                                user.EmailID
                                                            );
            string filter = TableQuery.CombineFilters(vaccineFilter, TableOperators.And, emailFilter);

            TableQuery tableQuery = new TableQuery().Where(filter);
            
            int queriedResponse = registrationTable.ExecuteQuery(tableQuery)
                                                            .ToList()
                                                            .Count;
            return (queriedResponse == 0);            
        }
        public static string AddRowtoTable(Registration user)
        {
            string responseMessage = "\nUser Added Succesfully.\n";
            try{
                RegistrationTableSchema reg = new RegistrationTableSchema(user);
                TableOperation tableOperation = TableOperation.InsertOrReplace(reg);
                
                registrationTable.Execute(tableOperation);
                responseMessage = JsonConvert.SerializeObject(reg, Formatting.Indented);
            }
            catch(Exception ex){
                responseMessage = JsonConvert.SerializeObject(ex)+"\n";
            }
            return responseMessage;
        }
        // public static Registration FetchUsers(string vaccineName)
        // {
        //     string filter = TableQuery.GenerateFilterConditionForBool("isActive",
        //                                                                 QueryComparisons.Equal,
        //                                                                 true
        //                                                             );
        //     if(!String.IsNullOrEmpty(vaccineName)){
        //         string vaccineFilter = TableQuery.GenerateFilterCondition("PartitionKey",
        //                                                         QueryComparisons.Equal,
        //                                                         vaccineName
        //                                                     );
        //         filter = TableQuery.CombineFilters(vaccineFilter, TableOperators.And, filter);
        //     }
            
        //     TableQuery tableQuery = new TableQuery().Where(filter);
            
        //     List<Registration> queriedResponse = registrationTable.ExecuteQuery(tableQuery)
        //                                                     .Select( _item => new Registration(){
        //                                                         Name = _item.Properties.ContainsKey("Name") ? _item.Properties["Name"].StringValue : null,
        //                                                         EmailID = _item.Properties.ContainsKey("EmailID") ? _item.Properties["EmailID"].StringValue : null,
        //                                                         YearofBirth = (int)(_item.Properties.ContainsKey("Age") ? DateTime.Now.Year -  _item.Properties["Age"].Int32Value : 45),
        //                                                         Phone = ,
        //                                                         PinCode = ,
        //                                                     }.Initialise((Vaccine)Enum.Parse(typeof(Vaccine), _item.PartitionKey))
        //                                                     ).ToList()
        //                                                     .Count;
            
        // }

    }
}