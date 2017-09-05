using DocsToPictures.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace DocsToPictures.Models
{
    public abstract class DocumentHandler : IDisposable
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
            workQueueStopper.Set();
        }

        public void Initialize()
        {
            workThread = new Thread(Handle);
            workThread.Start();
        }
        protected abstract void Handle();

        public abstract void Dispose();
    }
}
