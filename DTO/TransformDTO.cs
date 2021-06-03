using System;
using System.Collections.Generic;
using System.Linq;
using CoWinAlert.Utils;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;

namespace CoWinAlert.DTO
{
    public class TransformDTO
    {
        public string Batch{get;set;}
        public string EmailID{get;set;}
        public string Name{get;set;}
        public string YearofBirth{get;set;}
        public string PeriodDate{get;set;}
        public string PinCode{get;set;}
        public string DistrictCode{get;set;}
        public string Phone{get;set;}
        public string Vaccine{get;set;}
        public string Payment{get;set;}

        public IEnumerable<string> Transfer()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            CloudTableClient client = account.CreateCloudTableClient();
            CloudTable sourceTable = client.GetTableReference("UserRegistration");

            string filter = TableQuery.GenerateFilterConditionForBool("isActive",
                                                                        QueryComparisons.Equal,
                                                                        true
                                                                    );
            TableQuery tableQuery = new TableQuery().Where(filter);

            IEnumerable<string> responseList = new List<string>();
            try
            {
                int count = 1;
                foreach(string response in sourceTable.ExecuteQuery(tableQuery)
                                                    .Select( _item => TableInfo.UpsertRowtoTable(new RegistrationDTO(){
                                                        Vaccine = String.IsNullOrEmpty(_item.PartitionKey) ? 
                                                                                    "ANY"
                                                                                    : _item.PartitionKey,
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
                                                                                    : null,
                                                        IsActive = _item.Properties.ContainsKey("isActive") ? 
                                                                                    _item.Properties["isActive"].BooleanValue.Value
                                                                                    : true
                                                    })))
                {
                    responseList = responseList.Append($"{count++}: "+response);
                }
                
            }
            catch(Exception ex){
                 responseList = responseList.Append(ex.Message);
            }
            return responseList;
        }
   }
}