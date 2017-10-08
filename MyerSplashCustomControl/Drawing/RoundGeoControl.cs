using Windows.UI.Xaml;

namespace MyerSplashCustomControl
{
    public class RoundGeoControl : BaseGeoControl
    {
        public int Radius
        {
            get { return (int)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(int), typeof(CustomCanvasControl),
                new PropertyMetadata(0, (s, e) =>
                {
                    var control = s as CustomCanvasControl;
                    control.Invalidate();
                }));
    }
}