using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SituationCenter.Shared.ResponseObjects.General
{
    public class ListResponse<T> : ResponseBase
    {
        public List<T> Data { get; set; }
        public static implicit operator ListResponse<T>(List<T> value)
            => new ListResponse<T> { Data = value };
    }
}
