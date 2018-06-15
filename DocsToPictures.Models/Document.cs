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
        public int Progress => _pagesPaths == null ? 0
            : GetAvailablePages().Count / (_pagesPaths.Length - 1) * 100;
        public bool Removed => removed;

        private List<int> indexes;
        private string[] _pagesPaths;
        private bool removed = false;

        public event Action<int, string> PageReady;
        public event Action<int> MetaReadyEvent;

        public void SetPagesCount(int length)
        {
            MetaReadyEvent?.Invoke(length);
            _pagesPaths = new string[length + 1];
        }

        public ICollection<int> GetAvailablePages()
        {
            if (removed) return new int[0];
            indexes = _pagesPaths?
                .Select((P, N) => new { Num = N, Path = P })
                .Where((Pair) => !string.IsNullOrEmpty(Pair.Path))
                .Select(Pair => Pair.Num).ToList() ?? new List<int>();
            return indexes;
        }

        public int PagesCount => removed? -1 : _pagesPaths?.Count() - 1 ?? -1;

        public int ReadyPagesCount => removed ? -1 : _pagesPaths?.Where(P => !string.IsNullOrEmpty(P)).Count() ?? 0;

        public Stream GetPicture(int pageNum)
        {
            if (_pagesPaths == null || removed) return null;
            if (File.Exists(_pagesPaths[pageNum]))
                return File.OpenRead(_pagesPaths[pageNum]);
            return null;
        }

        public string this[int i]
        {
            get => removed ? null :_pagesPaths[i];
            set
            {
                _pagesPaths[i] = value;
                PageReady?.Invoke(i, value);
            }
        }

        public void Remove()
        {
            removed = true;
            while (true)
            {
                try
                {
                    Directory.Delete(Folder, true);
                    break;
                }
                catch
                {
                }
            }
        }
    }
}
