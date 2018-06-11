using System;
using System.Collections.Generic;
using System.Text;

namespace DocsToPictures.ClIProtocol
{
    public class PageReadyMessage : Message
    {
        public PageReadyMessage()
        {
            MessageType = MessageType.PageReady;
        }
        public int PageNum { get; set; }
        public string PagePath { get; set; }
    }
}
