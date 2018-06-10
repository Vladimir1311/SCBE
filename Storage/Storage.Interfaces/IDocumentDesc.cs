namespace Storage.Interfaces
{
    public interface IDocumentDesc : IFileDesc
    {
        int PageCount { get; }
        int ReadyCount { get; }
    }
}