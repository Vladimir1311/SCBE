using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DocsToPictures.Interfaces
{
    public interface IDocument
    {
        Guid Id { get; }
        string Name { get; }
        int PagesCount { get; }
        int ReadyPagesCount { get; }
        ICollection<int> GetAvailablePages();
        Stream GetPicture(int pageNum);
    }
}
