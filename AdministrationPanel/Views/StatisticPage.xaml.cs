using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using AdministrationPanel.Models;
using AdministrationPanel.Services;

using Windows.UI.Xaml.Controls;

namespace AdministrationPanel.Views
{
    public sealed partial class StatisticPage : Page, INotifyPropertyChanged
    {
        // TODO: UWPTemplates: Change the chart as appropriate to your app.
        // For help see http://docs.telerik.com/windows-universal/controls/radchart/getting-started
        public StatisticPage()
        {
            InitializeComponent();
        }

        public ObservableCollection<DataPoint> Source
        {
            get
            {
                // TODO UWPTemplates: Replace this with your actual data
                return SampleDataService.GetChartSampleData();
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
