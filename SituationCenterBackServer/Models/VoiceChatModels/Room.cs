using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public class Room
    {
        private static byte _lastClientId;

        public byte Id { get; }

        private readonly List<ApplicationUser> _users;

        public string Name { get; set; }

        //TODO сделать умерщвтление комнаты после ухода пользователей
        [JsonIgnore]
        public DateTime TimeOut { get; set; }

        [JsonIgnore]
        public IEnumerable<ApplicationUser> Users => _users;
        public Room(ApplicationUser creater, byte id)
        {
            _users = new List<ApplicationUser> { creater };
            creater.InRoomId = _lastClientId++;
            Id = id;
        }

        internal void AddUser(ApplicationUser user)
        {
            _users.Add(user);
            user.InRoomId = _lastClientId++;
        }

        internal void UserSended(IConnector connector, FromClientPack dataPack)
        {
            _users//.WithOut(dataPack.User)
                 .ForEach(_ => connector.SendPack(new ToClientPack
                 {  
                     User = dataPack.User,
                     PackType = PackType.Voice,
                     Data = dataPack.VoiceRecord,
                 }));
        }

        internal void RemoveUser(ApplicationUser user)
        {
            _users.Remove(user);
            //TODO Усли пользователей нет - начать отсчет до сметри 
        }
    }
}
