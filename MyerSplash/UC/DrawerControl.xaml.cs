using JP.Utils.Helper;
using MyerSplash.ViewModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.ViewManagement;

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
                DownloadEntryBtn.Visibility = Visibility.Collapsed;
                FullscreenBtn.Visibility = Visibility.Collapsed;
            }

            Window.Current.SizeChanged += Current_SizeChanged;
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            if (!ApplicationView.GetForCurrentView().IsFullScreenMode)
            {
                FullscreenIcon.Symbol = Symbol.FullScreen;
            }
            else
            {
                FullscreenIcon.Symbol = Symbol.BackToWindow;
            }
        }
    }
}
