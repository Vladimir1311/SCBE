using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DocsToPictures.Interfaces
{
    public interface IDocumentProcessor
    {
        IDocument AddToHandle(string fileName, Stream fileStream);

        IDocument CheckDocument(Guid id);
    }
}
