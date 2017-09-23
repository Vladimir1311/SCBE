using System;
using System.Collections.Generic;
using System.Text;

namespace CCF.Handle
{
    interface IRequestHandler
    {
        InvokeResult Handle(InvokeMessage message);
    }
}
