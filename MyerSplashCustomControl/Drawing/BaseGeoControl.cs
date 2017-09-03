using Windows.UI;
using Windows.UI.Xaml;

namespace MyerSplashCustomControl
{
    public class BaseGeoControl : CustomCanvasControl
    {
        public double ShadowRadius
        {
            get { return (double)GetValue(ShadowRadiusProperty); }
            set { SetValue(ShadowRadiusProperty, value); }
        }

        public static readonly DependencyProperty ShadowRadiusProperty =
            DependencyProperty.Register("ShadowRadius", typeof(double), typeof(CustomCanvasControl),
                new PropertyMetadata(2d, (s, e) =>
                {
                    var control = s as CustomCanvasControl;
                    control.Invalidate();
                }));

        public Color ForeColor
        {
            get { return (Color)GetValue(ForeColorProperty); }
            set { SetValue(ForeColorProperty, value); }
        }

        public static readonly DependencyProperty ForeColorProperty =
            DependencyProperty.Register("ForeColor", typeof(Color), typeof(CustomCanvasControl),
                new PropertyMetadata(Colors.Black, (s, e) =>
                {
                    var control = s as CustomCanvasControl;
                    control.Invalidate();
                }));

        public Color ShadowColor
        {
            get { return (Color)GetValue(ShadowColorProperty); }
            set { SetValue(ShadowColorProperty, value); }
        }

        public static readonly DependencyProperty ShadowColorProperty =
            DependencyProperty.Register("ShadowColor", typeof(Color), typeof(CustomCanvasControl),
                new PropertyMetadata((Color)Application.Current.Resources["ShadowColor"], (s, e) =>
                {
                    var control = s as CustomCanvasControl;
                    control.Invalidate();
                }));
    }
}
