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
        private readonly Func<T> instanceCreater;
        private readonly Func<string, ITransporter> transporterCreator;
        private readonly ITransporter transporter;

        public ServiceProvider(Func<T> serviceInvoker, string password, Func<string, ITransporter> transporterCreator)
        {
            instanceCreater = serviceInvoker ?? throw new ArgumentNullException(nameof(serviceInvoker));
            this.transporterCreator = transporterCreator ?? throw new ArgumentNullException(nameof(transporterCreator));
            transporter.OnNeedNewInstance += NeedNewInstance;
            transporter = transporterCreator(password);
        }

        private Task NeedNewInstance(string password)
        {
            var instanceTransporter = transporterCreator(password);
            var instance = instanceCreater();
            var codeInvoker = new CodeInvoker(instance, typeof(T));
            InstanceWrapper rootWrapper = new InstanceWrapper(codeInvoker, instanceTransporter);
            return Task.CompletedTask;
        }
    }
}
