namespace SituationCenterBackServer.Models.StorageModels
{
    public class Picture : IStorageEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public PictureState State { get; set; }
        public int Number { get; set; }
        public string Link { get; set; }
    }
}