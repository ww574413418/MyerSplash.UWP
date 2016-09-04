using MyerSplash.Common;
using MyerSplash.ViewModel;
using Windows.UI.Xaml;

namespace MyerSplash.UC
{
    public sealed partial class SettingsControl : NavigableUserControl
    {
        private SettingsViewModel SettingsVM { get; set; }

        public SettingsControl()
        {
            this.InitializeComponent();
            this.DataContext = SettingsVM = new SettingsViewModel();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Shown = false;
        }
    }
}
