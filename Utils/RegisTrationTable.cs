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
        #region Private Variables and Initialisation
        private static CloudTable registrationTable;
        private static Random randomGenereator;
        public static void InitialiseConfig()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            var client = account.CreateCloudTableClient();
            registrationTable = client.GetTableReference(Environment.GetEnvironmentVariable("TABLE_NAME"));

            randomGenereator = new Random();
        }
        #endregion Private Variables and Initialisation

        public static bool isUserExisting(RegistrationDTO user)
        {
            string vaccineFilter = TableQuery.GenerateFilterCondition("Vaccine",
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
            return (queriedResponse != 0);            
        }
        public static string UpsertRowtoTable(RegistrationDTO user)
        {
            string responseMessage = $"\nUser: {user.Name} Added Succesfully.\n";
            try{
                RegistrationTableSchemaDTO reg = new RegistrationTableSchemaDTO(user);
                TableOperation tableOperation = TableOperation.InsertOrReplace(reg);
                
                registrationTable.Execute(tableOperation);
            }
            catch(Exception ex){
                responseMessage = JsonConvert.SerializeObject(ex)+"\n";
            }
            return responseMessage;
        }
        public static IEnumerable<RegistrationDTO> FetchPartition(string partition = null, string vaccineName = null)
        {
            string filter = TableQuery.GenerateFilterConditionForBool("isActive",
                                                                        QueryComparisons.Equal,
                                                                        true
                                                                    );
            if(!String.IsNullOrEmpty(partition)){
                string batchFilter = TableQuery.GenerateFilterCondition("PartitionKey",
                                                                QueryComparisons.Equal,
                                                                partition
                                                            );
                filter = TableQuery.CombineFilters(batchFilter, TableOperators.And, filter);
            }
            if(!String.IsNullOrEmpty(vaccineName)){
                string vaccineFilter = TableQuery.GenerateFilterCondition("Vaccine",
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
                                                        Batch = String.IsNullOrEmpty(_item.PartitionKey) ? 
                                                                                    partition
                                                                                    : _item.PartitionKey,
                                                        EmailID = String.IsNullOrEmpty(_item.RowKey) ? 
                                                                                    null
                                                                                    : _item.RowKey,
                                                        Vaccine = _item.Properties.ContainsKey("Vaccine") ? 
                                                                                    _item.Properties["Vaccine"].StringValue
                                                                                    : "ANY",
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
        public static RegistrationDTO FetchUser(string emailId = null, string phone = null)
        {
            string filter = TableQuery.GenerateFilterCondition("RowKey",
                                                            QueryComparisons.Equal,
                                                            emailId
                                                        );
            string phoneFilter = TableQuery.GenerateFilterCondition("Phone",
                                                            QueryComparisons.Equal,
                                                            phone
                                                        );
            filter = TableQuery.CombineFilters(filter, TableOperators.And, phoneFilter);

            TableQuery tableQuery = new TableQuery().Where(filter);
            RegistrationDTO queriedResponse = new RegistrationDTO();
            try
            {
                queriedResponse = registrationTable.ExecuteQuery(tableQuery)
                                            .Select( _item => new RegistrationDTO(){
                                                Batch = String.IsNullOrEmpty(_item.PartitionKey) ? 
                                                                            null
                                                                            : _item.PartitionKey,
                                                EmailID = String.IsNullOrEmpty(_item.RowKey) ? 
                                                                            null
                                                                            : _item.RowKey,
                                                Vaccine = _item.Properties.ContainsKey("Vaccine") ? 
                                                                            _item.Properties["Vaccine"].StringValue
                                                                            : "ANY",
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
                                                                            : null,
                                                IsActive = _item.Properties.ContainsKey("isActive") ? 
                                                                            _item.Properties["isActive"].BooleanValue.Value
                                                                            : true
                                                    })
                                                    .First();
                return queriedResponse;
            }
            catch{
                return queriedResponse;
            }
        }
    }
}