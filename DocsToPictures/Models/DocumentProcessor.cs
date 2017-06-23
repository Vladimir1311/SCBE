using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace DocsToPictures.Models
{
    public class DocumentProcessor
    {
        private static DocumentProcessor proccessor = new DocumentProcessor();
        private List<DocumentHandler> handlers;
        private ConcurrentDictionary<Guid, Document> documentsBase;

        public static DocumentProcessor Instance => proccessor;

        private DocumentProcessor()
        {
            handlers = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(T => T.IsExtended<DocumentHandler>())
                .Select(T => Activator.CreateInstance(T) as DocumentHandler)
                .ToList();
            documentsBase = new ConcurrentDictionary<Guid, Document>();
            Task.Factory.StartNew(async () => await DocsCheck());
        }


        public void AddToHandle(Document doc)
        {
            var neededHandler = handlers.FirstOrDefault(H => H.CanConvert(doc));
            if (neededHandler == null)
                throw new Exception("doc format unsopported");
            documentsBase[doc.Id] = doc;
            neededHandler.AddToHandle(doc);

        }

        public Document GetDocument(Guid Id)
        {
            Document doc;
            if (!documentsBase.TryGetValue(Id, out doc))
                throw new Exception("No doc with id " + Id);
            return doc;
        }

        private async Task DocsCheck()
        {
            while(true)
            {
                foreach(var doc in documentsBase.Values.ToArray())
                    if (DateTime.Now - Directory.GetCreationTime(doc.Folder) > TimeSpan.FromHours(2))
                        DeleteDocument(doc.Id);
                await Task.Delay(TimeSpan.FromMinutes(20));
            }
        }

        private void DeleteDocument(Guid id)
        {
            Document document;
            if (documentsBase.TryRemove(id, out document))
            {
                Directory.Delete(document.Folder, true);
            }
        }


    }
}