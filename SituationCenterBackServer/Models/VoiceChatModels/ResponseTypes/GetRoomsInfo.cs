using System.Collections.Generic;

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