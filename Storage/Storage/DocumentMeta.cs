using System.Collections.Generic;

namespace Storage
{
    /// <summary>
    /// Информация о документе
    /// </summary>
    public class DocumentMeta
    {
        public int PageCount { get; set; }
        public HashSet<int> NotReadyPage { get; set; }
        public HashSet<int> HavePaintPage { get; set; }

        public DocumentMeta()
        {
            NotReadyPage = new HashSet<int>();
            HavePaintPage = new HashSet<int>();
        }
    }
}