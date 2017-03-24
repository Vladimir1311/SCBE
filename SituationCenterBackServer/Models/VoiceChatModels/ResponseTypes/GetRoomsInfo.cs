using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.VoiceChatModels.ResponseTypes
{
    public class GetRoomsInfo : ResponseData
    {
        public IEnumerable<Room> Rooms { get; set; }
        public GetRoomsInfo()
        {
            Success = true;
        }
    }
}
