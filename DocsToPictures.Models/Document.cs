using DocsToPictures.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        public List<int> GetAvailablePages()
        {
                indexes = PagesPaths?
                    .Select((P, N) => new { Num = N, Path = P})
                    .Where((Pair) => !string.IsNullOrEmpty(Pair.Path))
                    .Select(Pair => Pair.Num).ToList() ?? new List<int>();
                return indexes;
        }

        public int PagesCount => PagesPaths?.Count() - 1 ?? -1;

        public int ReadyPagesCount => PagesPaths?.Where(P => !string.IsNullOrEmpty(P)).Count() ?? 0;

        public Stream GetPicture(int pageNum)
        {
            if (PagesPaths == null) return null;
            if (File.Exists(PagesPaths[pageNum]))
                return File.OpenRead(PagesPaths[pageNum]);
            return null;
        }
    }
}
