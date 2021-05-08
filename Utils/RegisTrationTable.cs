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
        public static bool isUserExisting(RegistrationDTO user)
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
        public static string AddRowtoTable(RegistrationDTO user)
        {
            string responseMessage = "\nUser Added Succesfully.\n";
            try{
                RegistrationTableSchemaDTO reg = new RegistrationTableSchemaDTO(user);
                TableOperation tableOperation = TableOperation.InsertOrReplace(reg);
                
                registrationTable.Execute(tableOperation);
                responseMessage = JsonConvert.SerializeObject(reg, Formatting.Indented);
            }
            catch(Exception ex){
                responseMessage = JsonConvert.SerializeObject(ex)+"\n";
            }
            return responseMessage;
        }
        public static IEnumerable<RegistrationDTO> FetchUsers(string vaccineName = null)
        {
            string filter = TableQuery.GenerateFilterConditionForBool("isActive",
                                                                        QueryComparisons.Equal,
                                                                        true
                                                                    );
            if(!String.IsNullOrEmpty(vaccineName)){
                string vaccineFilter = TableQuery.GenerateFilterCondition("PartitionKey",
                                                                QueryComparisons.Equal,
                                                                vaccineName
                                                            );
                filter = TableQuery.CombineFilters(vaccineFilter, TableOperators.And, filter);
            }
            
            TableQuery tableQuery = new TableQuery().Where(filter);
            
            try
            {
                IEnumerable<RegistrationDTO> queriedResponse = registrationTable.ExecuteQuery(tableQuery)
                                                    .Select( _item => new RegistrationDTO(){
                                                        Vaccine = String.IsNullOrEmpty(_item.PartitionKey) ? 
                                                                                    "ANY"
                                                                                    : _item.PartitionKey.ToUpper(),
                                                        EmailID = String.IsNullOrEmpty(_item.RowKey) ? 
                                                                                    null
                                                                                    : _item.RowKey,
                                                        Payment = _item.Properties.ContainsKey("Payment") ? 
                                                                                    _item.Properties["Payment"].StringValue
                                                                                    :"ANY",
                                                        Name = _item.Properties.ContainsKey("Name") ?
                                                                                    _item.Properties["Name"].StringValue 
                                                                                    : null,
                                                        PeriodDate = _item.Properties.ContainsKey("PeriodDate") ?
                                                                                    JsonConvert.DeserializeObject<DateRangeDTO>(_item.Properties["PeriodDate"].StringValue) 
                                                                                    : new DateRangeDTO(){},
                                                        YearofBirth = (int)(_item.Properties.ContainsKey("YearofBirth") ?
                                                                                    _item.Properties["YearofBirth"].Int32Value 
                                                                                    : DateTime.Now.Year - 45),
                                                        Phone = _item.Properties.ContainsKey("Phone") ?
                                                                                    _item.Properties["Phone"].StringValue 
                                                                                    : null,
                                                        PinCode = _item.Properties.ContainsKey("PinCode") ?
                                                                                    _item.Properties["PinCode"].StringValue 
                                                                                    : null,
                                                        DistrictCode = _item.Properties.ContainsKey("DistrictCode") ?
                                                                                    _item.Properties["DistrictCode"].StringValue 
                                                                                    : null
                                                    });
                return queriedResponse;
            }
            catch{
                return new List<RegistrationDTO>().AsEnumerable();
            }
        }

    }
}