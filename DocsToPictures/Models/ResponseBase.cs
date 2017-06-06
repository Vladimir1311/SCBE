using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace Common.ResponseObjects
{
    public class ResponseBase : ActionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Object { get; set; }
        public ResponseBase(bool success)
        {
            Success = success;
        }
        public ResponseBase(bool success, string message) : this(success)
        {
            Message = message;
        }
        public ResponseBase(bool success, object data) : this(success)
        {
            Object = data;
        }
        public static ResponseBase BadResponse(string message)
        {
            return new ResponseBase(false, message);
        }

        public static ResponseBase GoodResponse()
        {
            return new ResponseBase(true);
        }
        public static ResponseBase GoodResponse(object data)
        {
            return new ResponseBase(true, data);
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "application/json";
            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            context.HttpContext.Response.Write(JsonConvert.SerializeObject(this, Formatting.Indented, jsonSettings));
        }
    }
}
