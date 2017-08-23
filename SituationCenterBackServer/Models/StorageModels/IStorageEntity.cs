namespace SituationCenterBackServer.Models.StorageModels
{
    public interface IStorageEntity
    {
        string Id { get; set; }
        string Name { get; set; }
        string Path { get; set; }
    }
}