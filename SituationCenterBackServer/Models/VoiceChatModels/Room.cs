﻿using Newtonsoft.Json;
using SituationCenterBackServer.Models.VoiceChatModels.Connectors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public class Room
    {
        public byte Id { get; }
        public string Name { get; set; }


        private static byte _lastClientId;
        private readonly List<ApplicationUser> _users;


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

        internal void UserSpeak(IConnector connector, ApplicationUser user, byte[] voiceData)
        {
            var packedData = _users.FirstOrDefault(U => U.Id == user.Id).InRoomId.Concat(voiceData);
            _users//.WithOut(dataPack.User)
                 .ForEach(receiver => connector.SendPack(new ToClientPack
                 {  
                     Sender = user,
                     Receiver = receiver,
                     PackType = PackType.Voice,
                     Data = packedData
                 }));
        }

        internal void RemoveUser(ApplicationUser user)
        {
            _users.Remove(user);
            //TODO Усли пользователей нет - начать отсчет до сметри 
        }
    }
}
