using System;

namespace SituationCenterBackServer.Models.VoiceChatModels.Connectors
{
    public interface IStableConnector : IConnector
    {
        event Action<ApplicationUser> OnUserDisconnected;
    }
}