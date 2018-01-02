using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IPResolver.Models.Points
{
    public class PointsLinker
    {
        private RemotePoint first;
        private RemotePoint second;
        private readonly ILogger<PointsLinker> logger;
        private CancellationTokenSource tokenSorce;

        private Task readFirst;
        private Task readSecond;

        private static long count;
        private long id = count++;

        public PointsLinker(RemotePoint first, RemotePoint second, ILogger<PointsLinker> logger)
        {
            this.first = first;
            this.second = second;
            this.logger = logger;
            tokenSorce = new CancellationTokenSource();
        }

        public async Task StartConnection()
        {
            readFirst = Task.Factory.StartNew(async () => await SetPipe(first, second));
            readSecond = Task.Factory.StartNew(async () => await SetPipe(second, first));
            logger.LogTrace($"{id} started tasks");
            await Task.WhenAll(readFirst, readSecond);
            logger.LogTrace($"{id} tasks ended");
            first.Dispose();
            second.Dispose();
            logger.LogTrace($"{id} done work");
        }


        private async Task SetPipe(RemotePoint from, RemotePoint to)
        {
            while (true)
            {
                var data = from.ReadMessage();
                await to.SendMessage(
                    data.length,
                    data.packId,
                    data.messageType,
                    data.data);
            }
        }
    }
}
