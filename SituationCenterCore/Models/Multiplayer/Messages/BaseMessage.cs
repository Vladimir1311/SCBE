using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterCore.Models.Multiplayer.Messages
{
    public class BaseMessage
    {
        public MultiPlayerMessageType Type { get; set; }
    }
}
