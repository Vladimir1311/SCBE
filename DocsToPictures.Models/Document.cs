using DocsToPictures.Interfaces;
using Newtonsoft.Json;
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

        private List<int> indexes;
        private string[] _pagesPaths;

        public event Action<int, string> PageReady;
        public event Action<int> MetaReadyEvent;

        public void SetPagesCount(int length)
        {
            MetaReadyEvent?.Invoke(length);
            _pagesPaths = new string[length + 1];
        }

        public ICollection<int> GetAvailablePages()
        {
            indexes = _pagesPaths?
                .Select((P, N) => new { Num = N, Path = P })
                .Where((Pair) => !string.IsNullOrEmpty(Pair.Path))
                .Select(Pair => Pair.Num).ToList() ?? new List<int>();
            return indexes;
        }

        public int PagesCount => _pagesPaths?.Count() - 1 ?? -1;

        public int ReadyPagesCount => _pagesPaths?.Where(P => !string.IsNullOrEmpty(P)).Count() ?? 0;

        public Stream GetPicture(int pageNum)
        {
            if (_pagesPaths == null) return null;
            if (File.Exists(_pagesPaths[pageNum]))
                return File.OpenRead(_pagesPaths[pageNum]);
            return null;
        }

        public string this[int i]
        {
            get => _pagesPaths[i];
            set
            {
                _pagesPaths[i] = value;
                PageReady?.Invoke(i, value);
            }
        }
    }
}
