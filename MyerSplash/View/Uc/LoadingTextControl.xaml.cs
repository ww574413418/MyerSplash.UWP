using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyerSplash.View.Uc
{
    public sealed partial class LoadingTextControl : UserControl
    {
        public string LoadingText
        {
            get { return (string)GetValue(LoadingTextProperty); }
            set { SetValue(LoadingTextProperty, value); }
        }

        public static readonly DependencyProperty LoadingTextProperty =
            DependencyProperty.Register("LoadingText", typeof(string), typeof(LoadingTextControl),
                new PropertyMetadata(null, OnLoadingTextPropertyChanged));

        private static void OnLoadingTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LoadingTextControl;
            control.LoadingTB.Text = (string)e.NewValue;
        }

        public LoadingTextControl()
        {
            this.InitializeComponent();
        }
    }
}