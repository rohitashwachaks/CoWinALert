
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
        private SearchMode _searchmode;
        private List<int> _pincodes;
        private int _yearofBirth;
        private string _email;
        #endregion
        
        #region Public Members
        public string Name{get;set;}
        public string EmailID{
            get{
                return _email;
            }
            set{
                try{
                    if(Regex.IsMatch(value,@"*@*.*")){
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
        public int Age{get; set;}
        public string SearchByMode{
            get{
                return _searchmode.ToString();
            }
            set{
                try{
                    _searchmode = (SearchMode)Enum.Parse(typeof(SearchMode),value);
                }
                catch{
                    _searchmode = SearchMode.Pin;
                }
            }
        }
        public int YearofBirth{
            get{
                return _yearofBirth;
            }
            set{
                try{
                    if(value <= (DateTime.Now.Year - 45)){
                        Age = 45;
                    }
                    else if(value <= (DateTime.Now.Year - 18) 
                            && value > (DateTime.Now.Year - 45)){
                        Age = 18;
                    }
                    else{
                        Age = -1;
                    }
                }
                catch{
                    Age = 45;
                }
                _yearofBirth = value;         
            }
        }
        public List<int> PinCode{
            get{
                return _pincodes;
            }
            set{
                try{
                    _pincodes = value.Where(_code => 
                                    Regex.IsMatch(_code.ToString(), @"[0-9]^6")
                                )
                                .ToList();
                }
                catch{
                    value = new List<int>();
                }
            }
        }
        #endregion Public Members
    }
    public enum SearchMode{
        Pin,
        District
    }
}