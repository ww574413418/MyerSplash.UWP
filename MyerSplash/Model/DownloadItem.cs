using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JP.Utils.Debug;
using MyerSplash.Common;
using MyerSplashCustomControl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.System;

namespace MyerSplash.Model
{
    public enum DisplayMenu
    {
        Downloading = 0,
        Retry = 1,
        SetAs = 2
    }

    public class DownloadItem : ViewModelBase
    {
        public event Action<DownloadItem, bool> OnMenuStatusChanged;

        private TaskCompletionSource<int> _tcs;

        public Guid DownloadOperationGUID { get; set; }

        private IStorageFile _resultFile;
        private CancellationTokenSource _cts;

        private UnsplashImage _image;
        public UnsplashImage Image
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
                    if (value >= 100) UpateUIWhenCompleted();
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

        private string _resolution;
        public string Resolution
        {
            get
            {
                return _resolution;
            }
            set
            {
                if (_resolution != value)
                {
                    _resolution = value;
                    RaisePropertyChanged(() => Resolution);
                }
            }
        }

        private int _displayIndex;
        public int DisplayIndex
        {
            get
            {
                return _displayIndex;
            }
            set
            {
                if (_displayIndex != value)
                {
                    _displayIndex = value;
                    RaisePropertyChanged(() => DisplayIndex);
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
                    await WallpaperSettingHelper.SetAsBackgroundAsync(_resultFile as StorageFile);
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
                    await WallpaperSettingHelper.SetAsLockscreenAsync(_resultFile as StorageFile);
                });
            }
        }

        private RelayCommand _setBothCommand;
        [IgnoreDataMember]
        public RelayCommand SetBothCommand
        {
            get
            {
                if (_setBothCommand != null) return _setBothCommand;
                return _setBothCommand = new RelayCommand(async () =>
                {
                    IsMenuOn = false;
                    await WallpaperSettingHelper.SetBothAsync(_resultFile as StorageFile);
                });
            }
        }

        private RelayCommand _setAsCommand;
        [IgnoreDataMember]
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
                      }

                      DisplayIndex = (int)DisplayMenu.Retry;
                  });
            }
        }

        private RelayCommand _deleteCommand;
        [IgnoreDataMember]
        public RelayCommand DeleteCommand
        {
            get
            {
                if (_deleteCommand != null) return _deleteCommand;
                return _deleteCommand = new RelayCommand(() =>
                  {
                      App.VMLocator.DownloadsVM.DeleteDownload(this);
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

        private RelayCommand _retryDownloadCommand;
        [IgnoreDataMember]
        public RelayCommand RetryDownloadCommand
        {
            get
            {
                if (_retryDownloadCommand != null) return _retryDownloadCommand;
                return _retryDownloadCommand = new RelayCommand(async () =>
                  {
                      await RetryAsync();
                  });
            }
        }

        public DownloadItem(UnsplashImage image)
        {
            Image = image;
            Progress = 0;
            ProgressString = "0 %";
            IsMenuOn = false;
            DisplayIndex = (int)DisplayMenu.Downloading;
        }

        public async Task RetryAsync()
        {
            await DownloadFullImageAsync(_cts = new CancellationTokenSource());
        }

        public async void UpateUIWhenCompleted()
        {
            await Task.Delay(500);
            DownloadStatus = "";
            DisplayIndex = (int)DisplayMenu.SetAs;
        }

        public async Task CheckDownloadStatusAsync(IReadOnlyList<DownloadOperation> operations)
        {
            var task = Image.RestoreDataAsync();

            var folder = await AppSettings.Instance.GetSavingFolderAsync();
            var item = await folder.TryGetItemAsync(Image.GetFileNameForDownloading());
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
            if (Progress != 100)
            {
                var downloadOperation = operations.Where(s => s.Guid == DownloadOperationGUID).FirstOrDefault();
                if (downloadOperation != null)
                {
                    var progress = new Progress<DownloadOperation>();
                    progress.ProgressChanged += Progress_ProgressChanged;
                    _cts = new CancellationTokenSource();
                    await downloadOperation.StartAsync().AsTask(_cts.Token, progress);
                }
                else
                {
                    DisplayIndex = (int)DisplayMenu.Retry;
                }
            }
        }

        public async Task AwaitGuidCreatedAsync()
        {
            if (_tcs != null)
            {
                await _tcs.Task;
            }
        }

        public async Task<bool> DownloadFullImageAsync(CancellationTokenSource cts)
        {
            _cts = cts;

            _tcs = new TaskCompletionSource<int>();

            var url = Image.GetSaveImageUrlFromSettings();

            if (string.IsNullOrEmpty(url)) return false;

            DisplayIndex = (int)DisplayMenu.Downloading;

            Image.DownloadStatus = Common.DownloadStatus.Downloading;

            StorageFolder savedFolder = null;
            StorageFile savedFile = null;

            try
            {
                savedFolder = await AppSettings.Instance.GetSavingFolderAsync();
                savedFile = await savedFolder.CreateFileAsync(Image.GetFileNameForDownloading(), CreationCollisionOption.OpenIfExists);
            }
            catch (UnauthorizedAccessException e)
            {
                await Logger.LogAsync(e);
                ToastService.SendToast("No right to create file for writing. Please check your security settings. \n If necessary, please contact me via about page.", 5000);
                return false;
            }

            var backgroundDownloader = new BackgroundDownloader();
            backgroundDownloader.SuccessToastNotification = ToastHelper.CreateToastNotification("Saved:D",
                                $"Find it in {savedFolder.Path}.", savedFile.Path);

            var downloadOperation = backgroundDownloader.CreateDownload(new Uri(url), savedFile);
            downloadOperation.Priority = BackgroundTransferPriority.High;

            DownloadOperationGUID = downloadOperation.Guid;
            _tcs.TrySetResult(0);

            ToastService.SendToast("Downloading in background...", 2000);

            try
            {
                DownloadStatus = "DOWNLOADING";
                Progress = 0;

                var progress = new Progress<DownloadOperation>();
                progress.ProgressChanged += Progress_ProgressChanged;

                await downloadOperation.StartAsync().AsTask(_cts.Token, progress);

                return true;
            }
            catch (TaskCanceledException)
            {
                await downloadOperation.ResultFile.DeleteAsync();
                ToastService.SendToast("Download has been cancelled.");
                DownloadStatus = "";
                DisplayIndex = (int)DisplayMenu.Retry;
                Image.DownloadStatus = Common.DownloadStatus.Pending;
                throw;
            }
            catch (Exception e)
            {
                Image.DownloadStatus = Common.DownloadStatus.Pending;
                await Logger.LogAsync(e);
                ToastService.SendToast("ERROR" + e.Message, 2000);

                return false;
            }
        }

        private void Progress_ProgressChanged(object sender, DownloadOperation e)
        {
            Progress = ((double)e.Progress.BytesReceived / e.Progress.TotalBytesToReceive) * 100;
            Debug.WriteLine(Progress);
            if (Progress >= 100)
            {
                _resultFile = e.ResultFile;
                Image.DownloadStatus = Common.DownloadStatus.Ok;
                Image.DownloadedFile = _resultFile as StorageFile;
                Messenger.Default.Send(new GenericMessage<string>(Image.ID), MessengerTokens.REPORT_DOWNLOADED);
            }
        }
    }
}
