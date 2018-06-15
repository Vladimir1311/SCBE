using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace DocsToPictures.NETFrameworkWEB.Models
{
    public class Logger<T>
    {
        private string LogFilePath => ConfigurationManager.AppSettings["LogFilePath"];
        private readonly static object locker = new object();

        public void Info(string message)
            => WriteLine(LogLevel.Info, message);

        private void WriteLine(LogLevel level, string content)
        {

            lock (locker)
            {
                while (true)
                {
                    try
                    {
                        File.AppendAllText(LogFilePath, $"{DateTime.Now} {{{typeof(T).FullName}}} |{level}|: {content}\n");
                        break;
                    }
                    catch {}

                }
            }
        }
        private enum LogLevel
        {
            Trace,
            Debug,
            Info,
            Warn,
            Critical
        }

        internal void Warning(string v, Exception ex = null)
            => WriteLine(LogLevel.Warn, $"{v} : {ex?.Message}\n{ex?.StackTrace}");

        internal void Debug(string v)
            => WriteLine(LogLevel.Trace, v);

        internal void Trace(string v)
            => WriteLine(LogLevel.Trace, v);
    }
}