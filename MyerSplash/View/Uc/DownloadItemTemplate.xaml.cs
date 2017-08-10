using CompositionHelper;
using CompositionHelper.Animation.Fluent;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
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
        private Visual _backFIVisual;
        private Visual _openBtnVisual;
        private Visual _copyBtnVisual;
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
            _backFIVisual = BackFI.GetVisual();
            _openBtnVisual = OpenBtn.GetVisual();
            _copyBtnVisual = CopyUrlBtn.GetVisual();

            _setAsWallpaperVisual.Offset = new Vector3(0, 52 * 3, 0);
            _setAsLockVisual.Offset = new Vector3(0, 52 * 2, 0);
            _setBothVisual.Offset = new Vector3(0, 52 * 1, 0);

            _setAsWallpaperVisual.Opacity = 0f;
            _setAsLockVisual.Opacity = 0f;
            _setBothVisual.Opacity = 0f;
            _backFIVisual.Opacity = 0f;
            _copyBtnVisual.Opacity = 0f;
        }

        private void ShowMenu()
        {
            _showMenu = !_showMenu;
            _setAsTBVisual.StartBuildAnimation().Animate(AnimateProperties.Opacity)
                .To(_showMenu ? 0f : 1f)
                .Spend(300)
                .BeginAfter(TimeSpan.FromMilliseconds(_showMenu ? 0f : 300f))
                .Over()
                .Start();

            OpenBtn.Visibility = Visibility.Visible;
            _openBtnVisual.StartBuildAnimation().Animate(AnimateProperties.Opacity)
                .To(_showMenu ? 0f : 1f)
                .Spend(300)
                .BeginAfter(TimeSpan.FromMilliseconds(_showMenu ? 0f : 500f))
                .Over()
                .Start()
                .Completed += (s, e) =>
                  {
                      OpenBtn.Visibility = _showMenu ? Visibility.Collapsed : Visibility.Visible;
                  };

            _backFIVisual.StartBuildAnimation().Animate(AnimateProperties.Opacity)
                .To(_showMenu ? 1f : 0f)
                .BeginAfter(TimeSpan.FromMilliseconds(_showMenu ? 300f : 0f))
                .Spend(300)
                .Over()
                .Start();

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

        private void Img_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            _copyBtnVisual.StartBuildAnimation().Animate(AnimateProperties.Opacity)
                .To(1f)
                .Spend(300)
                .Over()
                .Start();
        }

        private void Img_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            _copyBtnVisual.StartBuildAnimation().Animate(AnimateProperties.Opacity)
                .To(0f)
                .Spend(300)
                .Over()
                .Start();
        }
    }
}
