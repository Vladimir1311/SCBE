using DocsToPictures.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsToPictures.NETFrameworkWEB.Models
{
    public class DocumentsQueue
    {
        private static DocumentsQueue documentsQueue;
        public static DocumentsQueue Instance => documentsQueue ?? 
            (documentsQueue = new DocumentsQueue());
        private readonly ConcurrentQueue<Document> documents = new ConcurrentQueue<Document>();
        private DocumentsQueue()
        {
        }
        public void Add
    }
}