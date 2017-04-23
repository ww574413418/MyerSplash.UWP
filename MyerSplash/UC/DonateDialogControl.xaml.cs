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
            PopupService.Instance.TryHide();

            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText("18520944923");
            Clipboard.SetContent(dataPackage);
            ToastService.SendToast("Alipay account is copied in Clipboard", 2000);
        }

        private async void InAppClick_Click(object sender, RoutedEventArgs e)
        {
            PopupService.Instance.TryHide();

            try
            {
                var licenseInformation = CurrentApp.LicenseInformation;
                var license = licenseInformation.ProductLicenses["MyerSplashIAP"];
                if (license.IsActive)
                {
                    // the customer can access this feature
                    ToastService.SendToast("Thanks for your drink. I will do better. :P", 2000);
                }
                else
                {
                    var uc = new LoadingTextControl() { LoadingText = "Grabing infomation..." };
                    await PopupService.Instance.ShowAsync(uc);

                    // the customer can' t access this feature
                    var result = await CurrentApp.RequestProductPurchaseAsync("MyerSplashIAP");

                    PopupService.Instance.TryHide(500);

                    switch (result.Status)
                    {
                        case ProductPurchaseStatus.AlreadyPurchased:
                            {
                                ToastService.SendToast("Thanks. I will do better :P", 4000);
                            }; break;
                        case ProductPurchaseStatus.NotPurchased:
                            {

                            }; break;
                        case ProductPurchaseStatus.Succeeded:
                            {
                                ToastService.SendToast("Thanks. I will do better. :P", 2000);
                            }; break;
                    }
                }
            }
            catch (Exception ex)
            {
                await Logger.LogAsync(ex);
            }
            finally
            {
                PopupService.Instance.TryHide();
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            PopupService.Instance.TryHide();
        }
    }
}
