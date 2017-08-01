using System.Collections.Generic;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public struct ToClientPack
    {
        public PackType PackType;
        public ApplicationUser Receiver;
        public ApplicationUser Sender;
        public IEnumerable<byte> Data;
    }
}