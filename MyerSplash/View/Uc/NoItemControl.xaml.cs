using MyerSplash.ViewModel;
using Windows.UI.Xaml.Controls;

namespace MyerSplash.View.Uc
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