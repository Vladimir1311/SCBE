using System.Threading.Tasks;

namespace SituationCenter.NotifyHub.Services.Interfaces
{
    public interface INotificator
    {
        Task Notify<T>(string topic, T data);
    }
}
