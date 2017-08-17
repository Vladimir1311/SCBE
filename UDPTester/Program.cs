using Castle.DynamicProxy;

namespace UDPTester
{
    public class Program
    {
        private static void Main(string[] args)
        {
            ProxyGenerator generator = new ProxyGenerator();
            ILOL t = (ILOL)generator.CreateInterfaceProxyWithoutTarget(typeof(ILOL), new Worker());
            t.StrLength("sdfsdfsgsrg");
        }
    }

    internal class Worker : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.ReturnValue = "sdfsfe";
        }
    }

    public interface ILOL
    {
        int StrLength(string str);
    }
}