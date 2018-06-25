using System;
using System.Collections.Generic;
using System.Text;

namespace DocsToPictures.ClIProtocol
{
    public class FinishMessage : Message
    {
        public FinishMessage()
        {
            MessageType = MessageType.Finish;
        }
    }
}
