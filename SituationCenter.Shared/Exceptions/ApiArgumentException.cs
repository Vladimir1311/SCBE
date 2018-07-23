using System;
namespace SituationCenter.Shared.Exceptions
{
    public class ApiArgumentException : ArgumentException
    {
        public ApiArgumentException(string message = null, string paramName = null, Exception ex = null) 
            : base(message, paramName, ex)
        {

        }
    }
}
