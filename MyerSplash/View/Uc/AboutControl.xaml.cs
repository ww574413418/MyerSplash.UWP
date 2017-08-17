using MyerSplash.Common;
using MyerSplash.ViewModel;
using Windows.UI.Xaml;

namespace MyerSplash.View.Uc
{
    public sealed partial class AboutControl : NavigableUserControl
    {
        private AboutViewModel AboutVM { get; set; }

        public AboutControl()
        {
            this.InitializeComponent();
            this.DataContext = AboutVM = new AboutViewModel();
        }

        public override void OnShow()
        {
            base.OnShow();
            Window.Current.SetTitleBar(TitleBar);
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Shown = false;
        }
    }
}
