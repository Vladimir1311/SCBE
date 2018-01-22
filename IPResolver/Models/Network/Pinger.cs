using CCF.Shared;
using IPResolver.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IPResolver.Models.Network
{
    public class Pinger
    {
        private readonly Func<long, Guid, MessageType, Stream, Task> sendMessage;
        private readonly ILogger<Pinger> logger;
        private Dictionary<Guid, DateTime> pingDates = new Dictionary<Guid, DateTime>();

        public DateTime LastPing { get; private set; }
        public TimeSpan PingTime { get; private set; }

        public Pinger(Func<long, Guid, MessageType, Stream, Task> sendMessage, ILogger<Pinger> logger)
        {
            this.sendMessage = sendMessage ?? throw new ArgumentNullException(nameof(sendMessage));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendPing()
        {
            
            var pair = pingDates.AddPair(Guid.NewGuid(), DateTime.Now);
            await sendMessage(17, pair.Key, MessageType.PingRequest, null);
        }

        public void SetPing(Guid pingId)
        {

            try
            {
                if(!pingDates.TryGetValue(pingId, out var pair))
                {
                    logger.LogWarning($"try setting strange ping response");
                    return;
                }
                var now = DateTime.Now;
                LastPing = now;
                PingTime = now - pair;
                pingDates.Remove(pingId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while settingPing");
            }

        }
    }
}
