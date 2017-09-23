using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SituationCenter.Shared.Exceptions
{
    public class MultiStatusCodeException : Exception
    {
        public StatusCode[] Codes;
        public MultiStatusCodeException(IEnumerable<StatusCode> codes) =>
            Codes = codes.ToArray();
    }
}
