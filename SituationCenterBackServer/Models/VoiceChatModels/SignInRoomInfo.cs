using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public class SignInRoomInfo : ResponseData
    {
        public byte RoomId { get; set; }
        public byte ClientId { get; set; }
        public int Port { get; set; }

        public SignInRoomInfo()
        {
            Succsess = true;
        }
    }
}
