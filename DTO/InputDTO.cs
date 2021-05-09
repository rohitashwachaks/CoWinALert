using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace CoWinAlert.DTO
{
    public class InputDTO
    {
        #region Private Members
        private bool _isValid = true;
        private string _reasonPhrase = "";
        private List<long> _pincodes = new List<long>();
        private List<int> _districtcodes = new List<int>();
        private int _yearofBirth = (DateTime.Now.Year - 45);
        private string _name = "";
        private string _email = "";
        private string _phone = "";
        private Vaccine _vaccine = DTO.Vaccine.ANY;
        private FeeTypeDTO _payment = DTO.FeeTypeDTO.ANY;
        
        #endregion Private Members
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
                        _reasonPhrase += $".. Empty Name";
                    }
                }
                catch{
                    _isValid = false;
                    _reasonPhrase += $".. Error in Name Parsing. Input value = {value}";
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
                        _reasonPhrase += $".. InValid Email";
                    }
                }
                catch{
                    _isValid = false;
                    _reasonPhrase += $".. Error in EmailID Parsing. Input value = {value}";
                }
            }
        }
        public int YearofBirth{
            get{
                return _yearofBirth;
            }
            set{
                try{
                    if(Regex.IsMatch(value.ToString(),@"^19[0-9]{2}$")){
                        _yearofBirth = value;
                    }
                    else{
                        _isValid = false;
                        _reasonPhrase += $".. Invalid Year of Birth.";
                    }
                }
                catch{
                    _isValid = false;
                    _reasonPhrase += $".. Error in Year of Birth Parsing. Input value = {value.ToString()}";
                }        
            }
        }
        public DateRangeDTO PeriodDate{get;set;}
        public string PinCode{
            get{
                return JsonConvert.SerializeObject(_pincodes);
            }
            set{
                try{
                    _pincodes =JsonConvert.DeserializeObject<List<long>>(value)
                                    .Where(_code => Regex.IsMatch(_code.ToString(), @"^[0-9]{6}$"))
                                    //.Select(_code => long.Parse(_code))
                                    .ToList();
                    if(_pincodes.Count == 0){
                        _isValid = false;
                        _reasonPhrase += $".. Invalid Pin Code.";
                    }
                }
                catch{
                    _isValid = false;
                    _reasonPhrase += $".. Error in Pin Code Parsing. Input value = {value}";
                }
            }
        }
        [JsonIgnore]
        public string DistrictCode{
            get{
                return JsonConvert.SerializeObject(_districtcodes);
            }
            set{
                try{
                    _districtcodes = JsonConvert.DeserializeObject<List<int>>(value)
                                        .Where(_code => 
                                            Regex.IsMatch(_code.ToString(), @"^[0-9]+$")
                                        )
                                        // .Select(_code => int.Parse(_code))
                                        .ToList();
                    if(_pincodes.Count == 0){
                        _isValid = false;
                        _reasonPhrase += $".. Invalid District Code.Please give comma-separated values";
                    }
                }
                catch{
                    _isValid = false;
                    _reasonPhrase += $".. Error in District code Parsing. Input value = {value}";
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
                        _reasonPhrase += $".. Invalid Phone Number. 10 digits please";
                    }
                }
                catch{
                    _isValid = false;
                    _reasonPhrase += $".. Error in Phone Number Parsing. Input value = {value}";
                }        
            }
        }
        public string Vaccine
        { get
            {
                return _vaccine.ToString();
            } 
            set
            {
                try
                {
                    _vaccine = (Vaccine)Enum.Parse(typeof(Vaccine), value.ToUpper());
                }
                catch
                {
                    _isValid = false;
                    _reasonPhrase += $".. Error in Vaccine Parsing. Input value = {value}";
                }
            }
        }
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
                    _reasonPhrase += $".. Error in Payment Parsing. Input value = {value}";
                }
            }
        }

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
}