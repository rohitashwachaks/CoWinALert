using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace CoWinAlert.Utils
{
    public static class HttpResponseHandler{
        public static HttpResponseMessage StructureResponse(string reason = null,
                                                            object content = null,
                                                            HttpStatusCode code = HttpStatusCode.OK
                                                            ){
            if(String.IsNullOrEmpty(reason)){
                reason = code.ToString();
            }
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            responseMessage.ReasonPhrase = reason;
            responseMessage.Content = new StringContent(JsonConvert.SerializeObject(content));
            responseMessage.StatusCode = code;
            return responseMessage;
        }
    }
}