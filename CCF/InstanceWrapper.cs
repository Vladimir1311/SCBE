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

        private long lastId;
        private readonly long id;
        private ConcurrentDictionary<long, InstanceWrapper> subWrappers;
        private readonly CodeInvoker invoker;
        private readonly ITransporter transporter;

        public InstanceWrapper(CodeInvoker invoker, ITransporter transporter)
            : this(invoker, 0, new ConcurrentDictionary<long, InstanceWrapper>())
        {
            subWrappers[0] = this;
            transporter.OnReceiveMessge += RecieveMessage;
        }

        private InstanceWrapper(CodeInvoker invoker, long id, ConcurrentDictionary<long, InstanceWrapper> wrappersCollection)
        {
            this.invoker = invoker ?? throw new ArgumentNullException(nameof(invoker));
            subWrappers = wrappersCollection;
            this.id = id;
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
                data = invoker.Invoke(message);
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
                subWrappers[newId] = new InstanceWrapper(codeInvoker, newId, subWrappers);
            }
            return InvokeResult.HardObject(message.Id, newId);
        }
    }
}
