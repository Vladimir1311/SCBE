using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPResolver.Models.Common
{
    public class UniqueID
    {
        private static long lastId = 0;
        public long Id => lastId++;
    }
}
