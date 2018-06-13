using DocsToPictures.Interfaces;
using DocsToPictures.Models;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
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

        private readonly ConcurrentQueue<DocumentHandler> documentHandlers = new ConcurrentQueue<DocumentHandler>();

        private readonly ConcurrentQueue<Document> documents = new ConcurrentQueue<Document>();

        private readonly List<IDocument> inMemoryDocuments = new List<IDocument>();
        private readonly List<Task> tasks = new List<Task>();

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
            //documents.Enqueue(doc);
            var handler = new DocumentHandler(doc);
            tasks.Add(Task.Run(handler.Run));
            return doc;
        }
    }
}