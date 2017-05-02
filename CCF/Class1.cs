using Castle.DynamicProxy;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CCF
{
    public class GlobalProxy: IInterceptor
    {
        private static IProxyGenerator _proxyGenerator = new ProxyGenerator();

        private IConnector _connector = new SampleConnector();

        public static T CreateFor<T>()
        {
            return (T)_proxyGenerator.CreateInterfaceProxyWithoutTarget(
                typeof(T),
                new GlobalProxy());
            
        }



        public void Intercept(IInvocation invocation)
        {
            _connector.Send(invocation.Method.Name);
            string returned = null;
            using (var mre = new ManualResetEvent(false))
            {
                Action<string> handler = null;
                handler = value =>
                {
                    _connector.Received -= handler;
                    returned = value;
                    mre.Set();
                };
                _connector.Received += handler;
                mre.WaitOne();
            }
            invocation.ReturnValue = returned;
        }
    }


    class SampleConnector : IConnector
    {
        public event Action<string> Received;

        public void Send(string val)
        {
            
            Console.WriteLine("Sendong " + val);
            Task.Factory.StartNew(() => Task.Delay(5000).ContinueWith(T => Received("Wow! I receive value!")));
        }
    }
}
