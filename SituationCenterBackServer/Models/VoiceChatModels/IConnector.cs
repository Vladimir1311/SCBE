using System;
using System.Net;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public interface IConnector
    {
        event Action<FromClientPack> OnRecieveData;
        void Start();
        void Stop();
        void SendPack(ToClientPack pack);
        void SetBindToUser(Func<string, ApplicationUser> findUserFunc);
    }
}
