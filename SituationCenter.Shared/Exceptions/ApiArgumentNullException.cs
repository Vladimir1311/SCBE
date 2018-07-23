using System;
namespace SituationCenter.Shared.Exceptions
{
    public class ApiArgumentNullException : ArgumentNullException
    {
        public ApiArgumentNullException(string paramName) : base(paramName: paramName)
        {
        }
    }
}
