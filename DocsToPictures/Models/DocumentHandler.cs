using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.IO;
using System.Threading;
using DocsToPictures.Interfaces;

namespace DocsToPictures.Models
{
    public abstract class DocumentHandler
    {
        protected ManualResetEvent workQueueStopper;
        private List<string> supportedFormats;
        private Thread workThread;

        public DocumentHandler()
        {
            supportedFormats = GetType().GetCustomAttributes<SupportedFormatAttribyte>()
                .Select(A => A.Format)
                .ToList();
            workQueueStopper = new ManualResetEvent(false);
        }

        public bool CanConvert(IDocument doc) =>
            supportedFormats.Contains(Path.GetExtension(doc.Name));

        protected ConcurrentQueue<Document> documentsStream = new ConcurrentQueue<Document>();
        public void AddToHandle(Document doc)
        {
            if (!CanConvert(doc))
                return;
            documentsStream.Enqueue(doc);
        }

        public void Initialize()
        {
            workThread = new Thread(Handle);
            workThread.Start();
        }
        protected abstract void Handle();
    }
}