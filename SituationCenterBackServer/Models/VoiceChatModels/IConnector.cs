using System;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public interface IConnector
    {
        event Action<FromClientPack> OnRecieveData;
        void Start();
        void Stop();
        void SendPack(ToClientPack pack);
    }
}
