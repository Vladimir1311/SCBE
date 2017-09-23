using Microsoft.Extensions.Logging;
using SituationCenterBackServer.Extensions;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Logging
{
    public class SocketLoggerProvider : ILoggerProvider
    {
        private readonly Func<string, LogLevel, bool> _filter;

        private static LinkedList<WebSocket> sockets;

        public SocketLoggerProvider(Func<string, LogLevel, bool> filter = null)
        {
            sockets = new LinkedList<WebSocket>();
            _filter = filter;
        }

        private void LogToAll(string message)
        {
            lock (sockets)
            {
                foreach (var socket in sockets)
                {
                    socket.SendAsync(message).Wait();
                }
            }
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new SocketLogger(categoryName, _filter, LogToAll);
        }

        internal static async Task AddSocketAsync(WebSocket socket)
        {
            lock (sockets)
            {
                sockets.AddLast(socket);
            }
            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    var message = await socket.GetMessage();
                    await socket.SendAsync(message);
                }
            }
            finally
            {
                lock (sockets)
                {
                    sockets.Remove(socket);
                }
            }
        }

        public void Dispose()
        {
        }
    }
}