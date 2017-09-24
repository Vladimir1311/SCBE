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

        private class CreateInstanceSet
        {
            public Guid PackId { get; set; }
            public int ServiceId { get; set; }
            public ManualResetEvent MsEvent { get; set; }
        }
        private HashSet<CreateInstanceSet> servicesIds =
            new HashSet<CreateInstanceSet>();
        internal async Task<int> CreateInstanse()
        {
            var msEvent = new ManualResetEvent(false);
            var set = new CreateInstanceSet { PackId = Guid.NewGuid(), ServiceId = -1, MsEvent = msEvent };
            servicesIds.Add(set);
            await SendMessage(17, set.PackId, CCF.Shared.MessageType.ServiceCreateRequest, null);
            msEvent.WaitOne();
            return set.ServiceId;
        }


        internal void SetInstaceCreating(Guid id, int serviceId)
        {
            var tuple = servicesIds.FirstOrDefault(T => T.PackId == id);
            if (tuple == null) return;
            tuple.ServiceId= serviceId;
            tuple.MsEvent.Set();
        }
    }
}
