using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;

namespace MyerSplashCustomControl
{
    public enum LayoutStretch
    {
        Stretch,
        Center,
        Bottom
    }

    public class ContentPopupEx : ContentControl
    {
        private TaskCompletionSource<int> _tcs;

        private Grid _rootGrid;
        private Grid _contentGrid;
        private FrameworkElement _rootFramework;
        private Border _maskBorder;

        private Popup _currentPopup;
        private LayoutStretch _layoutStretch;

        public bool PlayPopupAnim = true;

        private Storyboard _inStory;
        private Storyboard _outStory;

        public Page CurrentPage
        {
            get
            {
                return ((Window.Current.Content as Frame).Content) as Page;
            }
        }

        private bool _isOpen = false;

        public bool AllowTapMaskToHide { get; set; } = true;

        private ContentPopupEx()
        {
            DefaultStyleKey = typeof(ContentPopupEx);

            _tcs = new TaskCompletionSource<int>();

            if (_currentPopup == null)
            {
                _currentPopup = new Popup();
                _currentPopup.VerticalAlignment = VerticalAlignment.Stretch;
                this.Height = (Window.Current.Content as Frame).Height;
                this.Width = (Window.Current.Content as Frame).Width;
                _currentPopup.Child = this;
                _currentPopup.IsOpen = true;
            }

            CurrentPage.SizeChanged += Page_SizeChanged;
        }

        public ContentPopupEx(FrameworkElement element, LayoutStretch layout = LayoutStretch.Center) : this()
        {
            this._rootFramework = element;
            this._layoutStretch = layout;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _rootGrid = GetTemplateChild("RootGrid") as Grid;
            _contentGrid = GetTemplateChild("ContentGrid") as Grid;
            if (_layoutStretch == LayoutStretch.Stretch)
            {
                _contentGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                _contentGrid.VerticalAlignment = VerticalAlignment.Stretch;
            }
            else if (_layoutStretch == LayoutStretch.Bottom)
            {
                _contentGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                _contentGrid.VerticalAlignment = VerticalAlignment.Bottom;
            }
            _contentGrid.Children.Add(_rootFramework);
            _contentGrid.Margin = new Thickness(0, 10, 0, 10);
            _inStory = _rootGrid.Resources["InStory"] as Storyboard;
            _outStory = _rootGrid.Resources["OutStory"] as Storyboard;
            _maskBorder = GetTemplateChild("MaskBorder") as Border;
            _outStory.Completed += ((sender, e) =>
            {
                _currentPopup.IsOpen = false;
            });
            _maskBorder.Tapped += ((sendert, et) =>
            {
                if (!AllowTapMaskToHide) return;
                if (!_isOpen)
                {
                    return;
                }
                PopupService.Instance.TryHide();
            });
            _tcs.TrySetResult(0);
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateCurrentLayout();
        }

        private void UpdateCurrentLayout()
        {
            _rootGrid.Width = this.Width = Window.Current.Bounds.Width;
            _rootGrid.Height = this.Height = Window.Current.Bounds.Height;
        }

        public async Task ShowAsync()
        {
            await _tcs.Task;
            UpdateCurrentLayout();
            _maskBorder.Visibility = Visibility.Visible;
            _isOpen = true;
            if (PlayPopupAnim)
                _inStory.Begin();
        }

        public void Hide()
        {
            CurrentPage.SizeChanged -= Page_SizeChanged;
            _rootGrid.Children.Remove(_rootFramework);
            _rootFramework = null;
            _isOpen = false;
            _outStory.Begin();
            _maskBorder.Visibility = Visibility.Collapsed;
        }
    }
}
