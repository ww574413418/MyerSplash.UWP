using MyerSplashCustomControl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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

        private void InAppClick_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            PopupService.Instance.TryToHide();
        }
    }
}
