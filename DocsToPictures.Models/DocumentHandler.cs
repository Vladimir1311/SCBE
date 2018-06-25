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
        protected string FilePath { get; private set; }
        protected string OutFolder { get; private set; }


        private Document doc;
        private readonly CancellationToken cancellationToken;
        private List<string> supportedFormats;
        private Thread workThread;

        public IEnumerable<string> SupportedFormats => supportedFormats;
        public DocumentHandler(CancellationToken cancellationToken)
        {
            supportedFormats = GetType().GetCustomAttributes<SupportedFormatAttribyte>()
                .Select(A => A.Format)
                .ToList();
            this.cancellationToken = cancellationToken;
        }

        public bool CanConvert(IDocument doc) =>
            supportedFormats.Contains(Path.GetExtension(doc.Name));

        public void SetDocument(Document doc)
        {
            if (!CanConvert(doc))
                return;
            this.doc = doc;
            FilePath = Path.Combine(doc.Folder, doc.Name);
            OutFolder = doc.Folder;

        }

        public void Initialize()
        {
            workThread = new Thread(Wrapper);
            workThread.Start();
        }

        private void Wrapper()
        {
            try
            {
                Handle();
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void Handle()
        {
            cancellationToken.ThrowIfCancellationRequested();
            Prepare();
            var pagesCount = ReadMeta();
            doc.SetPagesCount(pagesCount);
            cancellationToken.ThrowIfCancellationRequested();
            for (int i = 1; i <= pagesCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var imagePath = Path.Combine(OutFolder, $"{i}.png");
                RenderPage(i, imagePath);
                doc[i] = imagePath;
            }
        }

        protected abstract void Prepare();
        protected abstract int ReadMeta();
        protected abstract void RenderPage(int number, string targetFilePath);

        public abstract void Dispose();
    }
}
