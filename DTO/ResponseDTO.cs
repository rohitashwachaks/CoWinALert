using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace CoWinAlert.DTO
{
    public class VaccineFeeDTO
    {
        public string vaccine{get;set;}
        public string fee{get;set;}
    }
    public class SessionDTO{
        private DateTime _date = DateTime.Now;
        private Vaccine _vaccine = DTO.Vaccine.ANY;
        public string Session_id{get; set;}
        
        public DateTime SessionDate{
            get
            {
                return _date;
            }
        }
        public string Date
        {            
            set
            {
                _date = DateTime.ParseExact(value, "mm-dd-yyyy", CultureInfo.InvariantCulture);
            }
        }
        public int Available_capacity{get; set;}
        public int Min_age_limit{get; set;}
        public string Vaccine
        {
            get
            {
                return _vaccine.ToString();
            } 
            set
            {
                try
                {
                    _vaccine = (Vaccine)Enum.Parse(typeof(Vaccine), value.ToLower());
                }
                catch
                {
                    ;
                }
            }
        }
        public List<string> Slots{get; set;}
    }
    public class SessionCalendarDTO
    {
        public string Center_id{get;set;}
        public string Name{get;set;}
        public string Address{get; set;}
        public string State_name{get; set;}
        public string District_name{get; set;}
        public string Block_name{get; set;}
        public long Pincode{get; set;}
        public double Lat{get; set;}
        public double Long{get; set;}
        public DateTime From{get; set;}
        public DateTime To{get; set;}
        public FeeTypeDTO Fee_type{get; set;}
        public List<VaccineFeeDTO> Vaccine_fees{get; set;}
        public List<SessionDTO> Sessions{get; set;}
    }
    public class CentersDTO
    {
        public List<SessionCalendarDTO> Centers{get; set;}
    }
    }
/**
{
  "sessions": [
    {
      "center_id": 1234,
      "name": "District General Hostpital",
      "name_l": "",
      "address": "45 M G Road",
      "address_l": "",
      "state_name": "Maharashtra",
      "state_name_l": "",
      "district_name": "Satara",
      "district_name_l": "",
      "block_name": "Jaoli",
      "block_name_l": "",
      "pincode": "413608",
      "lat": 28.7,
      "long": 77.1,
      "from": "09:00:00",
      "to": "18:00:00",
      "fee_type": "Paid",
      "fee": "250",
      "session_id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "date": "31-05-2021",
      "available_capacity": 50,
      "min_age_limit": 18,
      "vaccine": "COVISHIELD",
      "slots": [
        "FORENOON",
        "AFTERNOON"
      ]
    }
  ]
}



CALENDER CENTER
{
  "centers": [
    {
      "center_id": 1234,
      "name": "District General Hostpital",
      "name_l": "",
      "address": "45 M G Road",
      "address_l": "",
      "state_name": "Maharashtra",
      "state_name_l": "",
      "district_name": "Satara",
      "district_name_l": "",
      "block_name": "Jaoli",
      "block_name_l": "",
      "pincode": "413608",
      "lat": 28.7,
      "long": 77.1,
      "from": "09:00:00",
      "to": "18:00:00",
      "fee_type": "Free",
      "vaccine_fees": [
        {
          "vaccine": "COVISHIELD",
          "fee": "250"
        }
      ],
      "sessions": [
        {
          "session_id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
          "date": "31-05-2021",
          "available_capacity": 50,
          "min_age_limit": 18,
          "vaccine": "COVISHIELD",
          "slots": [
            "FORENOON",
            "AFTERNOON"
          ]
        }
      ]
    }
  ]
}


**/
