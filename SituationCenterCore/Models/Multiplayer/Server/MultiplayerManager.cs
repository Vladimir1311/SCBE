
using SituationCenterCore.Models.Multiplayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Threading;
using System.Text;
using SituationCenterCore.Data;
using SituationCenterCore.Models.Multiplayer.Messages;
using SituationCenterCore.Extensions;

namespace SituationCenterCore.Models.Multiplayer.Server
{
    public class MultiplayerManager : IMultiplayerManager
    {
        private HashSet<EndPoint> endPoints = new HashSet<EndPoint>();
        
        public async Task AddClient(WebSocket webSocket, ApplicationUser user)
        {
            var endpoint = new EndPoint(webSocket, user);
            endPoints.Add(endpoint);
            while (true)
            {
                var message = await endpoint.ReadMessageAsync();
                foreach (var point in endPoints.Except(endpoint))
                {
                    await point.SendMessageAsync(message);
                }
            }
        }
        
    }
}
