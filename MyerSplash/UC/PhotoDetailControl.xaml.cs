using CompositionHelper.Animation.Fluent;
using GalaSoft.MvvmLight.Messaging;
using JP.Utils.Debug;
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
        private Visual _taskbarImageVisual;
        private Visual _lockScreenImageVisual;
        private Visual _secondaryToolVisual;
        private Visual _setAsSPVisual;

        private CancellationTokenSource _cts;
        private int _showingPreview = 0;

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
            var manager = DataTransferManager.GetForCurrentView();
            manager.DataRequested += _dataTransferManager_DataRequested;

            Messenger.Default.Register<GenericMessage<string>>(this, MessengerTokens.REPORT_DOWNLOADED, msg =>
              {
                  var id = msg.Content;
                  if (id == CurrentImage.ID)
                  {
                      FlipperControl.DisplayIndex = (int)DownloadStatus.OK;
                  }
              });
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
            _taskbarImageVisual = ElementCompositionPreview.GetElementVisual(TaskBarImage);
            _lockScreenImageVisual = ElementCompositionPreview.GetElementVisual(LockImage);
            _secondaryToolVisual = ElementCompositionPreview.GetElementVisual(SecondaryToolSP);
            _setAsSPVisual = ElementCompositionPreview.GetElementVisual(SetAsSP);

            ResetVisualInitState();
        }

        private void ResetVisualInitState()
        {
            _infoGridVisual.Offset = new Vector3(0f, -100f, 0);
            _shareBtnVisual.Offset = new Vector3(150f, 0f, 0f);
            _flipperVisual.Offset = new Vector3(170f, 0f, 0f);
            _detailGridVisual.Opacity = 0;
            _taskbarImageVisual.Opacity = 0;
            _lockScreenImageVisual.Opacity = 0;
            _secondaryToolVisual.Opacity = 1;
            _setAsSPVisual.Opacity = 0;
            _setAsSPVisual.Offset = new Vector3(0f, 150f, 0f);

            PhotoSV.ChangeView(null, 0, null);
            StartLoadingAnimation();
        }

        private void MaskBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            HideDetailControl();
        }

        public void HideDetailControl()
        {
            ToggleSetAsSP(false);

            DismissPreview();
            TogglePreviewButtonAnimation(false);

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

        private void TogglePreviewButtonAnimation(bool show)
        {
            _secondaryToolVisual.StartBuildAnimation()
                .Animate(AnimateProperties.Opacity)
                .To(show ? 1 : 0)
                .Spend(300)
                .Over()
                .Start();
        }

        public void ToggleDetailGridAnimation(bool show)
        {
            IsShown = show;

            this.Visibility = Visibility.Visible;

            TogglePreviewButtonAnimation(true);

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
            await CurrentImage.CheckAndGetDownloadedFileAsync();
            this.FlipperControl.DisplayIndex = (int)CurrentImage.DownloadStatus;
        }

        #region Download

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
                item = await App.VMLocator.DownloadsVM.AddDownloadingImageAsync(item);

                if (item != null)
                {
                    await item.DownloadFullImageAsync(_cts);
                }

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
            if (image == null) return;
            var file = await StorageFile.GetFileFromPathAsync(image.ListImageBitmap.LocalPath);
            if (file != null)
            {
                args.Data.SetStorageItems(new List<StorageFile>() { file });
            }
            args.Data.RequestedOperation = DataPackageOperation.Copy;
            args.Data.SetText(image.ShareText);
            args.Data.SetWebLink(new Uri(image.GetSaveImageUrlFromSettings()));
        }

        private void TogglePreview()
        {
            ToggleSetAsSP(false);
            _showingPreview++;
            if (_showingPreview > 2)
            {
                _showingPreview = 0;
            }

            Visual fadingVisual = null;
            Visual showingVisual = null;
            switch (_showingPreview)
            {
                case 0:
                    {
                        fadingVisual = _lockScreenImageVisual;
                        showingVisual = null;
                    }
                    break;
                case 1:
                    {
                        fadingVisual = null;
                        showingVisual = _taskbarImageVisual;
                    }
                    break;
                case 2:
                    {
                        fadingVisual = _taskbarImageVisual;
                        showingVisual = _lockScreenImageVisual;
                    }
                    break;
            }
            if (fadingVisual != null)
            {
                fadingVisual.StartBuildAnimation()
                    .Animate(AnimateProperties.Opacity)
                    .To(0)
                    .Spend(300)
                    .Over()
                    .Start()
                    .Completed += (sender, e) =>
                      {
                          if (_showingPreview == 2)
                          {
                              TaskBarImage.Visibility = Visibility.Collapsed;
                          }
                          else if (_showingPreview == 0)
                          {
                              LockImage.Visibility = Visibility.Collapsed;
                          }
                      };
            }
            if (showingVisual != null)
            {
                if (_showingPreview == 1)
                {
                    TaskBarImage.Visibility = Visibility.Visible;
                }
                else if (_showingPreview == 2)
                {
                    LockImage.Visibility = Visibility.Visible;
                }
                showingVisual.StartBuildAnimation()
                    .Animate(AnimateProperties.Opacity)
                    .To(1)
                    .Spend(300)
                    .Over()
                    .Start();
            }
        }

        private void ToggleSetAsSP(bool show)
        {
            if (show)
            {
                DismissPreview();
            }
            if (!show)
            {
                FlipperControl.DisplayIndex = 2;
            }
            SetAsSP.Visibility = Visibility.Visible;
            _setAsSPVisual.StartBuildAnimation()
                  .Animate(AnimateProperties.Offset.Y)
                  .To(show ? 0 : 150f)
                  .Spend(500)
                  .Over()
                  .Start()
                  .Completed += (sender, e) =>
                  {
                      if (!show)
                      {
                          SetAsSP.Visibility = Visibility.Collapsed;
                      }
                  };
            _setAsSPVisual.StartBuildAnimation()
                  .Animate(AnimateProperties.Opacity)
                  .To(show ? 1 : 0)
                  .Spend(300)
                  .Over()
                  .Start();
        }

        private void DismissPreview()
        {
            _showingPreview = 0;
            _taskbarImageVisual.StartBuildAnimation()
                   .Animate(AnimateProperties.Opacity)
                   .To(0)
                   .Spend(300)
                   .Over()
                   .Start()
                   .Completed += (sender, e) =>
                   {
                       TaskBarImage.Visibility = Visibility.Collapsed;
                   };
            _lockScreenImageVisual.StartBuildAnimation()
                  .Animate(AnimateProperties.Opacity)
                  .To(0)
                  .Spend(300)
                  .Over()
                  .Start()
                  .Completed += (sender, e) =>
                  {
                      LockImage.Visibility = Visibility.Collapsed;
                  };
        }

        private async void CopyUlrBtn_Click(object sender, RoutedEventArgs e)
        {
            ToggleSetAsSP(false);
            CopyFlipperControl.DisplayIndex = 1;
            await Task.Delay(2000);
            CopyFlipperControl.DisplayIndex = 0;
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_setAsSPVisual.Opacity == 0)
            {
                FlipperControl.DisplayIndex = 3;
                ToggleSetAsSP(true);
            }
            else
            {
                ToggleSetAsSP(false);
            }
        }

        private async void SetAsBackgroundBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentImage.DownloadedFile != null)
            {
                await WallpaperSettingHelper.SetAsBackgroundAsync(CurrentImage.DownloadedFile);
            }
        }

        private async void SetAsLockscreenBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentImage.DownloadedFile != null)
            {
                await WallpaperSettingHelper.SetAsLockscreenAsync(CurrentImage.DownloadedFile);
            }
        }

        private async void SetAsBothBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentImage.DownloadedFile != null)
            {
                await WallpaperSettingHelper.SetBothAsync(CurrentImage.DownloadedFile);
            }
        }

        private void PreviewBtn_Click(object sender, RoutedEventArgs e)
        {
            TogglePreview();
        }

        private void InfoBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SetAsGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetAsGrid.Clip = new RectangleGeometry()
            {
                Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height)
            };
        }

        private void ShareBtn_Click(object sender, RoutedEventArgs e)
        {
            ToggleSetAsSP(false);
        }

        private void AutherNameBtn_Click(object sender, RoutedEventArgs e)
        {
            ToggleSetAsSP(false);
        }
    }
}
