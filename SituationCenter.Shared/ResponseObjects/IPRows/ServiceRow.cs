using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SituationCenter.Shared.ResponseObjects.IPRows
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
            StringIP = address.MapToIPv4().ToString();
            ServiceType = serviceType;
        }
    }
}
