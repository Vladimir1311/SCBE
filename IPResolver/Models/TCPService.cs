using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IPResolver.Models
{
    public class TCPService : RemotePoint
    {
        public HashSet<TCPServiceUser> Listeners { get; set; } = new HashSet<TCPServiceUser>();


            new HashSet<(Guid packId, int serviceId, ManualResetEvent msEvent)>();
        private HashSet<(Guid packId, int serviceId, ManualResetEvent msEvent)> servicesIds =
        internal async Task<int> CreateInstanse()
        {
            var msEvent = new ManualResetEvent(false);
            var tuple = (packId: Guid.NewGuid(), messageId: -1, msEvent);
            servicesIds.Add(tuple);
            await SendMessage(17, tuple.packId, CCF.Shared.MessageType.ServiceCreateRequest, null);
            msEvent.WaitOne();
            return tuple.messageId;
        }


        internal void SetInstaceCreating(Guid id, int serviceId)
        {
            var tuple = servicesIds.FirstOrDefault(T => T.packId == id);
            if (tuple.msEvent == null) return;
            tuple.serviceId = serviceId;
            tuple.msEvent.Set();
        }
    }
}
