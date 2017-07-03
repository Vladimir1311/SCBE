using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AdministrationPanel.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IPResolverPanel : Page, INotifyPropertyChanged
    {
        public IPResolverPanel()
        {
            InitializeComponent();
            var refresher = Refresh();
        }

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyChangedEventHandler PropertyChanged;

        private async void RefreshButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Refresh();
        }

        private async Task Refresh()
        {
            try
            {
                RefreshButton.IsEnabled = false;
                OperationInProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync("http://ipresolver.azurewebsites.net/ip/coreip");
                    var obj = JObject.Parse(response);
                    CoreIPBox.Text = obj.GetValue("ip").ToString();
                }
            }
            catch (Exception ex)
            {
                CoreIPBox.Text = ex.Message;
            }
            finally
            {
                RefreshButton.IsEnabled = true;
                OperationInProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private async void SetCoreIPClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                RefreshButton.IsEnabled = false;
                OperationInProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync("http://ipresolver.azurewebsites.net/ip/setcoreip/" + CoreIPBox.Text);
                }
            }
            catch (Exception ex)
            {
                CoreIPBox.Text = ex.Message;
            }
            finally
            {
                RefreshButton.IsEnabled = true;
                OperationInProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }
    }
}
