using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IPResolver.Extensions
{
    public static class MemoryStreamExtensions
    {
        public static void Write(this MemoryStream stream, byte[] array)
        {
            stream.Write(array, 0, array.Length);
        }
    }
}
