using JP.Utils.UI;
using Microsoft.Graphics.Canvas.Effects;
using MyerSplash.Common;
using MyerSplash.Model;
using MyerSplash.ViewModel;
using MyerSplashShared.Utils;
using System;
using System.Diagnostics;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MyerSplash.View.Page
{
    public sealed partial class MainPage : CustomizedTitleBarPage
    {
        private const float TITLE_GRID_HEIGHT = 70;
        private const float DRAWER_WIDTH = 285;

        private MainViewModel MainVM { get; set; }

        private Compositor _compositor;
        private Visual _drawerVisual;
        private Visual _drawerMaskVisual;
        private Visual _titleGridVisual;
        private Visual _refreshBtnVisual;
        private Visual _titleStackVisual;

        private double _lastVerticalOffset;
        private bool _isHideTitleGrid;
        private bool _restoreTitleStackStatus;

        private ImageItem _clickedImg;
        private FrameworkElement _clickedContainer;

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(MainViewModel),
                new PropertyMetadata(false, OnLoadingPropertyChanged));

        public static void OnLoadingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var page = d as MainPage;
            if (!(bool)e.NewValue)
            {
                page.HideLoading();
            }
            else page.ShowLoading();
        }

        public bool DrawerOpended
        {
            get { return (bool)GetValue(DrawerOpendedProperty); }
            set { SetValue(DrawerOpendedProperty, value); }
        }

        public static readonly DependencyProperty DrawerOpendedProperty =
            DependencyProperty.Register("DrawerOpended", typeof(bool), typeof(MainPage),
                new PropertyMetadata(false, OnDrawerOpenedPropertyChanged));

        public static void OnDrawerOpenedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var page = d as MainPage;
            page.ToggleDrawerAnimation((bool)e.NewValue);
            page.ToggleDrawerMaskAnimation((bool)e.NewValue);
        }

        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = MainVM = new MainViewModel();
            this.Loaded += MainPage_Loaded;
            InitComposition();
            InitBinding();
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            _drawerMaskVisual.Opacity = 0;
            _drawerVisual.SetTranslation(new Vector3(-DRAWER_WIDTH, 0f, 0f));

            DrawerMaskBorder.Visibility = Visibility.Collapsed;
        }

        private void InitBinding()
        {
            var b = new Binding()
            {
                Source = MainVM,
                Path = new PropertyPath("IsRefreshing"),
                Mode = BindingMode.TwoWay,
            };
            this.SetBinding(IsLoadingProperty, b);

            var b2 = new Binding()
            {
                Source = MainVM,
                Path = new PropertyPath("DrawerOpened"),
                Mode = BindingMode.TwoWay,
            };
            this.SetBinding(DrawerOpendedProperty, b2);
        }

        private void InitComposition()
        {
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            _drawerVisual = DrawerControl.GetVisual();
            _drawerMaskVisual = DrawerMaskBorder.GetVisual();
            _titleGridVisual = TitleGrid.GetVisual();
            _refreshBtnVisual = RefreshBtn.GetVisual();
            _titleStackVisual = TitleStack.GetVisual();
        }

        #region Loading animation
        private void ShowLoading()
        {
            ListControl.Refreshing = true;
        }

        private void HideLoading()
        {
            ListControl.Refreshing = false;
        }
        #endregion

        #region Drawer animation
        private void ToggleDrawerAnimation(bool show)
        {
            var offsetAnim = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnim.InsertKeyFrame(1f, show ? 0f : -DRAWER_WIDTH);
            offsetAnim.Duration = TimeSpan.FromMilliseconds(300);

            _drawerVisual.StartAnimation(_drawerVisual.GetTranslationXPropertyName(), offsetAnim);
        }

        private void ToggleDrawerMaskAnimation(bool show)
        {
            if (show) DrawerMaskBorder.Visibility = Visibility.Visible;

            var fadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
            fadeAnimation.InsertKeyFrame(1f, show ? 0.8f : 0f);
            fadeAnimation.Duration = TimeSpan.FromMilliseconds(500);

            var batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            _drawerMaskVisual.StartAnimation("Opacity", fadeAnimation);
            batch.Completed += (sender, e) =>
              {
                  if (!show) DrawerMaskBorder.Visibility = Visibility.Collapsed;
              };
            batch.End();
        }
        #endregion

        private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ListControl.ScrollToTop();
        }

        private async void ListControl_OnClickItemStarted(ImageItem img, FrameworkElement container)
        {
            _clickedContainer = container;
            _clickedImg = img;

            DetailControl.Visibility = Visibility.Visible;
            await DetailControl.WaitForNonZeroSizeAsync();
            ToggleDetailControlAnimation();
        }

        private void DetailControl_OnHidden(object sender, EventArgs args)
        {
            if (_restoreTitleStackStatus)
            {
                ToggleTitleStackAnimation(true);
                ToggleRefreshBtnAnimation(true);
                _restoreTitleStackStatus = false;
            }
        }

        private void ToggleDetailControlAnimation()
        {
            var position = DetailControl.GetTargetPosition();
            var titleRect = TitleStack.TransformToVisual(Window.Current.Content)
                .TransformBounds(new Rect(0, 0, TitleStack.ActualWidth, TitleStack.ActualHeight));
            var clickedItemRect = _clickedContainer.TransformToVisual(Window.Current.Content)
                .TransformBounds(new Rect(0, 0, _clickedContainer.ActualWidth, _clickedContainer.ActualHeight));
            titleRect.Intersect(clickedItemRect);
            if (!titleRect.IsEmpty)
            {
                _restoreTitleStackStatus = true;
                ToggleTitleStackAnimation(false);
            }

            ToggleRefreshBtnAnimation(false);

            DetailControl.CurrentImage = _clickedImg;
            DetailControl.Show(_clickedContainer);

            NavigationService.AddOperation(() =>
            {
                DetailControl.Hide();
                return true;
            });
        }

        #region Scrolling
        private void ToggleTitleBarAnimation(bool show)
        {
            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, show ? 0f : -100f);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(500);

            _titleGridVisual.StartAnimation(_titleGridVisual.GetTranslationYPropertyName(), offsetAnimation);
        }

        private void ToggleRefreshBtnAnimation(bool show)
        {
            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, show ? 1f : 0);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(500);

            _refreshBtnVisual.CenterPoint = new Vector3((float)RefreshBtn.ActualWidth / 2f, (float)RefreshBtn.ActualHeight / 2f, 0f);
            _refreshBtnVisual.StartAnimation("Scale.X", offsetAnimation);
            _refreshBtnVisual.StartAnimation("Scale.Y", offsetAnimation);
        }

        private void ToggleTitleStackAnimation(bool show)
        {
            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, show ? 0f : -100f);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(500);

            _titleStackVisual.StartAnimation(_titleStackVisual.GetTranslationYPropertyName(), offsetAnimation);
        }

        private void ListControl_OnScrollViewerViewChanged(ScrollViewer scrollViewer)
        {
            if ((scrollViewer.VerticalOffset - _lastVerticalOffset) > 5 && !_isHideTitleGrid)
            {
                _isHideTitleGrid = true;
                ToggleRefreshBtnAnimation(false);
                ToggleTitleStackAnimation(false);
            }
            else if (scrollViewer.VerticalOffset < _lastVerticalOffset && _isHideTitleGrid)
            {
                _isHideTitleGrid = false;
                ToggleRefreshBtnAnimation(true);
                ToggleTitleStackAnimation(true);
            }
            _lastVerticalOffset = scrollViewer.VerticalOffset;

            var offset = 100 - scrollViewer.VerticalOffset;
            var alpha = offset > 0 ? (1 - offset / 90f) : 1;
            BlurBorder.Opacity = alpha;

            Debug.WriteLine("offset:" + scrollViewer.VerticalOffset);
        }
        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            CustomTitleBar();
        }

        protected override void SetUpTitleBar()
        {
            TitleBarHelper.SetUpLightTitleBar();
        }

        private void OnShownChanged(object sender, ShownArgs e)
        {
            if (!e.Shown)
            {
                Window.Current.SetTitleBar(TitleGrid);
            }
        }

        private void TitleGrid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ToggleTitleStackAnimation(true);
        }

        private void TitleGrid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
        }
    }
}
