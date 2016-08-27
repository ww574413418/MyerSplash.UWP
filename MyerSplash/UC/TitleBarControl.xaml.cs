using JP.Utils.Helper;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MyerSplash.UC
{
    public sealed partial class TitleBarControl : UserControl
    {
        public event Action OnClickBackBtn;
        public event Action OnTapClickBackPlaceHolder;

        public bool ShowBackBtn
        {
            get { return (bool)GetValue(ShowBackBtnProperty); }
            set { SetValue(ShowBackBtnProperty, value); }
        }

        public static readonly DependencyProperty ShowBackBtnProperty =
            DependencyProperty.Register("ShowBackBtn", typeof(bool), typeof(TitleBarControl),
                new PropertyMetadata(true, (sender, e) =>
                 {
                     if ((bool)e.NewValue)
                     {
                         (sender as TitleBarControl).BackBtn.Visibility = Visibility.Visible;
                     }
                     else
                     {
                         (sender as TitleBarControl).BackBtn.Visibility = Visibility.Collapsed;
                     }
                 }));

        public TitleBarControl()
        {
            this.InitializeComponent();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            OnClickBackBtn?.Invoke();
        }

        public void Setup()
        {
            Window.Current.SetTitleBar(BackGrdRect);
        }
    }
}
