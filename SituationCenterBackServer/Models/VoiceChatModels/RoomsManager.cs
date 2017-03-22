using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public class RoomsManager : IRoomManager
    {
        private byte lastId = 0;
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

        public IEnumerable<Room> Rooms => rooms;
    }
}
