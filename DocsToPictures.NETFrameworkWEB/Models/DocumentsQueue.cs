using DocsToPictures.Interfaces;
using DocsToPictures.Models;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DocsToPictures.NETFrameworkWEB.Models
{
    public class DocumentsQueue
    {
        private static string ASPTemp => ConfigurationManager.AppSettings["PathToASPTemp"];
        private static int DocumentLifeTimeMinutes => int.Parse(ConfigurationManager.AppSettings["DocumentLifeTimeMinutes"]);
        private readonly Logger<DocumentsQueue> logger = new Logger<DocumentsQueue>();

        private static DocumentsQueue documentsQueue;
        public static DocumentsQueue Instance => documentsQueue ?? 
            (documentsQueue = new DocumentsQueue());

        public IEnumerable<IDocument> CurrentDocuments => inMemoryDocuments;

        private readonly ConcurrentDictionary<Guid, DocumentHandler> documentHandlers = new ConcurrentDictionary<Guid, DocumentHandler>();

        private readonly ConcurrentQueue<Document> documents = new ConcurrentQueue<Document>();

        private readonly List<Document> inMemoryDocuments = new List<Document>();

        private readonly Task deleteLoopTask;

        private DocumentsQueue()
        {
            logger.Info("CREATED DOCUMENTS QUEUE");
            deleteLoopTask = Task.Run(DeleteLoop);
        }
        public async Task<IDocument> AddAsync(string fileName, Stream fileStream)
        {
            var doc = new Document
            {
                Name = fileName,
                Id = Guid.NewGuid()
            };
            var dirPath = Path.Combine(ASPTemp, doc.Id.ToString());
            doc.Folder = dirPath;
            Directory.CreateDirectory(dirPath);
            var filePath = Path.Combine(dirPath, fileName);
            using (var fileSystemStream = File.Create(filePath))
            {
                await fileStream.CopyToAsync(fileSystemStream);
            }
            inMemoryDocuments.Add(doc);
            var handler = new DocumentHandler(doc);
            handler.Done += () =>
            {
                documentHandlers.TryRemove(handler.Id, out var removedHandler);
                doc.DoneTime = DateTime.Now;
            };
            handler.CurrentTask = Task.Run(handler.Run);
            documentHandlers[handler.Id] = handler;
            return doc;
        }

        public void Remove(Guid id)
        {
            logger.Trace($"Try remove document with id {id}");
            var document = inMemoryDocuments.SingleOrDefault(d => d.Id == id);
            if (document == null)
                return;
            inMemoryDocuments.Remove(document);
            if (documentHandlers.TryGetValue( id, out var handler))
            {
                logger.Trace($"finded document handler {id}, doc progress is {handler.Progress}");
                handler.Dispose();
                logger.Trace($"Disposed handler {id}");
                handler.Done += () => TryRemove(document);
            }
            else
            {
                logger.Trace($"document {id}, done, delete folder {document.Folder}");
                TryRemove(document);
            }
        }

        private void TryRemove(Document doc)
        {
            try
            {
                doc.Remove();
            }
            catch (Exception ex)
            {
                logger.Warning("removing doc", ex);
            }
        }

        private async Task DeleteLoop()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
                var nowTime = DateTime.Now;
                var oldDocs = inMemoryDocuments
                    .Where(d => nowTime - d.DoneTime > TimeSpan.FromMinutes(DocumentLifeTimeMinutes))
                    .ToList();
                inMemoryDocuments.RemoveAll(d => oldDocs.Contains(d));
                oldDocs.ForEach(d =>
                    {
                        logger.Trace($"Remove doc {d.Id} after {nowTime - d.DoneTime}");
                        TryRemove(d);
                    });
            }
        }
    }
}