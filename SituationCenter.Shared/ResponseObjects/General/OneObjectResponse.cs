using System;
using System.Collections.Generic;
using System.Text;

namespace SituationCenter.Shared.ResponseObjects.General
{
    public class OneObjectResponse<T> : ResponseBase
    {
        public T Data { get; set; }
        public static implicit operator OneObjectResponse<T>(T value)
            => new OneObjectResponse<T> { Data = value };
    }
}
