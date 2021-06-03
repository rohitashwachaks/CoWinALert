using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CoWinAlert.DTO
{
    public class RegistrationDTO
    {
        #region Private Members
        private bool _isValid = true;
        private string _reasonPhrase = "";
        private List<Vaccine> _vaccine = new List<Vaccine>(){ DTO.Vaccine.ANY };
        private FeeTypeDTO _payment = DTO.FeeTypeDTO.ANY;
        private List<long> _pincodes = new List<long>();
        private List<int> _districtcodes = new List<int>();
        private int _yearofBirth = (DateTime.Now.Year - 45);
        private string _name = "";
        private string _email = "";
        private string _phone = "";
        #endregion Private Members
        
        public RegistrationDTO()
        {
            Random randomGenereator = new Random();
            int batchCount = randomGenereator.Next(10000)%12;
            
            Batch = String.IsNullOrEmpty(Batch)?  batchCount.ToString(): Batch;
        }

        #region Public Members
        public string Name{
            get{
                return _name;
            }
            set{
                try{
                    if(!String.IsNullOrEmpty(value)){
                        _name = value;
                    }
                    else{
                        _isValid = false;
                    }
                }
                catch{
                    _isValid = false;
                    _reasonPhrase += $"\nError in Name Parsing. Input value = {value}";
                }
            }
        }
        public string EmailID{
            get{
                return _email;
            }
            set{
                try{
                    if(Regex.IsMatch(value,@"^[a-z0-9+_.-]+@[a-z0-9.-]+$")){
                        _email = value;
                    }
                    else{
                        _isValid = false;
                    }
                }
                catch{
                    _isValid = false;
                    _reasonPhrase += $"\nError in EmailID Parsing. Input value = {value}";
                }
            }
        }
        public int YearofBirth{
            get{
                return _yearofBirth;
            }
            set{
                try{
                    if(Regex.IsMatch(value.ToString(),@"^19[0-9]{2}$|^20[0-9]{2}$")){
                        _yearofBirth = value;
                    }
                    else{
                        _isValid = false;
                        _reasonPhrase += $"\nError in Year of Birth Parsing.";
                    }
                }
                catch{
                    _isValid = false;
                    _reasonPhrase += $"\nError in Year of Birth Parsing. Input value = {value.ToString()}";
                }        
            }
        }
        public DateRangeDTO PeriodDate{get;set;}
        [JsonIgnore]
        public List<long> Codes{
            get{
                return _pincodes;
            }
        }
        public string PinCode{
            set{
                try{
                    if(String.IsNullOrEmpty(value))
                    {
                        _pincodes = new List<long>();
                    }
                    else
                    {
                        _pincodes = JsonConvert.DeserializeObject<List<long>>(value)
                                            .Where(_code => Regex.IsMatch(_code.ToString(), @"^[0-9]{6}$"))
                                            .ToList();
                    }
                    // if(_pincodes.Count == 0){
                    //     _isValid = false;
                    // }
                }
                catch{
                    _isValid = false;
                    _reasonPhrase += $"\nError in Pin Code Parsing. Input value = {value}";
                }
            }
        }
        [JsonIgnore]
        public List<int> District{
            get{
                return _districtcodes;
            }
        }
        public string DistrictCode{
            set{
                try{
                    if(String.IsNullOrEmpty(value))
                    {
                        _districtcodes = new List<int>();
                    }
                    else
                    {
                        _districtcodes = JsonConvert.DeserializeObject<List<int>>(value)
                                                    .Where(_code => 
                                                        Regex.IsMatch(_code.ToString(), @"^[0-9]+$")
                                                    )
                                                    .ToList();
                    }
                    // if(_districtcodes.Count == 0){
                    //     _isValid = false;
                    // }
                }
                catch{
                    _isValid = false;
                    _reasonPhrase += $"\nError in District code Parsing. Input value = {value}";
                }
            }
        }
        public string Phone{
            get{
                return _phone;
            }
            set{
                try{
                    if(Regex.IsMatch(value.ToString(),@"^[0-9]{10}$")){
                        _phone = value;
                    }
                    if(String.IsNullOrEmpty(_phone)){
                        _isValid = false;
                        _reasonPhrase += $"\nInvalid Phone Number";
                    }
                }
                catch{
                    _isValid = false;
                    _reasonPhrase += $"\nError in Phone Number Parsing. Input value = {value}";
                }        
            }
        }        
        [JsonIgnore]
        public string Vaccine
        { 
            get
            {
                return JsonConvert.SerializeObject(_vaccine);//_vaccine.ToString();
            } 
            set
            {
                try
                {
                    try{
                        _vaccine = JsonConvert.DeserializeObject<List<Vaccine>>(value).ToList();
                        if(_vaccine.Count == 0){
                            _isValid = false;
                        }
                    }
                    catch{
                        _vaccine = value.Split(",")
                                        .Select(_item => (Vaccine)Enum.Parse(typeof(Vaccine), _item.ToUpper()))
                                        .ToList();
                    }
                }
                catch
                {
                    _isValid = false;
                    _reasonPhrase += $"\nError in Vaccine Parsing. Input value = {value}";
                }
            }
        }
        [JsonIgnore]
        public string Payment
        {
            get
            {
                return _payment.ToString();
            }
            set
            {
                try
                {
                    _payment = (FeeTypeDTO)Enum.Parse(typeof(FeeTypeDTO), value.ToUpper());
                }
                catch
                {
                    _isValid = false;
                    _reasonPhrase += $"\nError in Payment Parsing. Input value = {value}";
                }
            }
        }
        [JsonIgnore]
        public string Batch{get;set;}
        public bool IsActive {get; set;}
        #endregion Public Members

        #region Public Functions        
        public bool isValid(){
            return _isValid;
        }
        public string InvalidReason(){
            return _reasonPhrase;
        }
        #endregion Public Functions
    }
    public class RegistrationTableSchemaDTO : TableEntity{
        public string Name{get;set;}
        public int YearofBirth{get;set;}
        public string PeriodDate{get;set;}
        public string PinCode{get;set;}
        public string DistrictCode{get;set;}
        public string Phone{get;set;}
        public string Vaccine{get;set;}
        public string Payment{get;set;}
        public bool isActive{get;set;}
        public RegistrationTableSchemaDTO(){}
        public RegistrationTableSchemaDTO(RegistrationDTO inp){
            this.PartitionKey = inp.Batch;
            this.RowKey = inp.EmailID;
            this.Phone = inp.Phone;
            this.Name = inp.Name;
            this.YearofBirth = inp.YearofBirth;
            this.Vaccine = inp.Vaccine.ToString();
            this.PeriodDate = JsonConvert.SerializeObject(inp.PeriodDate);
            this.PinCode = JsonConvert.SerializeObject(inp.Codes);
            this.DistrictCode = JsonConvert.SerializeObject(inp.District);
            this.Payment = inp.Payment;
            this.isActive = inp.IsActive;
        }
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Vaccine{
        ANY,
        COVISHIELD,
        COVAXIN,
        [Display(name:"SPUTNIK V")]
        SPUTNIK 
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FeeTypeDTO
    {
        ANY,
        FREE,
        PAID
    }
    public class DateRangeDTO{
        public DateTime StartDate{get;set;}
        public DateTime EndDate{get;set;}
        public DateRangeDTO(){
            StartDate = DateTime.Now;
            EndDate = DateTime.Now;
        }
    }
}