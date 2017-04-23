using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MyerSplash.UC
{
    public sealed partial class ExifInfoControl : UserControl
    {
        public SolidColorBrush ForegroundBrush
        {
            get { return (SolidColorBrush)GetValue(ForegroundBrushProperty); }
            set { SetValue(ForegroundBrushProperty, value); }
        }

        public static readonly DependencyProperty ForegroundBrushProperty =
            DependencyProperty.Register("ForegroundBrush", typeof(SolidColorBrush),
                typeof(ExifInfoControl), new PropertyMetadata(null, (s, e) =>
                 {
                     var control = s as ExifInfoControl;
                     control.TitleTB.Foreground = e.NewValue as SolidColorBrush;
                     control.TextTB.Foreground = e.NewValue as SolidColorBrush;
                 }));

        public SolidColorBrush BackgroundBrush
        {
            get { return (SolidColorBrush)GetValue(BackgroundBrushProperty); }
            set { SetValue(BackgroundBrushProperty, value); }
        }

        public static readonly DependencyProperty BackgroundBrushProperty =
            DependencyProperty.Register("BackgroundBrush", typeof(SolidColorBrush),
                typeof(ExifInfoControl), new PropertyMetadata(null, (s, e) =>
                {
                    var control = s as ExifInfoControl;
                }));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ExifInfoControl),
                new PropertyMetadata(null, (s, e) =>
                {
                    var control = s as ExifInfoControl;
                    control.TextTB.Text = e.NewValue as string;
                }));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ExifInfoControl),
                new PropertyMetadata(null, (s, e) =>
                {
                    var control = s as ExifInfoControl;
                    control.TitleTB.Text = e.NewValue as string;
                }));


        public string Symbol
        {
            get { return (string)GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }

        public static readonly DependencyProperty SymbolProperty =
            DependencyProperty.Register("Symbol", typeof(string), typeof(ExifInfoControl),
                new PropertyMetadata(null, (s, e) =>
                {
                    var control = s as ExifInfoControl;
                }));

        public ExifInfoControl()
        {
            this.InitializeComponent();
        }
    }
}
