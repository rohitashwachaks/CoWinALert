using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CoWinAlert.DTO
{
    public class Registration
    {
        #region Private Members
        private bool _isValid = true;
        private List<long> _pincodes = new List<long>();
        private int _yearofBirth = (DateTime.Now.Year - 45);
        private string _name = "";
        private string _email = "";
        private string _phone = "";
        private Vaccine _vaccine;
        #endregion
        
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
                }
            }
        }
        public int Age{
            get{
                return DateTime.Now.Year - _yearofBirth;
            }
        }
        // public string SearchByMode{
        //     get{
        //         return _searchmode.ToString();
        //     }
        //     set{
        //         try{
        //             _searchmode = (SearchMode)Enum.Parse(typeof(SearchMode),value);
        //         }
        //         catch{
        //             _isValid = false;
        //         }
        //     }
        // }
        public int YearofBirth{
            set{
                try{
                    if(Regex.IsMatch(value.ToString(),@"^19[0-9]{2}")){
                        _yearofBirth = value;
                    }
                    else{
                        _isValid = false;
                    }
                }
                catch{
                    _isValid = false;
                }        
            }
        }
        public List<long> Codes{
            get{
                return _pincodes;
            }
        }
        public string PinCode{
            set{
                try{
                    _pincodes = JsonConvert.DeserializeObject<List<string>>(value)
                                        .Where(_code => 
                                            Regex.IsMatch(_code, @"^[0-9]{6}")
                                        ).Select(_codes => Int64.Parse(_codes))
                                        .ToList();
                    if(_pincodes.Count == 0){
                        _isValid = false;
                    }
                }
                catch{
                    _isValid = false;
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
                    }
                }
                catch{
                    _isValid = false;
                }        
            }
        }
        public Vaccine Vaccine{
            get{
                return _vaccine;
            }
        }
        public bool isValid{
            get{
                return _isValid;
            }
        }
        #endregion Public Members

        #region Public Functions        
        public void Initialise(Vaccine vaccineName){
            _vaccine = vaccineName;
        }
        
        #endregion Public Functions
    }
    public class RegistrationTableSchema : TableEntity{
        public string Name{get;set;}
        public int Age{get;set;}
        public string PinCode{get;set;}
        public string Phone{get;set;}
        public RegistrationTableSchema(){}
        public RegistrationTableSchema(Registration inp){
            this.PartitionKey = inp.Vaccine.ToString();
            this.RowKey = inp.EmailID;
            this.Phone = inp.Phone;
            this.Name = inp.Name;
            this.Age = inp.Age;
            this.PinCode = JsonConvert.SerializeObject(inp.Codes);
        }
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Vaccine{
        covishield,
        covaxin
    }
}