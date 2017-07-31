using Common.ResponseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Exceptions
{
    public class MultiStatusCodeException : Exception
    {
        public StatusCode[] Codes;
        public MultiStatusCodeException(IEnumerable<StatusCode> codes) =>
            Codes = codes.ToArray();
    }
}
