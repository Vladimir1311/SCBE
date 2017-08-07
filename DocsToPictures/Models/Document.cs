using DocsToPictures.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace DocsToPictures.Models
{
    public class Document : IDocument
    {
        public Guid Id { get; set; }
        public string Folder { get; set; }
        public string Name { get; set; }
        public int Progress { get; set; }
        public string[] PagesPaths { get; set; }

        public IEnumerable<int> AvailablePages => PagesPaths?.Select((P, N) => N).Where(N => N > 0) ?? new int [0];

        public Stream GetPicture(int pageNum)
        {
            if (PagesPaths == null) return null;
            if (File.Exists(PagesPaths[pageNum]))
                return File.OpenRead(PagesPaths[pageNum]);
            return null;
        }
    }
}