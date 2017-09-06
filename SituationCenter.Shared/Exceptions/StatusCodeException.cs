using System;

namespace SituationCenter.Shared.Exceptions
{
    public class StatusCodeException : Exception
    {
        public StatusCode StatusCode { get; set; }
        public StatusCodeException(StatusCode statusCode) =>
            StatusCode = statusCode;
    }
}
