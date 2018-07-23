using System;
using System.Threading.Tasks;
namespace SituationCenterCore.Services.Interfaces.RealTime
{
    public interface INotificator
    {
        Task Notify(string topic, object data);
        Task Notify<T>(string topic, T data);
    }
}
