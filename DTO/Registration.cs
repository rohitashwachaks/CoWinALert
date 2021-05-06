
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.WindowsAzure.Storage.Table;

namespace CoWinAlert.DTO
{
    public class Registration : TableEntity
    {
        #region Private Members
        private SearchMode _searchmode = SearchMode.Pin;
        private List<long> _pincodes = new List<long>();
        private int _yearofBirth = (DateTime.Now.Year - 45);
        private string _email = "";
        private string _phone = "";
        #endregion
        
        #region Public Members
        public string Name{get;set;}
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
                        _email = "-1";
                    }
                }
                catch{
                    _email = "-1";
                }
            }
        }
        public int Age{
            get{
                return DateTime.Now.Year - _yearofBirth;
            }
        }
        public string SearchByMode{
            get{
                return _searchmode.ToString();
            }
            set{
                try{
                    _searchmode = (SearchMode)Enum.Parse(typeof(SearchMode),value);
                }
                catch{
                    ;
                }
            }
        }
        public int YearofBirth{
            set{
                try{
                    if(Regex.IsMatch(value.ToString(),@"^19[0-9]{2}")){
                        _yearofBirth = value;
                    }
                }
                catch{
                    ;
                }        
            }
        }
        public List<string> PinCode{
            get{
                return _pincodes.Select(_code => _code.ToString()).ToList();
            }
            set{
                try{
                    _pincodes = value.Where(_code => 
                                        Regex.IsMatch(_code, @"^[0-9]{6}")
                                        )
                                        .Select(_codes => Int64.Parse(_codes))
                                        .ToList();
                }
                catch{
                    value = new List<string>();
                }
            }
        }
        public string Phone{
            get{
                return _phone;
            }
            set{
                try{
                    if(Regex.IsMatch(value.ToString(),@"^[0-9]{10}")){
                        _phone = value;
                    }
                }
                catch{
                    ;
                }        
            }
        }
        #endregion Public Members

        #region Public Functions
        // public Registration(){
        //     // Name Valid?
        //     if(! String.IsNullOrEmpty(Name)){
        //         _isValid = true;
        //     }

        //     // Email Valid ?
        //     if(! String.IsNullOrEmpty(_email)){
        //         _isValid = true;
        //     }

        //     // Age Valid ?

        // }

        
        #endregion Public Functions
    }
    public enum SearchMode{
        Pin,
        District
    }
}