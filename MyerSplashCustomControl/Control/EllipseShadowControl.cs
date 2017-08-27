using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;

namespace MyerSplashCustomControl
{
    public class EllipseShadowControl : CustomCanvasControl
    {
        public int Radius
        {
            get { return (int)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(int), typeof(EllipseShadowControl),
                new PropertyMetadata(0, (s, e) =>
                {
                    var control = s as EllipseShadowControl;
                    control.Invalidate();
                }));

        public double ShadowRadius
        {
            get { return (double)GetValue(ShadowRadiusProperty); }
            set { SetValue(ShadowRadiusProperty, value); }
        }

        public static readonly DependencyProperty ShadowRadiusProperty =
            DependencyProperty.Register("ShadowRadius", typeof(double), typeof(EllipseShadowControl),
                new PropertyMetadata(2d, (s, e) =>
                {
                    var control = s as EllipseShadowControl;
                    control.Invalidate();
                }));

        public Color ForeColor
        {
            get { return (Color)GetValue(ForeColorProperty); }
            set { SetValue(ForeColorProperty, value); }
        }

        public static readonly DependencyProperty ForeColorProperty =
            DependencyProperty.Register("ForeColor", typeof(Color), typeof(EllipseShadowControl),
                new PropertyMetadata(Colors.Black, (s, e) =>
                {
                    var control = s as EllipseShadowControl;
                    control.Invalidate();
                }));

        public EllipseShadowControl() : base()
        {
        
        }

        protected override void OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var radius = Radius;
            var center = new Vector2((float)sender.Size.Width / 2f, (float)sender.Size.Height / 2f);

            using (var renderTarget = new CanvasRenderTarget(sender, sender.Size))
            {
                using (var ds = renderTarget.CreateDrawingSession())
                {
                    ds.FillEllipse(center, radius, radius, new CanvasSolidColorBrush(sender, ForeColor));
                }
                using (var effect = new ShadowEffect())
                {
                    effect.Source = renderTarget;
                    effect.ShadowColor = (Color)Application.Current.Resources["ShadowColor"];
                    effect.BlurAmount = (float)ShadowRadius;

                    using (args.DrawingSession)
                    {
                        args.DrawingSession.DrawImage(effect, 1, 1);
                        args.DrawingSession.DrawImage(renderTarget);
                    }
                }
            }
        }
    }
}
