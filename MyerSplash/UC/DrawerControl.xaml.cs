using JP.Utils.Helper;
using MyerSplash.ViewModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace MyerSplash.UC
{
    public sealed partial class DrawerControl : UserControl
    {
        private MainViewModel MainVM
        {
            get
            {
                return this.DataContext as MainViewModel;
            }
        }

        public DrawerControl()
        {
            this.InitializeComponent();
            if (DeviceHelper.IsMobile)
            {
                DownloadEntryBtn.Visibility = Visibility.Visible;
            }
            else DownloadEntryBtn.Visibility = Visibility.Collapsed;
        }
    }
}
