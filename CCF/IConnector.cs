using System;
using System.Collections.Generic;
using System.Text;

namespace CCF
{
    interface IConnector
    {
        void Send(string val);
        event Action<string> Received;
    }
}
