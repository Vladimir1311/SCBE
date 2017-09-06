using AdministrationPanel.Services;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AdministrationPanel.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LogsPage : Page
    {
        private ClientWebSocket webSocket;
        private Task socketHandler;

        public LogsPage()
        {
            this.InitializeComponent();
            webSocket = new ClientWebSocket();
            socketHandler = Task.Factory.StartNew(WebSocketCycle);
        }

        private async Task WebSocketCycle()
        {
            webSocket.Options.SetRequestHeader("Authorization", "Bearer " + AuthorizeService.Token);
            await webSocket.ConnectAsync(new Uri("ws://localhost/logs/connect"), CancellationToken.None);
            while (true)
            {
                ArraySegment<byte> data;
                var a = await webSocket.ReceiveAsync(data, CancellationToken.None);
                Lol.Items.Add(data.Count);
            }
        }
    }
}
