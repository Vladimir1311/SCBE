using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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


    }
}