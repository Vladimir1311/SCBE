using System;
using System.Net;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public interface IConnector
    {
        event Action<FromClientPack> OnRecieveData;
        event Action<ApplicationUser> OnUserConnected;
        void Start();
        void Stop();
        void SendPack(ToClientPack pack);
        void SetBindToUser(Func<string, ApplicationUser> findUserFunc);
    }
}
