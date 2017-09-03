using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace MyerSplashCustomControl
{
    public class CustomCanvasControl : Control
    {
        private TaskCompletionSource<int> _tcs;
        private CanvasControl _canvasControl;

        public CustomCanvasControl()
        {
            DefaultStyleKey = typeof(CustomCanvasControl);
            _tcs = new TaskCompletionSource<int>();
            this.SizeChanged += CustomCanvasControl_SizeChanged;
        }

        private void CustomCanvasControl_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            Invalidate();
        }

        public void Invalidate()
        {
            _canvasControl?.Invalidate();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _canvasControl = GetTemplateChild("Canvas") as CanvasControl;
            _canvasControl.Draw += OnDraw;
            _tcs.TrySetResult(0);
        }

        protected virtual void OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            
        }
    }
}
