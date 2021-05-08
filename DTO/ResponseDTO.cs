using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CoWinAlert.DTO
{
    public class VaccineFeeDTO
    {
        public static string vaccine{get;set;}
        public static string fee{get;set;}
    }
    public class SessionDTO{
        private static Vaccine _vaccine = DTO.Vaccine.ANY;
        public static string Session_id{get; set;}
        public static DateTime Date{get; set;}
        public static int Available_capacity{get; set;}
        public static int Min_age_limit{get; set;}
        public static string Vaccine
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
        public static List<string> Slots{get; set;}
    }
    public class SessionCalendarDTO
    {
        public static string Center_id{get;set;}
        public static string Name{get;set;}
        public static string Address{get; set;}
        public static string State_name{get; set;}
        public static string District_name{get; set;}
        public static string Block_name{get; set;}
        public static long Pincode{get; set;}
        public static double Lat{get; set;}
        public static double Long{get; set;}
        public static DateTimeOffset From{get; set;}
        public static DateTimeOffset To{get; set;}
        public static FeeTypeDTO Fee_type{get; set;}
        public static List<VaccineFeeDTO> Vaccine_fees{get; set;}
        public static List<SessionDTO> Sessions{get; set;}
    }
    public class ResponseDTO
    {

    }
    public enum FeeTypeDTO
    {
        FREE,
        PAID
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
