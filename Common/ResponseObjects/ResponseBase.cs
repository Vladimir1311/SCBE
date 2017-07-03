using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ResponseObjects
{
    public class ResponseBase
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        protected ResponseBase() { Success = true; }

        protected ResponseBase(string message)
        {
            Success = false;
            Message = message;
        }

        public static ResponseBase BadResponse(string message) =>
            new ResponseBase(message);
        public static ResponseBase BadResponse(string message, ILogger logger)
        {
            logger.LogWarning(message);
            return BadResponse(message);
        }

        public static ResponseBase GoodResponse() => new ResponseBase();
    }
}
