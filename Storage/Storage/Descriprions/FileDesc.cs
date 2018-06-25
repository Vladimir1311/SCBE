using Storage.Interfaces;

namespace Storage
{
    public class FileDesc : IFileDesc
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        public long Size { get; set; }
    }
}