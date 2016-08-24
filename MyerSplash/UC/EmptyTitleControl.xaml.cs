using JP.Utils.Helper;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyerSplash.UC
{
    public sealed partial class EmptyTitleControl : UserControl
    {
        public EmptyTitleControl()
        {
            this.InitializeComponent();
            if (DeviceHelper.IsDesktop)
            {
                Window.Current.SetTitleBar(this);
            }
        }
    }
}
