using GalaSoft.MvvmLight.Ioc;
using MyerSplash.ViewModel;
using MyerSplashCustomControl;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyerSplash.View.Uc
{
    public sealed partial class DeleteDialogControl : UserControl
    {
        public DownloadsViewModel DownloadsVM
        {
            get
            {
                return SimpleIoc.Default.GetInstance<DownloadsViewModel>();
            }
        }

        public DeleteDialogControl()
        {
            this.InitializeComponent();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            PopupService.Instance.TryHide();
        }

        private void DeleteAllBtn_Click(object sender, RoutedEventArgs e)
        {
            DownloadsVM.DeleteFailed();
        }

        private void DeleteDownloadingBtn_Click(object sender, RoutedEventArgs e)
        {
            DownloadsVM.DeleteDownloading();
        }

        private void DeleteDownloadedBtn_Click(object sender, RoutedEventArgs e)
        {
            DownloadsVM.DeleteDownloaded();
        }
    }
}
