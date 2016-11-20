using JP.Utils.Debug;
using MyerSplashCustomControl;
using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Store;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyerSplash.UC
{
    public sealed partial class DonateDialogControl : UserControl
    {
        public DonateDialogControl()
        {
            this.InitializeComponent();
        }

        private void AlipayBtn_Click(object sender, RoutedEventArgs e)
        {
            PopupService.Instance.TryToHide();

            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText("18520944923");
            Clipboard.SetContent(dataPackage);
            ToastService.SendToast("Alipay account is copied in Clipboard", 2000);
        }

        private async void InAppClick_Click(object sender, RoutedEventArgs e)
        {
            PopupService.Instance.TryToHide();

            try
            {
                var licenseInformation = CurrentApp.LicenseInformation;
                var license = licenseInformation.ProductLicenses["MyerSplashIAP"];
                if (license.IsActive)
                {
                    // the customer can access this feature
                    ToastService.SendToast("Thanks :D", 2000);
                }
                else
                {
                    // the customer can' t access this feature
                    var result = await CurrentApp.RequestProductPurchaseAsync("MyerSplashIAP");
                    ToastService.SendToast(result.Status.ToString(), 3000);
                }
            }
            catch (Exception ex)
            {
                await Logger.LogAsync(ex);
                ToastService.SendToast(ex.Message.ToString(), 3000);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            PopupService.Instance.TryToHide();
        }
    }
}
