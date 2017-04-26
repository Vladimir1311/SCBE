using System;
using System.Collections.Generic;
using System.Text;

namespace UDPTester
{
    public interface IRemoteWorker
    {
        int Value(double arg);
    }
}
