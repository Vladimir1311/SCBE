using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace IPResolver.Models
{
    public class ServiceRow
    {
        public Guid Id { get; set; }
        public string ServiceType { get; set; }
        public string StringIP { get; set; }
        public ServiceRow()
        {}
        public ServiceRow(IPAddress address, string serviceType)
        {
            IP = address;
            ServiceType = serviceType;
        }

        [NotMapped]
        public IPAddress IP {

            get => IPAddress.Parse(StringIP);
            set => StringIP = value.MapToIPv4().ToString();
        }
    }
}
