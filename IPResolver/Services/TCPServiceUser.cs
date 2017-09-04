using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace IPResolver.Services
{
    public class TCPServiceUser
    {
        public string Password { get; set; }
        public string InterfaceName { get; set; }
        public TcpClient Connection { get; set; }
        public HashSet<Guid> WaitedPacks { get; set; } = new HashSet<Guid>();
    }
}