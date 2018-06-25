using CCF.Messages;
using CCF.Transport;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCF
{
    class InstanceWrapper
    {
        private static Type[] PrimitiveTypes =
            typeof(JToken)
            .GetMethods()
            .Where(M => M.Name == "op_Implicit")
            .Select(M => M.GetParameters()[0].ParameterType)
            .ToArray();

        private static long lastId;
        private readonly long id = lastId++;
        private static ConcurrentDictionary<long, InstanceWrapper> subWrappers =
            new ConcurrentDictionary<long, InstanceWrapper>();
        private readonly CodeInvoker invoker;
        private readonly ITransporter transporter;

        public long Id => id;

        public InstanceWrapper(CodeInvoker invoker, ITransporter transporter)
            : this(invoker)
        {
            transporter.OnReceiveMessge += RecieveMessage;
            this.transporter = transporter;
        }

        private InstanceWrapper(CodeInvoker invoker)
        {
            this.invoker = invoker ?? throw new ArgumentNullException(nameof(invoker));
            subWrappers[id] = this;
        }

        private async Task RecieveMessage(InvokeMessage arg)
        {
            var target = subWrappers[arg.SubObjectId];
            await transporter.SendResult(target.GetInvokationResult(arg));
        }

        private InvokeResult GetInvokationResult(InvokeMessage message)
        {
            object data;
            try
            {
                data = invoker.Invoke(message, CreateRemoteInvoker);
            }
            catch (Exception ex)
            {
                return InvokeResult.Exception(message.Id, ex);
            }
            if (data == null)
                return InvokeResult.Null(message.Id);
            if (data is Stream stream)
                return InvokeResult.Stream(message.Id, stream);
            if (PrimitiveTypes.Contains(data.GetType()))
                return InvokeResult.Primitive(message.Id, JToken.FromObject(data));

            var codeInvoker = new CodeInvoker(data, data.GetType());
            long newId;
            //TODO lock need?
            lock (subWrappers)
            {
                newId = ++lastId;
                subWrappers[newId] = new InstanceWrapper(codeInvoker);
            }
            return InvokeResult.HardObject(message.Id, newId);
        }

        private object CreateRemoteInvoker(long invokerId, Type invokerType)
        {
            return RemoteWorker.Create(invokerType, transporter, invokerId);
        }
    }
}
