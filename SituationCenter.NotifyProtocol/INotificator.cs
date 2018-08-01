using System.Threading.Tasks;

namespace SituationCenter.NotifyProtocol
{
    public interface INotificator
    {
        Task Notify<T>(string topic, T data) where T: class;
    }
}
