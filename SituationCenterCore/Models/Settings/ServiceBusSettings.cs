using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterCore.Models.Settings
{
    public class ServiceBusSettings
    {
        public string ConnectionString { get; set; }
        public string FileServerQueueName { get; set; }
    }
}
