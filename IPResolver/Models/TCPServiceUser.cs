using System;
using System.Collections.Generic;

namespace IPResolver.Models
{
    public class TCPServiceUser : RemotePoint
    {
        public HashSet<Guid> WaitedPacks { get; set; } = new HashSet<Guid>();
    }
}