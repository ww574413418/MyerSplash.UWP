using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;

namespace MyerSplashCustomControl
{
    public class RectShadowControl : BaseGeoControl
    {
        protected override void OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            using (var renderTarget = new CanvasRenderTarget(sender, sender.Size))
            {
                using (var ds = renderTarget.CreateDrawingSession())
                {
                    ds.FillRectangle(0, 0, (float)sender.Size.Width - (float)ShadowRadius * 2f, (float)sender.Size.Height - (float)ShadowRadius * 2f,
                        new CanvasSolidColorBrush(sender, ForeColor));
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
