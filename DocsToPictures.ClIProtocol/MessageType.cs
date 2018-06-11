using System;
using System.Collections.Generic;
using System.Text;

namespace DocsToPictures.ClIProtocol
{
    public enum MessageType
    {
        MetaReady,
        PageReady,
        IncorrectDoc,
        Error,
        InvalidArgs,
        Info
    }
}
