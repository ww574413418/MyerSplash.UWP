using MyerSplashCustomControl;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyerSplash.UC
{
    public sealed partial class DeleteDialogControl : UserControl
    {
        public DeleteDialogControl()
        {
            this.InitializeComponent();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            PopupService.Instance.TryToHide();
        }

        private void DeleteAllBtn_Click(object sender, RoutedEventArgs e)
        {
            App.VMLocator.DownloadsVM.DeleteFailed();
        }

        private void DeleteDownloadingBtn_Click(object sender, RoutedEventArgs e)
        {
            App.VMLocator.DownloadsVM.DeleteDownloading();
        }

        private void DeleteDownloadedBtn_Click(object sender, RoutedEventArgs e)
        {
            App.VMLocator.DownloadsVM.DeleteDownloaded();
        }
    }
}
