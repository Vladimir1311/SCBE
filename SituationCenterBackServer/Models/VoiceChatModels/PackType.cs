using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public enum PackType : byte
    {
        Voice = 0,
        Message = 1,
        Auth = 2
    }
}
