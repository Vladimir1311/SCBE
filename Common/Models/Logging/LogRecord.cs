using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models.Logging
{
    public class LogRecord
    {

        public LogLevel LogLevel {get; set;}
        public EventId EventId { get; set; }
        public string Message { get; set; }
        public string CategoryName { get; set; }
    }
}
