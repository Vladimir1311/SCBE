using System;
using System.Threading.Tasks;
namespace SituationCenterCore.Services.Interfaces.RealTime
{
    public interface INotificator
    {
        Task Notify<T>(string topic, T data);
    }
}
