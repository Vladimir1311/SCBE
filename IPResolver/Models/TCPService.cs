using IPResolver.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace IPResolver.Models
{
    public class TCPService : CCFService
    {
        public string Password { get; set; }
        public TcpClient Connection { get; set; }
        public HashSet<TCPServiceUser> Listeners { get; set; } = new HashSet<TCPServiceUser>();
    }
}
