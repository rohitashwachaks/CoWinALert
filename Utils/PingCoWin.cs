// calendarByPin
// https://cdn-api.co-vin.in/api/v2/appointment/sessions/public/calendarByPin?pincode=110001&date=30-03-2021

// calendarByDistrict
// https://cdn-api.co-vin.in/api/v2/appointment/sessions/public/calendarByDistrict?district_id=512&date=31-03-2021

// FindByDistrict
// https://cdn-api.co-vin.in/api/v2/appointment/sessions/public/findByDistrict?district_id=512&date=31-03-2021

// FindByPin
// https://cdn-api.co-vin.in/api/v2/appointment/sessions/public/findByPin?pincode=110001&date=31-03-2021

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CoWinAlert.Utils
{
    public static class PingCoWin
    {
        private static HttpClient client = new HttpClient(){ Timeout = TimeSpan.FromSeconds(120)};
        public static void Initialise(){            
            client.DefaultRequestHeaders.Accept.Clear();
        }
        #region Structure URLS
        // private static string findByDistrict = "https://cdn-api.co-vin.in/api/v2/appointment/sessions/public/findByDistrict?";
        // private static string findByPin = "https://cdn-api.co-vin.in/api/v2/appointment/sessions/public/findByPin?";
        // private static string calendarByDistrict = "https://cdn-api.co-vin.in/api/v2/appointment/sessions/public/calendarByDistrict?";
        // private static string calendarByPin = "https://cdn-api.co-vin.in/api/v2/appointment/sessions/public/calendarByPin?";
        public static Task<HttpResponseMessage> FindByDistrict(int district_id, DateTime date)
        {
            string url = PingAction.FindByDistrict.ToDescriptionString()+$"district_id={district_id.ToString()}&date={date.ToShortDateString()}";
            return client.GetAsync(url);
        }
        public static Task<HttpResponseMessage> FindByPin(long pincode, DateTime date)
        {
            string url = PingAction.FindByPin.ToDescriptionString()+$"pincode={pincode.ToString()}&date={date.ToShortDateString()}";
            return client.GetAsync(url);
        }
        public static Task<HttpResponseMessage> CalendarByDistrict(int district_id, DateTime date)
        {
            string url = PingAction.CalendarByDistrict.ToDescriptionString()+$"district_id={district_id.ToString()}&date={date.ToShortDateString()}";
            return client.GetAsync(url);
        }
        public static Task<HttpResponseMessage> CalendarByPin(long pincode, DateTime date)
        {
            string url = PingAction.CalendarByPin.ToDescriptionString()+$"pincode={pincode.ToString()}&date={date.ToShortDateString()}";
            return client.GetAsync(url);
        }
        #endregion Structure URLS
        #region Async Calls
        public static async Task<IEnumerable<HttpResponseMessage>> GetResultAsync(
                                            // HttpClient client,
                                            // PingAction actionName, 
                                            List<DateTime> dateTimes,
                                            List<long> pincodes,
                                            List<int> district_id
                                        )
        {
            IEnumerable<Task<HttpResponseMessage>> lstResponse = new List<Task<HttpResponseMessage>>();
            foreach(DateTime date in dateTimes){
                
                IEnumerable<Task<HttpResponseMessage>> lstPin = from param in pincodes select FindByPin(param, date);
                IEnumerable<Task<HttpResponseMessage>> lstDist = from param in district_id select FindByDistrict(param, date);

                lstResponse.Concat(lstPin);
                lstResponse.Concat(lstDist);
                // foreach(long code in pincodes){
                //     lstResponse.Append(FindByPin(code, date));
                // }
                // foreach(int id in district_id){
                //     lstResponse.Append(FindByDistrict(id, date));
                // }
            }
            int x = lstResponse.Count();
            IEnumerable<HttpResponseMessage> responses = await Task.WhenAll(lstResponse);

            return responses;
        }
        #endregion Async Calls
        public static string ToDescriptionString(this PingAction val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
            .GetType()
            .GetField(val.ToString())
            .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
    public enum PingAction{
        [Description("https://cdn-api.co-vin.in/api/v2/appointment/sessions/public/findByDistrict?")]
        FindByDistrict,
        [Description("https://cdn-api.co-vin.in/api/v2/appointment/sessions/public/findByPin?")]
        FindByPin,
        [Description("https://cdn-api.co-vin.in/api/v2/appointment/sessions/public/calendarByDistrict?")]
        CalendarByDistrict,
        [Description("https://cdn-api.co-vin.in/api/v2/appointment/sessions/public/calendarByPin?")]
        CalendarByPin
    }
}