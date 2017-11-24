using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SituationCenterCore.Data.DatabaseAbstraction;
using SituationCenterCore.Models.Multiplayer.Interfaces;

namespace SituationCenterCore.Pages.Center
{
    public class WebSocketModel : AuthorizedPage
    {
        private readonly IMultiplayerManager multiplayerManager;

        public WebSocketModel(IMultiplayerManager multiplayerManager, IRepository repository) : base(repository)
        {
            this.multiplayerManager = multiplayerManager;
        }

        public string Message { get; private set; }
        public async Task OnGetAsync()
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                Message = $"Page for web socket connection, your id is {UserId}";
                return;
            }
            var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await multiplayerManager.AddClient(webSocket, SignedUser);
            Message = "OK";
        }
    }
}