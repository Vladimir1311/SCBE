using AdministrationPanel.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
