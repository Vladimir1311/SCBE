using System;
using System.Collections.Generic;
using System.Text;

namespace DocsToPictures.ClIProtocol
{
    public class MetaReadyMessage : Message
    {
        public MetaReadyMessage()
        {
            MessageType = MessageType.MetaReady;
        }
        public int PagesCount { get; set; }

    }
}
