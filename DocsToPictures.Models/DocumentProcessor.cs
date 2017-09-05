using DocsToPictures.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DocsToPictures.Models
{
    public class DocumentProcessor : IDocumentProcessor
    {
        private List<DocumentHandler> handlers;
        private ConcurrentDictionary<Guid, Document> documentsBase;
        private string dataFolder;
        public DocumentProcessor()
        {
            dataFolder = Directory.GetCurrentDirectory();
            handlers = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(T => T.IsSubclassOf(typeof(DocumentHandler)))
                .Select(T => Activator.CreateInstance(T) as DocumentHandler)
                .ToList();
            handlers.ForEach(DH => DH.Initialize());
            documentsBase = new ConcurrentDictionary<Guid, Document>();
            Task.Factory.StartNew(async () => await DocsCheck());
        }

        public IDocument AddToHandle(string fileName, Stream fileStream)
        {
            fileName = Path.GetFileName(fileName);
            var dataDir = Path.Combine(dataFolder, "uploads");
            Guid folderGuid = Guid.NewGuid();
            var folder = Directory.CreateDirectory(Path.Combine(dataDir, folderGuid.ToString()));
            using (var fileWriteStream = File.Create(Path.Combine(folder.FullName, fileName)))
                fileStream.CopyTo(fileWriteStream);

            var doc = new Document
            {
                Id = folderGuid,
                Name = fileName,
                Folder = folder.FullName
            };
            var neededHandler = handlers.FirstOrDefault(H => H.CanConvert(doc));
            if (neededHandler == null)
                throw new Exception("doc format unsopported");
            documentsBase[doc.Id] = doc;
            neededHandler.AddToHandle(doc);
            return doc;
        }

        public IDocument CheckDocument(Guid Id)
        {
            Document doc;
            if (!documentsBase.TryGetValue(Id, out doc))
                return null;
            return doc;
        }

        private async Task DocsCheck()
        {
            while (true)
            {
                foreach (var doc in documentsBase.Values.ToArray())
                    if (DateTime.Now - Directory.GetCreationTime(doc.Folder) > TimeSpan.FromHours(2))
                        DeleteDocument(doc);
                await Task.Delay(TimeSpan.FromMinutes(20));
            }
        }

        private void DeleteDocument(Document doc)
        {
            Document d = null;
            if (!documentsBase.TryRemove(doc.Id, out d))
                return;
            Directory.Delete(doc.Folder, true);
        }

        public void Dispose()
        {
            foreach (var handler in handlers)
            {
                try
                {
                    handler.Dispose();
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"error while disposing handler {ex.Message}");
                }
            }
        }
    }
}
