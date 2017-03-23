using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public class RoomsManager : IRoomManager
    {
        private byte lastRoomId = 0;
        private List<Room> rooms = new List<Room>();
        private IConnector _connector;

        public RoomsManager(IOptions<UnrealAPIConfiguration> configs)
        {
            _connector = new UdpConnector(configs.Value.Port);
            _connector.OnRecieveData += _connector_OnRecieveData;
            _connector.Start();
        }

        private void _connector_OnRecieveData(FromClientPack obj)
        {
            Console.WriteLine("RECIEVED!!!!");
            Console.WriteLine($"ClientId: {obj.ClientId}, Room: {obj.RoomId}");
            Console.WriteLine($"PackType: {obj.PackType}, buffer length: {obj.VoiceRecord.Length}");
            Console.WriteLine($"Adress: {obj.IP.Address.ToString()}");
        }

        public (Room, byte) CreateNewRoom(ApplicationUser creater, string name)
        {
            if (rooms.Any(R => R.Name == name))
                throw new Exception("Комната с таким именем уже существует!");
            Room newRoom = new Room(creater, lastRoomId++)
            {
                Name = name
            };
            rooms.Add(newRoom);
            return (newRoom, creater.InRoomId);
            
        }

        public IEnumerable<Room> FindRooms(Predicate<Room> func)
        {
            return rooms.Where(R => func(R));
        }

        public Room FirstOrDefault(Predicate<Room> func)
        {
            return rooms.FirstOrDefault(R => func(R));
        }

        public IEnumerable<string> RoomNames =>
                rooms.Select(R => R.Name);
    }
}
