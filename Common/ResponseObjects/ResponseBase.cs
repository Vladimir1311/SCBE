using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ResponseObjects
{
    public class ResponseBase
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Object { get; set; }

        public ResponseBase() {}
        public ResponseBase(bool success) =>
            Success = success;
        
        public ResponseBase(bool success, string message) : this(success) =>
            Message = message;
        
        public static ResponseBase BadResponse(string message) =>
            new ResponseBase(false, message);

        public static ResponseBase GoodResponse() => new ResponseBase(true);
    }
}
