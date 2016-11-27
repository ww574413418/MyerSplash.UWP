using JP.Utils.Debug;
using JP.Utils.UI;
using MyerSplash.Common;
using MyerSplash.Model;
using MyerSplashCustomControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace MyerSplash.UC
{
    public sealed partial class PhotoDetailControl : UserControl, INotifyPropertyChanged
    {
        public event Action OnHideControl;
        public event PropertyChangedEventHandler PropertyChanged;

        private Compositor _compositor;
        private Visual _detailGridVisual;
        private Visual _borderGridVisual;
        private Visual _shareBtnVisual;
        private Visual _infoGridVisual;
        private Visual _loadingPath;
        private Visual _flipperVisual;

        private CancellationTokenSource _cts;

        private UnsplashImageBase _currentImage;
        public UnsplashImageBase CurrentImage
        {
            get
            {
                return _currentImage;
            }
            set
            {
                if (_currentImage != value)
                {
                    _currentImage = value;
                    RaisePropertyChanged(nameof(CurrentImage));
                }
            }
        }

        private DataTransferManager _dataTransferManager;

        public bool IsShown { get; set; }

        private void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public PhotoDetailControl()
        {
            InitializeComponent();
            InitComposition();
            this.DataContext = this;
            _dataTransferManager = DataTransferManager.GetForCurrentView();
            _dataTransferManager.DataRequested += _dataTransferManager_DataRequested;
        }

        private async void _dataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequestDeferral deferral = args.Request.GetDeferral();
            sender.TargetApplicationChosen += (s, e) =>
              {
                  deferral.Complete();
              };
            await CurrentImage.SetDataRequestData(args.Request);
            deferral.Complete();
        }

        private void InitComposition()
        {
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            _detailGridVisual = ElementCompositionPreview.GetElementVisual(DetailGrid);
            _borderGridVisual = ElementCompositionPreview.GetElementVisual(MaskBorder);
            _infoGridVisual = ElementCompositionPreview.GetElementVisual(InfoGrid);
            _loadingPath = ElementCompositionPreview.GetElementVisual(LoadingPath);
            _shareBtnVisual = ElementCompositionPreview.GetElementVisual(ShareBtn);
            _flipperVisual = ElementCompositionPreview.GetElementVisual(FlipperControl);

            ResetVisualInitState();
        }

        private void ResetVisualInitState()
        {
            _infoGridVisual.Offset = new Vector3(0f, -100f, 0);
            _shareBtnVisual.Offset = new Vector3(150f, 0f, 0f);
            _flipperVisual.Offset = new Vector3(170f, 0f, 0f);
            _detailGridVisual.Opacity = 0;

            PhotoSV.ChangeView(null, 0, null);
            StartLoadingAnimation();
        }

        private void MaskBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            HideDetailControl();
        }

        public void HideDetailControl()
        {
            ToggleFlipperControlAnimation(false);
            ToggleShareBtnAnimation(false);

            var batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            ToggleInfoGridAnimation(false);
            batch.Completed += (s, ex) =>
            {
                OnHideControl?.Invoke();
                ToggleDetailGridAnimation(false);
                FlipperControl.DisplayIndex = (int)DownloadStatus.Pending;
            };
            batch.End();
        }

        public void ToggleDetailGridAnimation(bool show)
        {
            IsShown = show;

            this.Visibility = Visibility.Visible;

            var fadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
            fadeAnimation.InsertKeyFrame(1f, show ? 1f : 0f);
            fadeAnimation.Duration = TimeSpan.FromMilliseconds(show ? 700 : 300);
            fadeAnimation.DelayTime = TimeSpan.FromMilliseconds(show ? 400 : 0);

            var batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            _detailGridVisual.StartAnimation("Opacity", fadeAnimation);

            if (show)
            {
                var task = CheckImageDownloadStatusAsync();
                ToggleFlipperControlAnimation(true);
                ToggleShareBtnAnimation(true);
                ToggleInfoGridAnimation(true);
            }

            batch.Completed += (sender, e) =>
            {
                if (!show)
                {
                    ResetVisualInitState();
                    this.Visibility = Visibility.Collapsed;
                }
            };
            batch.End();
        }

        private void ToggleFlipperControlAnimation(bool show)
        {
            var offsetAnimation = _compositor.CreateVector3KeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, new Vector3(show ? 0f : 100f, 0f, 0f));
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(1000);
            offsetAnimation.DelayTime = TimeSpan.FromMilliseconds(show ? 500 : 0);

            _flipperVisual.StartAnimation("Offset", offsetAnimation);
        }

        private void ToggleShareBtnAnimation(bool show)
        {
            var offsetAnimation = _compositor.CreateVector3KeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, new Vector3(show ? 0f : 150f, 0f, 0f));
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(show ? 1000 : 400);
            offsetAnimation.DelayTime = TimeSpan.FromMilliseconds(show ? 400 : 0);

            _shareBtnVisual.StartAnimation("Offset", offsetAnimation);
        }

        private void ToggleInfoGridAnimation(bool show)
        {
            var offsetAnimation = _compositor.CreateVector3KeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, new Vector3(0f, show ? 0f : -100f, 0f));
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(500);
            offsetAnimation.DelayTime = TimeSpan.FromMilliseconds(show ? 500 : 0);

            _infoGridVisual.StartAnimation("Offset", offsetAnimation);
        }

        private void DetailGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.DetailContentGrid.Height = this.DetailContentGrid.ActualWidth / 1.5 + 100;
            this.DetailContentGrid.Clip = new RectangleGeometry()
            { Rect = new Rect(0, 0, DetailContentGrid.ActualWidth, DetailContentGrid.Height) };
        }

        private async Task CheckImageDownloadStatusAsync()
        {
            await CurrentImage.CheckDownloadedAsync();
            this.FlipperControl.DisplayIndex = (int)CurrentImage.DownloadStatus;
        }

        #region Download animation

        private async void DownloadBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentImage.DownloadStatus == DownloadStatus.OK)
            {
                return;
            }

            FlipperControl.DisplayIndex = (int)DownloadStatus.Downloading;

            try
            {
                _cts = new CancellationTokenSource();
                var item = new DownloadItem(CurrentImage);
                App.VMLocator.DownloadsVM.AddDownloadingImage(item);
                await item.DownloadFullImageAsync(_cts);

                //Still in this page
                if (IsShown)
                {
                    CurrentImage.DownloadStatus = DownloadStatus.OK;
                    FlipperControl.DisplayIndex = (int)DownloadStatus.OK;
                    ToastService.SendToast("Saved :D", 1000);
                }
            }
            catch (OperationCanceledException)
            {
                FlipperControl.DisplayIndex = (int)DownloadStatus.Pending;
            }
            catch (Exception ex)
            {
                var task = Logger.LogAsync(ex);
                FlipperControl.DisplayIndex = (int)DownloadStatus.Pending;
                ToastService.SendToast($"Exception throws.{ex.Message}", 1000);
            }
        }

        private void StartLoadingAnimation()
        {
            var rotateAnimation = _compositor.CreateScalarKeyFrameAnimation();
            rotateAnimation.InsertKeyFrame(1, 360f, _compositor.CreateLinearEasingFunction());
            rotateAnimation.Duration = TimeSpan.FromMilliseconds(800);
            rotateAnimation.IterationBehavior = AnimationIterationBehavior.Forever;

            _loadingPath.CenterPoint = new Vector3((float)LoadingPath.ActualWidth / 2, (float)LoadingPath.ActualHeight / 2, 0f);

            _loadingPath.RotationAngleInDegrees = 0f;
            _loadingPath.StopAnimation("RotationAngleInDegrees");
            _loadingPath.StartAnimation("RotationAngleInDegrees", rotateAnimation);
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            _cts?.Cancel();
        }
        #endregion

        private void DetailGrid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (Math.Abs(e.Cumulative.Translation.Y) > 30)
            {
                MaskBorder_Tapped(null, null);
            }
        }

        private void InfoPlaceHolderGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var grid = sender as Grid;
            grid.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height) };
        }

        private async void LargeImage_DragStarting(UIElement sender, DragStartingEventArgs args)
        {
            var image = (sender as FrameworkElement).DataContext as UnsplashImageBase;
            var file = await StorageFile.GetFileFromPathAsync(image.ListImageBitmap.LocalPath);
            if (file != null)
            {
                args.Data.SetStorageItems(new List<StorageFile>() { file });
            }
            args.Data.RequestedOperation = DataPackageOperation.Copy;
            args.Data.SetText(image.ShareText);
            args.Data.SetWebLink(new Uri(image.GetSaveImageUrlFromSettings()));
        }

        private async void CopyUlrBtn_Click(object sender, RoutedEventArgs e)
        {
            CopyFlipperControl.DisplayIndex = 1;
            await Task.Delay(2000);
            CopyFlipperControl.DisplayIndex = 0;
        }
    }
}
