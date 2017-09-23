using System.Collections.Generic;

namespace IPResolver.Models
{
    public class TCPService : RemotePoint
    {
        public HashSet<TCPServiceUser> Listeners { get; set; } = new HashSet<TCPServiceUser>();
    }
}
