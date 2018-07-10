using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SituationCenterCore.Models.Settings;
using SituationCenterCore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SituationCenterCore.Services.Implementations
{
    public class ServiceBusFileServerNotifier : IFileServerNotifier
    {
        private readonly IQueueClient queueClient;

        public ServiceBusFileServerNotifier(IOptions<ServiceBusSettings> options)
        {
            queueClient = new QueueClient(options.Value.ConnectionString, options.Value.FileServerQueueName);
        }

        public async Task AddToken(Guid userId, string token)
            => await SendAscync(new {userId, token});

        public async Task SetRoom(Guid userId, Guid? roomId)
            => await SendAscync(new {userId, roomId});

        private Task SendAscync(object body)
        {
            var text = JsonConvert.SerializeObject(body);
            var bytes = Encoding.UTF8.GetBytes(text);
            var message = new Message(bytes);
            return queueClient.SendAsync(message);
        }
    }
}
