using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CCF.Extensions
{
    internal static class StreamExtensions
    {
        public static async Task CopyPart(this Stream stream, Stream target, int length)
        {
            byte[] buffer = new byte[4096];
            while(length > 0)
            {
                var readed = await stream.ReadAsync(buffer, 0, Math.Min(length, buffer.Length));
                await target.WriteAsync(buffer, 0, readed);
                length -= readed;
            }
        }
    }
}
