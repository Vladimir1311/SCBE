using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IPResolver.Extensions
{
    public static class StreamExtensions
    {
        public static void Write(this Stream stream, byte[] array)
        {
            stream.Write(array, 0, array.Length);
        }

        public static async Task CopyPart(this Stream stream, Stream target, int length)
        {
            byte[] buffer = new byte[Math.Min(length, 4096)];
            while (length > 0)
            {
                var readed = await stream.ReadAsync(buffer, 0, Math.Min(length, buffer.Length));
                await target.WriteAsync(buffer, 0, readed);
                length -= readed;
            }
        }
    }
}
