using Storage.Interfaces;

namespace Storage
{
    public class DocumentDesc : FileDesc, IDocumentDesc
    {
        public int PageCount { get; set; }
        public int ReadyCount { get; set; }
    }
}