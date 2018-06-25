using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UDPTester
{
    public interface IRemoteWorker
    {
        int Value(double arg);
        string StreamInfo(Stream param);
        Stream CodeToJson(string value);

        int HardWork(INotifyer notifyer);
    }

    public interface INotifyer
    {
        void Notify(string message);
    }
}
