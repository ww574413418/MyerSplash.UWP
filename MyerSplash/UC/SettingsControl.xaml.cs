using MyerSplash.Common;
using MyerSplash.ViewModel;
using Windows.ApplicationModel;
using Windows.UI.Xaml;

namespace MyerSplash.UC
{
    public sealed partial class SettingsControl : NavigableUserControl
    {
        private SettingsViewModel SettingsVM { get; set; }

        public SettingsControl()
        {
            this.InitializeComponent();
            if(!DesignMode.DesignModeEnabled)
            {
                this.DataContext = SettingsVM = new SettingsViewModel();
            }
        }

        public override async void OnShow()
        {
            base.OnShow();
            Window.Current.SetTitleBar(TitleBar);
            await SettingsVM.CalculateCacheAsync();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Shown = false;
        }
    }
}
