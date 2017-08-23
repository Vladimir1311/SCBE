using Common.ResponseObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Exceptions
{
    public class StatusCodeException : Exception
    {
        public StatusCode StatusCode { get; set; }
        public StatusCodeException(StatusCode statusCode) =>
            StatusCode = statusCode;
    }
}
