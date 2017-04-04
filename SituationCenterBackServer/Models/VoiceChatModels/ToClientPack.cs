using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public struct ToClientPack
    {
        public PackType PackType;
        public ApplicationUser User;
        public byte[] Data;
    }
}
