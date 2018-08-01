using System.Linq;
using System.Threading.Tasks;
using SituationCenter.NotifyHub.Services.Interfaces;
using SituationCenter.NotifyProtocol;

namespace SituationCenter.NotifyHub.Services.Implementations
{
	public class WebSocketNotificator : INotificator
    {
        private readonly IWebSocketManager webSocketManager;

        public WebSocketNotificator(IWebSocketManager webSocketManager)
        {
            this.webSocketManager = webSocketManager;
        }

        public Task Notify<T>(string topic, T data) where T: class
        {
            return Task.WhenAll(webSocketManager
                .ForTopic(topic)
                .Select(wsh => wsh.Send(topic, data)));
        }
    }
}
