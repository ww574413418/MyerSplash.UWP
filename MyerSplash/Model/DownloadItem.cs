using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MyerSplash.Common;
using MyerSplash.ViewModel;
using MyerSplashCustomControl;
using System;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.System;
using Windows.System.UserProfile;
using Windows.UI.Xaml;

namespace MyerSplash.Model
{
    public class DownloadItem : ViewModelBase
    {
        private BackgroundDownloader _backgroundDownloader = new BackgroundDownloader();

        private IStorageFile _resultFile;
        private CancellationTokenSource _cts;

        private UnsplashImageBase _image;
        public UnsplashImageBase Image
        {
            get
            {
                return _image;
            }
            set
            {
                if (_image != value)
                {
                    _image = value;
                    RaisePropertyChanged(() => Image);
                }
            }
        }

        private double _progress;
        public double Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                if (_progress != value)
                {
                    _progress = double.Parse(value.ToString("f0"));
                    RaisePropertyChanged(() => Progress);
                    ProgressString = $"{_progress} %";
                    if (value >= 100)
                    {
                        DownloadStatus = "";
                        SetWallpaperVisibility = Visibility.Visible;
                    }
                }
            }
        }

        private string _progressString;
        public string ProgressString
        {
            get
            {
                return _progressString;
            }
            set
            {
                if (_progressString != value)
                {
                    _progressString = value;
                    RaisePropertyChanged(() => ProgressString);
                }
            }
        }

        private string _downloadStatus;
        public string DownloadStatus
        {
            get
            {
                return _downloadStatus;
            }
            set
            {
                if (_downloadStatus != value)
                {
                    _downloadStatus = value;
                    RaisePropertyChanged(() => DownloadStatus);
                }
            }
        }

        private Visibility _setWallpaperVisibility;
        public Visibility SetWallpaperVisibility
        {
            get
            {
                return _setWallpaperVisibility;
            }
            set
            {
                if (_setWallpaperVisibility != value)
                {
                    _setWallpaperVisibility = value;
                    RaisePropertyChanged(() => SetWallpaperVisibility);
                }
            }
        }

        private RelayCommand _setWallpaperCommand;
        [IgnoreDataMember]
        public RelayCommand SetWallpaperCommand
        {
            get
            {
                if (_setWallpaperCommand != null) return _setWallpaperCommand;
                return _setWallpaperCommand = new RelayCommand(async () =>
                  {
                      if (!UserProfilePersonalizationSettings.IsSupported())
                      {
                          ToastService.SendToast("Your device can set wallpaper.");
                          return;
                      }
                      if (_resultFile != null)
                      {
                          StorageFile file = null;

                          //WTF, the file should be copy to LocalFolder to make the setting wallpaer api work.
                          var folder = ApplicationData.Current.LocalFolder;
                          var oldFile = await folder.TryGetItemAsync(_resultFile.Name) as StorageFile;
                          if (oldFile != null)
                          {
                              await _resultFile.CopyAndReplaceAsync(oldFile);
                          }
                          else
                          {
                              file = await _resultFile.CopyAsync(folder);
                          }
                          var ok = await UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(file);
                          if (ok)
                          {
                              ToastService.SendToast("Set as wallpaper successfully.");
                          }
                          else
                          {
                              ToastService.SendToast("Fail to set as wallpaper.");
                          }
                      }
                  });
            }
        }

        private RelayCommand _cancelCommand;
        [IgnoreDataMember]
        public RelayCommand CancelCommand
        {
            get
            {
                if (_cancelCommand != null) return _cancelCommand;
                return _cancelCommand = new RelayCommand(() =>
                  {
                      if (_cts != null)
                      {
                          _cts.Cancel();
                          App.VMLocator.DownloadsVM.CancelDownload(this);
                      }
                  });
            }
        }

        private RelayCommand _openCommand;
        [IgnoreDataMember]
        public RelayCommand OpenCommand
        {
            get
            {
                if (_openCommand != null) return _openCommand;
                return _openCommand = new RelayCommand(async () =>
                  {
                      var folder = await AppSettings.Instance.GetSavingFolderAsync();
                      if (folder != null)
                      {
                          await Launcher.LaunchFolderAsync(folder);
                      }
                  });
            }
        }

        public DownloadItem(UnsplashImageBase image)
        {
            this.Image = image;
            Progress = -1;
            SetWallpaperVisibility = Visibility.Collapsed;
        }

        public async Task CheckDownloadStatusAsync()
        {
            var folder = await AppSettings.Instance.GetSavingFolderAsync();
            var item = await folder.TryGetItemAsync(GetFileNameForDownloading());
            if (item != null)
            {
                var file = item as StorageFile;
                if (file != null)
                {
                    var prop = await file.GetBasicPropertiesAsync();
                    if (prop.Size > 0)
                    {
                        Progress = 100;
                    }
                }
            }
            var task = Image.RestoreDataAsync();
        }

        private string GetFileNameForDownloading()
        {
            var fileName = $"{Image.Owner.Name}  {Image.CreateTimeString}.jpg";
            return fileName;
        }

        public async Task DownloadFullImageAsync(CancellationTokenSource cts)
        {
            _cts = cts;

            var url = Image.GetSaveImageUrlFromSettings();

            if (string.IsNullOrEmpty(url)) return;

            ToastService.SendToast("Downloading in background...", 2000);

            var folder = await AppSettings.Instance.GetSavingFolderAsync();

            var newFile = await folder.CreateFileAsync(GetFileNameForDownloading(), CreationCollisionOption.OpenIfExists);

            _backgroundDownloader.SuccessToastNotification = ToastHelper.CreateToastNotification("Saved:D",
                $"Tap to open {folder.Path}.");

            var downloadOperation = _backgroundDownloader.CreateDownload(new Uri(url), newFile);
            downloadOperation.Priority = BackgroundTransferPriority.High;

            var progress = new Progress<DownloadOperation>();
            progress.ProgressChanged += Progress_ProgressChanged;
            try
            {
                DownloadStatus = "DOWNLOADING";
                Progress = 0;
                await downloadOperation.StartAsync().AsTask(_cts.Token, progress);
            }
            catch (TaskCanceledException)
            {
                await downloadOperation.ResultFile.DeleteAsync();
                downloadOperation = null;
                ToastService.SendToast("CANCEL");
            }
            catch (Exception e)
            {
                ToastService.SendToast(e.Message, 2000);
            }
        }

        private void Progress_ProgressChanged(object sender, DownloadOperation e)
        {
            Progress = ((double)e.Progress.BytesReceived / e.Progress.TotalBytesToReceive) * 100;
            if (Progress >= 100)
            {
                _resultFile = e.ResultFile;
            }
        }
    }
}
