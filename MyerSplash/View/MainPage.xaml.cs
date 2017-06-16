using CompositionHelper;
using MyerSplash.Common;
using MyerSplash.Model;
using MyerSplash.ViewModel;
using MyerSplashShared.Utils;
using System;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace MyerSplash.View
{
    public sealed partial class MainPage : CustomizedTitleBarPage
    {
        private const float TITLE_GRID_HEIGHT = 70;
        private const float DRAWER_WIDTH = 275;

        private MainViewModel MainVM { get; set; }

        private Compositor _compositor;
        private Visual _titleGridVisual;
        private Visual _refreshBtnVisual;
        private Visual _titleStackVisual;

        private double _lastVerticalOffset;
        private bool _isHideTitleGrid;
        private bool _restoreTitleStackStatus;

        private UnsplashImageBase _clickedImg;
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

        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = MainVM = new MainViewModel();
            InitComposition();
            InitBinding();
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
        }

        private void InitComposition()
        {
            _compositor = this.GetVisual().Compositor;
            _titleGridVisual = TitleGrid.GetVisual();
            _refreshBtnVisual = RefreshBtn.GetVisual();
            _titleStackVisual = NavigationView.GetVisual();
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

        private async void ListControl_OnClickItemStarted(UnsplashImageBase img, FrameworkElement container)
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
            if (_titleStackVisual.Offset.Y == 0)
            {
                _restoreTitleStackStatus = true;
            }
            ToggleTitleStackAnimation(false);
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

            _titleGridVisual.StartAnimation("Offset.Y", offsetAnimation);
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

            _titleStackVisual.StartAnimation("Offset.Y", offsetAnimation);
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

        private void NavigationView_OnTapTitle(object sender, EventArgs e)
        {
            ListControl.ScrollToTop();
        }
    }
}
