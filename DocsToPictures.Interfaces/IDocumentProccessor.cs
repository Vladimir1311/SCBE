using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DocsToPictures.Interfaces
{
    public interface IDocumentProccessor
    {

        event Action<IDocument> DocumentReady;

        Guid AddToHandle(string fileName, Stream fileStream);

        IDocument CheckDocument(Guid id);
    }


    public interface IDocument
    {
        IEnumerable<int> AvailablePages { get; }

        Stream GetPicture(int pageNum);
    }
}
