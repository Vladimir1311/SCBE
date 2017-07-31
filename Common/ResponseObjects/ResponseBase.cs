using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Common.ResponseObjects
{
    public class ResponseBase
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }
        [JsonProperty("status")]
        public StatusCode StatusCode { get; set; }
        [JsonProperty("errors")]
        public StatusCode[] Errors { get; set; }


        protected ResponseBase() { Success = true; StatusCode = StatusCode.OK; }

        protected ResponseBase(string message)
        {
            Success = false;
            Message = message;
        }
        protected ResponseBase(StatusCode[] statusCodes, string message = null)
        {
            switch (statusCodes.Length)
            {
                case 0:
                    StatusCode = StatusCode.UnknownError;
                    break;
                case 1:
                    StatusCode = statusCodes[0];
                    break;
                default:
                    StatusCode = StatusCode.ComplexError;
                    Errors = statusCodes;
                    break;
            }
        }

        public static ResponseBase BadResponse(string message, params StatusCode[] statusCodes) =>
            new ResponseBase(statusCodes, message);

        public static ResponseBase BadResponse(StatusCode statusCode, params StatusCode[] statusCodes)
        {
            return new ResponseBase(new StatusCode[] { statusCode}.Concat(statusCodes).ToArray());
        }
        public static ResponseBase GoodResponse() => new ResponseBase();
    }
}
