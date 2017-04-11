﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Extensions
{
    public static class TCPClientExtensions
    {
        public static async Task<int> ReadPacketAsync(this TcpClient client, byte[] buffer, int offset, int size, CancellationToken token)
        {
            byte[] header = new byte[2];
            int readed = await client.GetStream().ReadAsync(header, 0, 2, token)-2;
            int must = ParseHeader(header[0], header[1]);
            while (readed != must)
            {
                readed += await client.GetStream().ReadAsync(buffer, readed, must - readed, token);
            }
            return readed;
        }
        private static int ParseHeader(byte first, byte second)
        {
            return ((first << 8) | second);
        }
    }


}
