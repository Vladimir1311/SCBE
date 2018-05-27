namespace Storage.Interfaces
{
    public interface IFileDesc
    {
        string Name { get; }
        string FullPath { get; }
        long Size { get; }
    }
}