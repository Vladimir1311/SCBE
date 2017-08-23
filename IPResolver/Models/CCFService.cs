using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPResolver.Models
{
    public class CCFService
    {
        public Guid Id { get; set; }
        public string InterfaceName { get; set; }
        public string CCFEndPoint { get; set; }
    }
}
