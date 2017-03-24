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
        void SendPack(IPEndPoint endpoint, int port, byte[] data);
    }
}
