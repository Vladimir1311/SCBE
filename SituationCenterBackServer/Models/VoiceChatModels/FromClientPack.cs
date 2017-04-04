using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public class FromClientPack
    {
        public ApplicationUser User;
        public PackType PackType;
        public byte[] VoiceRecord;
    }
}
