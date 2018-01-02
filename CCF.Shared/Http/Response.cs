using System;
using System.Collections.Generic;
using System.Text;

namespace CCF.Shared.Http
{
    public class Response
    {
        public bool Success { get; set; } = true;
        public string Password { get; set; }
        public int Port { get; set; }
    }
}
