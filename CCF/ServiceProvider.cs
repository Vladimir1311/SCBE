using CCF.Messages;
using CCF.Transport;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CCF
{
    internal class ServiceProvider<T>
    {
        private readonly Func<T> serviceInvoker;
        private readonly ITransporter transporter;

        public ServiceProvider(Func<T> serviceInvoker, ITransporter transporter)
        {
            this.serviceInvoker = serviceInvoker;
            this.transporter = transporter;
            transporter.OnReceiveMessge += Transporter_OnReceiveMessge;
        }

        private async Task Transporter_OnReceiveMessge(InvokeMessage arg)
        {
            await Task.CompletedTask;
            var service = serviceInvoker();

        }
    }
}
