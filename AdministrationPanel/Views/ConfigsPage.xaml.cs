using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using AdministrationPanel.Models;
using AdministrationPanel.Services;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace AdministrationPanel.Views
{
    public sealed partial class ConfigsPage : Page, INotifyPropertyChanged
    {
        private Order _selected;
        public Order Selected
        {
            get { return _selected; }
            set { Set(ref _selected, value); }
        }

        public ObservableCollection<Order> SampleItems { get; private set; } = new ObservableCollection<Order>();

        public ConfigsPage()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            SampleItems.Clear();

            var data = await SampleDataService.GetSampleModelDataAsync();

            foreach (var item in data)
            {
                SampleItems.Add(item);
            }
            Selected = SampleItems.First();
        }

        private void MasterListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e?.ClickedItem as Order;
            if (item != null)
            {
                if (WindowStates.CurrentState == NarrowState)
                {
                    NavigationService.Navigate<Views.ConfigsDetailPage>(item);
                }
                else
                {
                    Selected = item;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
    }
}
