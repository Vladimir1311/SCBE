using System;
using System.Collections.Generic;
using System.Text;

namespace UDPTester
{
    public class RemoteWorker : IRemoteWorker
    {
        public int Value(double arg)
        {
            Console.WriteLine($"i write {arg}");
            return (int)arg;
        }
    }
}
