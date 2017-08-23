namespace SituationCenterBackServer.Models.StorageModels
{
    public class Directory : IStorageEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
}