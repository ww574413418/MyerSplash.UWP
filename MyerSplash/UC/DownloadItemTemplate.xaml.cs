using CompositionHelper;
using CompositionHelper.Animation.Fluent;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace MyerSplash.UC
{
    public sealed partial class DownloadItemTemplate : UserControl
    {
        private Compositor _compositor;
        private Visual _setAsWallpaperVisual;
        private Visual _setAsLockVisual;
        private Visual _setBothVisual;
        private Visual _setAsTBVisual;
        private bool _showMenu = false;

        public bool IsMenuOn
        {
            get { return (bool)GetValue(IsMenuOnProperty); }
            set { SetValue(IsMenuOnProperty, value); }
        }

        public static readonly DependencyProperty IsMenuOnProperty =
            DependencyProperty.Register("IsMenuOn", typeof(bool), typeof(DownloadItemTemplate),
                new PropertyMetadata(false, (sender, e) =>
                 {
                     var control = sender as DownloadItemTemplate;
                     control.ShowMenu();
                 }));

        public DownloadItemTemplate()
        {
            this.InitializeComponent();
            this.InitComposition();
            this.DataContextChanged += DownloadItemTemplate_DataContextChanged;
        }

        private void DownloadItemTemplate_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var b = new Binding()
            {
                Source = args.NewValue,
                Path = new PropertyPath("IsMenuOn"),
                Mode = BindingMode.TwoWay
            };
            SetBinding(IsMenuOnProperty, b);
        }

        private void InitComposition()
        {
            _compositor = this.GetVisual().Compositor;
            _setAsWallpaperVisual = SetAsWallpaperBtn.GetVisual();
            _setAsLockVisual = SetAsLockBtn.GetVisual();
            _setBothVisual = SetBothBtn.GetVisual();
            _setAsTBVisual = SetAsTB.GetVisual();

            _setAsWallpaperVisual.Offset = new Vector3(0, 52 * 3, 0);
            _setAsLockVisual.Offset = new Vector3(0, 52 * 2, 0);
            _setBothVisual.Offset = new Vector3(0, 52 * 1, 0);

            _setAsWallpaperVisual.Opacity = 0;
            _setAsLockVisual.Opacity = 0;
            _setBothVisual.Opacity = 0;
        }
        private void ShowMenu()
        {
            _showMenu = !_showMenu;
            _setAsTBVisual.StartBuildAnimation().Animate(AnimateProperties.Opacity)
                .To(0f)
                .Spend(300)
                .Over()
                .Start()
                .Completed += (s, arg) =>
                {
                    SetAsTB.Text = _showMenu ? "CANCEL" : "SET AS";

                    _setAsTBVisual.StartBuildAnimation().Animate(AnimateProperties.Opacity)
                        .To(1f)
                        .Spend(300)
                        .Over()
                        .Start();
                };

            ToggleAnimation(_setAsWallpaperVisual, 3, _showMenu);
            ToggleAnimation(_setAsLockVisual, 2, _showMenu);
            ToggleAnimation(_setBothVisual, 1, _showMenu);
        }

        private void ToggleAnimation(Visual visual, int index, bool show)
        {
            visual.Opacity = 1;
            visual.StartBuildAnimation().Animate(AnimateProperties.Offset.Y)
                .To(show ? 0f : index * 52)
                .Spend(500)
                .Over()
                .Start()
                .Completed += (sender, e) =>
                  {
                      if (!show) visual.Opacity = 0;
                  };
        }

        private void RootGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RootGrid.Clip = new RectangleGeometry()
            {
                Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height)
            };
        }
    }
}
