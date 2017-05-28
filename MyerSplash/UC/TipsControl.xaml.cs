using MyerSplashCustomControl;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyerSplash.UC
{
    public sealed partial class TipsControl : UserControl
    {
        public TipsControl()
        {
            this.InitializeComponent();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            PopupService.Instance.TryHide();
        }
    }
}
