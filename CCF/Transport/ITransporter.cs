using CCF.Handle;
using System;
using System.Collections.Generic;
using System.Text;

namespace CCF.Transport
{
    interface ITransporter
    {
        event Action<InvokeMessage> OnReceiveMessge;
        event Action<InvokeResult> OnReceiveResult;
        event Action OnConnectionLost;
        void SendMessage(InvokeMessage result);
        void SendResult(InvokeResult result);
    }
}
