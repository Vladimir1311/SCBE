using AdministrationPanel.Models;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AdministrationPanel.Views
{
    public sealed partial class ConfigsDetailControl : UserControl
    {
        public Order MasterMenuItem
        {
            get { return GetValue(MasterMenuItemProperty) as Order; }
            set { SetValue(MasterMenuItemProperty, value); }
        }

        public static DependencyProperty MasterMenuItemProperty = DependencyProperty.Register("MasterMenuItem",typeof(Order),typeof(ConfigsDetailControl),new PropertyMetadata(null));

        public ConfigsDetailControl()
        {
            InitializeComponent();
        }
    }
}
