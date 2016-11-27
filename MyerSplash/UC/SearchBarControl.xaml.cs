using MyerSplash.ViewModel;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Composition;
using CompositionHelper;
using System.Numerics;
using Windows.UI.Xaml.Data;
using Windows.ApplicationModel;

namespace MyerSplash.UC
{
    public sealed partial class SearchBarControl : UserControl
    {
        private MainViewModel MainVM
        {
            get
            {
                return this.DataContext as MainViewModel;
            }
        }

        public bool Shown
        {
            get { return (bool)GetValue(ShownProperty); }
            set { SetValue(ShownProperty, value); }
        }

        public static readonly DependencyProperty ShownProperty =
            DependencyProperty.Register("Shown", typeof(bool), typeof(SearchBarControl),
                new PropertyMetadata(false, OnShownPropertyChanged));

        private static void OnShownPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SearchBarControl;
            control.ToggleAnimation();
        }

        private Compositor _compositor;
        private Visual _maskVisual;
        private Visual _barVisual;

        public SearchBarControl()
        {
            this.InitializeComponent();
            if (!DesignMode.DesignModeEnabled)
            {
                InitComposition();
                InitBinding();
            }
        }

        private void InitBinding()
        {
            var b = new Binding()
            {
                Source = MainVM,
                Path = new PropertyPath("ShowSearchBar"),
                Mode = BindingMode.TwoWay,
            };
            SetBinding(ShownProperty, b);
        }

        private void InitComposition()
        {
            this.Visibility = Visibility.Collapsed;

            _compositor = this.GetVisual().Compositor;
            _maskVisual = MaskBorder.GetVisual();
            _barVisual = SearchBorder.GetVisual();

            _maskVisual.Opacity = 0f;
            _barVisual.Offset = new Vector3(0f, -150f, 0f);
        }

        private void ToggleAnimation()
        {
            this.Visibility = Visibility.Visible;

            if (Shown)
            {
                InputTB.Focus(FocusState.Programmatic);
            }

            var maskAnimation = _compositor.CreateScalarKeyFrameAnimation();
            maskAnimation.InsertKeyFrame(1f, Shown ? 1f : 0f);
            maskAnimation.Duration = TimeSpan.FromMilliseconds(400);

            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, Shown ? 0f : -150f);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(400);

            var batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            _maskVisual.StartAnimation("Opacity", maskAnimation);
            _barVisual.StartAnimation("Offset.y", offsetAnimation);
            batch.Completed += (sender, e) =>
              {
                  if (!Shown)
                  {
                      this.Visibility = Visibility.Collapsed;
                  }
                  else
                  {

                  }
              };
            batch.End();
        }
    }
}
