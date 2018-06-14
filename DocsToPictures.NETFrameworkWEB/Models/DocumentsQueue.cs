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

        private static DocumentsQueue documentsQueue;
        public static DocumentsQueue Instance => documentsQueue ?? 
            (documentsQueue = new DocumentsQueue());

        public IEnumerable<IDocument> CurrentDocuments => inMemoryDocuments;

        private readonly ConcurrentDictionary<Guid, DocumentHandler> documentHandlers = new ConcurrentDictionary<Guid, DocumentHandler>();

        private readonly ConcurrentQueue<Document> documents = new ConcurrentQueue<Document>();

        private readonly List<IDocument> inMemoryDocuments = new List<IDocument>();

        private DocumentsQueue()
        {
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
            handler.CurrentTask = Task.Run(handler.Run);
            documentHandlers[handler.Id] = handler;
            return doc;
        }

        public Task Remove(Guid id)
        {
            if (documentHandlers.TryGetValue(id, out var handler))
            {
                handler.Dispose();
            }
            return Task.CompletedTask;
        }
    }
}