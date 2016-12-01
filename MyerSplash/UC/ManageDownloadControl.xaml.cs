using JP.Utils.Helper;
using MyerSplash.Common;
using MyerSplash.ViewModel;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MyerSplash.UC
{
    public sealed partial class ManageDownloadControl : NavigableUserControl
    {
        public DownloadsViewModel DownloadsVM
        {
            get
            {
                return App.VMLocator.DownloadsVM;
            }
        }

        public ManageDownloadControl()
        {
            this.InitializeComponent();
            if (!DesignMode.DesignModeEnabled)
            {
                if (DeviceHelper.IsMobile)
                {
                    ImageGridView.MinItemWidth = 160;
                    ImageGridView.MinItemHeight = 208;
                }
            }
        }

        public void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            App.MainVM.ShowDownloadsUC = false;
        }

        public override void OnShow()
        {
            base.OnShow();
            Window.Current.SetTitleBar(TitleBar);
        }
    }
}
