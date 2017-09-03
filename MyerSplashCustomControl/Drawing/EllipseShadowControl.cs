using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;

namespace MyerSplashCustomControl
{
    public class EllipseShadowControl : RoundGeoControl
    {

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
                    effect.ShadowColor = ShadowColor;
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
