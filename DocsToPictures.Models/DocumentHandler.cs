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
        private readonly CancellationToken cancellationToken;
        private List<string> supportedFormats;
        private Thread workThread;
        
        protected ConcurrentQueue<Document> documentsStream = new ConcurrentQueue<Document>();

        public IEnumerable<string> SupportedFormats => supportedFormats;
        public DocumentHandler(CancellationToken cancellationToken)
        {
            supportedFormats = GetType().GetCustomAttributes<SupportedFormatAttribyte>()
                .Select(A => A.Format)
                .ToList();
            workQueueStopper = new ManualResetEvent(false);
            this.cancellationToken = cancellationToken;
        }

        public bool CanConvert(IDocument doc) =>
            supportedFormats.Contains(Path.GetExtension(doc.Name));

        public void AddToHandle(Document doc)
        {
            if (!CanConvert(doc))
                return;
            documentsStream.Enqueue(doc);
            workQueueStopper.Set();
        }

        public void Initialize()
        {
            workThread = new Thread(HandleCycle);
            workThread.Start();
        }
        protected static int Percents(double done, double all)
        {
            return (int)Math.Floor((done / all) * 100);
        }


        private void HandleCycle()
        {
            while (true)
            {
                while (!workQueueStopper.WaitOne(TimeSpan.FromSeconds(3)))
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;
                };
                while (documentsStream.TryDequeue(out var document))
                {
                    Handle(document);
                }
                workQueueStopper.Reset();
            }
        }
        protected abstract void Handle(Document document);

        public abstract void Dispose();
    }
}
