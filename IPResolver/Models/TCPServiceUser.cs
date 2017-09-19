using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace IPResolver.Models
{
    public class TCPServiceUser : IPingable
    {
        public string Password { get; set; }
        public string InterfaceName { get; set; }
        public TcpClient Connection { get; set; }
        public HashSet<Guid> WaitedPacks { get; set; } = new HashSet<Guid>();
        public DateTime ConnectionTime { get; set; }
        public DateTime LastPing { get; set; }
    }
}