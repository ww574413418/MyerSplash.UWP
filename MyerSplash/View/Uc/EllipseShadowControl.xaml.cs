﻿using JP.Utils.UI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyerSplash.View.Uc
{
    public sealed partial class EllipseShadowControl : UserControl
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
                     control.CanvasControl.Invalidate();
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
                    control.CanvasControl.Invalidate();
                }));

        public EllipseShadowControl()
        {
            this.InitializeComponent();
            this.SizeChanged += EllipseShadowControl_SizeChanged;
        }

        private void EllipseShadowControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (CanvasControl != null)
            {
                CanvasControl.Invalidate();
            }
        }

        private void CanvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
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
                    effect.BlurAmount = 2f;

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
