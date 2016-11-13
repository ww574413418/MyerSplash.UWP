using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MyerSplash.Common;
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
        public event Action<DownloadItem, bool> OnMenuStatusChanged;

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

        private bool _isMenuOn;
        public bool IsMenuOn
        {
            get
            {
                return _isMenuOn;
            }
            set
            {
                if (_isMenuOn != value)
                {
                    _isMenuOn = value;
                    RaisePropertyChanged(() => IsMenuOn);
                    OnMenuStatusChanged?.Invoke(this, value);
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
                    IsMenuOn = false;
                    var file = await PrepareImageFileAsync();
                    if (file != null)
                    {
                        var ok = await UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(file);
                        if (ok)
                        {
                            ToastService.SendToast("Set as wallpaper successfully.");
                        }
                        else
                        {
                            ToastService.SendToast("Fail to set as lock screen.");
                        }
                        await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                    }
                    else
                    {
                        ToastService.SendToast("Fail to set as wallpaper and lock screen.");
                    }
                });
            }
        }

        private RelayCommand _setLockWallpaperCommand;
        [IgnoreDataMember]
        public RelayCommand SetLockWallpaperCommand
        {
            get
            {
                if (_setLockWallpaperCommand != null) return _setLockWallpaperCommand;
                return _setLockWallpaperCommand = new RelayCommand(async () =>
                {
                    IsMenuOn = false;
                    var file = await PrepareImageFileAsync();
                    if (file != null)
                    {
                        var ok = await UserProfilePersonalizationSettings.Current.TrySetLockScreenImageAsync(file);
                        if (ok)
                        {
                            ToastService.SendToast("Set as lock screen successfully.");
                        }
                        else
                        {
                            ToastService.SendToast("Fail to set as lock screen.");
                        }
                        await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                    }
                    else
                    {
                        ToastService.SendToast("Fail to set as wallpaper and lock screen.");
                    }
                });
            }
        }

        private RelayCommand _setBothCommand;
        public RelayCommand SetBothCommand
        {
            get
            {
                if (_setBothCommand != null) return _setBothCommand;
                return _setBothCommand = new RelayCommand(async () =>
                {
                    IsMenuOn = false;

                    var file = await PrepareImageFileAsync();
                    if (file != null)
                    {
                        var result1 = await UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(file);
                        var result2 = await UserProfilePersonalizationSettings.Current.TrySetLockScreenImageAsync(file);
                        if (result1 && result2)
                        {
                            ToastService.SendToast("Successfully.");
                        }
                        else
                        {
                            ToastService.SendToast("Fail to set as wallpaper and lock screen.");
                        }
                        await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                    }
                    else
                    {
                        ToastService.SendToast("Fail to set as wallpaper and lock screen.");
                    }
                });
            }
        }

        private RelayCommand _setAsCommand;
        public RelayCommand SetAsCommand
        {
            get
            {
                if (_setAsCommand != null) return _setAsCommand;
                return _setAsCommand = new RelayCommand(() =>
                  {
                      IsMenuOn = !IsMenuOn;
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
            IsMenuOn = false;
        }

        private async Task<StorageFile> PrepareImageFileAsync()
        {
            if (!UserProfilePersonalizationSettings.IsSupported())
            {
                ToastService.SendToast("Your device can set wallpaper.");
                return null;
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

                return file;
            }

            return null;
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
                    _resultFile = file;
                    var prop = await _resultFile.GetBasicPropertiesAsync();
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
