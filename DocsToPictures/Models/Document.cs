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

        private List<int> indexes;

        public IEnumerable<int> AvailablePages
        {
            get
            {
                indexes = PagesPaths?.Select((P, N) => N).Where((N) => !string.IsNullOrEmpty(PagesPaths[N])).ToList() ?? new List<int>();
                return indexes;
            }
        }

        public int PagesCount => PagesPaths?.Count() - 1 ?? -1;
        public Stream GetPicture(int pageNum)
        {
            if (PagesPaths == null) return null;
            if (File.Exists(PagesPaths[pageNum]))
                return File.OpenRead(PagesPaths[pageNum]);
            return null;
        }
    }
}