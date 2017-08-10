using MyerSplash.ViewModel;
using Windows.UI.Xaml.Controls;

namespace MyerSplash.UC
{
    public sealed partial class NoItemControl : UserControl
    {
        private MainViewModel MainVM
        {
            get
            {
                return this.DataContext as MainViewModel;
            }
        }

        public NoItemControl()
        {
            this.InitializeComponent();
        }
    }
}
