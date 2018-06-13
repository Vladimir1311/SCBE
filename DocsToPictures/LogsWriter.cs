using DocsToPictures.ClIProtocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocsToPictures
{
    static class LogsWriter
    {
        public static void PageReady(int pageNum, string pagePath)
            => Console.WriteLine(JsonConvert.SerializeObject(new PageReadyMessage { PageNum = pageNum, PagePath = pagePath }));
        public static void MetaReady(int pagesCount)
            => Console.WriteLine(JsonConvert.SerializeObject(new MetaReadyMessage { PagesCount = pagesCount }));

        public static void IncorrectDoc()
            => Console.WriteLine(JsonConvert.SerializeObject(new Message { MessageType = MessageType.IncorrectDoc }));

        public static void InvalidArgs()
            => Console.WriteLine(JsonConvert.SerializeObject(new Message { MessageType = MessageType.InvalidArgs }));

        internal static void IncorrectOutputPath()
            => Console.WriteLine(JsonConvert.SerializeObject(new Message { MessageType = MessageType.IncorrectOutputPath }));


        public static void Info(string message)
            => Console.WriteLine(JsonConvert.SerializeObject(new InfoMessage { Message = message }));
    }
}
