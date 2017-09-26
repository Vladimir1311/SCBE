using Castle.DynamicProxy;
using CCF;
using System;
using System.Dynamic;
using System.Reflection;
using System.Reflection.Emit;

namespace UDPTester
{
    public class Program
    {
        private static void Main(string[] args)
        {

            var newType = AssemblyBuilder
                .DefineDynamicAssembly(new AssemblyName("Some"), AssemblyBuilderAccess.Run)
                .DefineDynamicModule("Some.dll")
                .DefineType("MYBeutyInterface", TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract);
            newType.AddInterfaceImplementation(typeof(ILOL));
            newType.AddInterfaceImplementation(typeof(IDisposable));
            var generated = newType.CreateType();
            var invoker = new ProxyGenerator().CreateInterfaceProxyWithoutTarget(generated, new Ceptor()) as ILOL;
            var length = invoker.StrLength("HA ha ha");
            var disposable = invoker as IDisposable;
            disposable.Dispose();
            Console.WriteLine();
        }
    }
    class LOLERPROXY : DynamicObject
    {

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            result = this;
            return true;
            return base.TryConvert(binder, out result); 
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            Console.WriteLine(binder.Name);
            Action func = () => { Console.WriteLine("Dispose func"); };
            result = func;
            return true;
        }
    }


    public class Ceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine(invocation.Method.Name);
            invocation.ReturnValue = -1;
        }
    }

    class lol : ILOL, IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public int StrLength(string str) => str.Length;
    }

    

    public interface ILOL : IDisposable
    {
        int StrLength(string str);
    }
}