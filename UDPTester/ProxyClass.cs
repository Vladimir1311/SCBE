using Castle.DynamicProxy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace UDPTester
{
    class ProxyClass : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            
            Console.WriteLine($"Try to Call {invocation.Method.Name}");            
            invocation.ReturnValue = 4;
        }
    }
}
