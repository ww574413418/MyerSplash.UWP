using MyerSplash.ViewModel;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MyerSplash.UC
{
    public sealed partial class NavigationView : UserControl
    {
        public MainViewModel MainVM
        {
            get
            {
                return App.MainVM;
            }
        }

        public event EventHandler<EventArgs> OnTapTitle;

        public NavigationView()
        {
            this.InitializeComponent();
        }

        private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            OnTapTitle?.Invoke(this, new EventArgs());
        }
    }
}
