using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public class FromClientPack
    {
        public byte RoomId;
        public byte ClientId;
        public PackType PackType;
        public byte[] VoiceRecord;
        public IPEndPoint IP;
    }
}
