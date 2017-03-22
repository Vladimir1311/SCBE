using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public interface IRoomManager
    {
        IEnumerable<Room> Rooms { get; }
    }
}
