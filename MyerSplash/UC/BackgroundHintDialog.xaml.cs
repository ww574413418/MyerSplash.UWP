using MyerSplashCustomControl;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyerSplash.UC
{
    public sealed partial class BackgroundHintDialog : UserControl
    {
        public BackgroundHintDialog()
        {
            this.InitializeComponent();
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            PopupService.Instance.TryHide();
        }
    }
}
