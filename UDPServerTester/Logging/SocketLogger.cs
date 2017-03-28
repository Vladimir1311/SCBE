using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UDPServerTester.Logging
{
    public class SocketLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly Func<string, LogLevel, bool> _filter;
        private readonly Action<string> _toDo;

        public SocketLogger(string categoryName, Func<string, LogLevel, bool> filter, Action<string> toDo)
        {
            _categoryName = categoryName;
            _filter = filter;
            _toDo = toDo;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return new NoopDisposable();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return (_filter?.Invoke(_categoryName, logLevel) ?? true);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            if (formatter == null) throw new ArgumentNullException(nameof(formatter));

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message)) return;

            _toDo(JsonConvert.SerializeObject(
                new
                {
                    LogLevel = logLevel.ToString(),
                    EventId = eventId,
                    Message = message,
                    CategoryName = _categoryName
                }));
        }
        private class NoopDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}
