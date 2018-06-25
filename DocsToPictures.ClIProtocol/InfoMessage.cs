using System;
using System.Collections.Generic;
using System.Text;

namespace DocsToPictures.ClIProtocol
{
    public class InfoMessage : Message
    {
        public InfoMessage() => MessageType = MessageType.Info;
        public string Message { get; set; }
    }
}
