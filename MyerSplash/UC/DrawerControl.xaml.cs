using MyerSplash.ViewModel;
using Windows.UI.Xaml.Controls;

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
        }
    }
}
