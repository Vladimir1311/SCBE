using System;
using System.Threading.Tasks;

namespace CCF.Transport
{
    interface ITransporter : IDisposable
    {
        event Func<InvokeMessage, Task> OnReceiveMessge;
        event Func<InvokeResult, Task> OnReceiveResult;
        event Func<Guid, Task> OnNeedNewService;
        event Action OnConnectionLost;
        Task SendMessage(InvokeMessage result);
        Task SendResult(InvokeResult result);
        Task SetupNewService(Guid id, int serviceId);
    }
}
