using SituationCenterBackServer.Models.StorageModels;

namespace SituationCenterBackServer.Services
{
    public interface IBuffer
    {
        string GetLinkFor(File file);

        string ServLink { get; }
    }
}