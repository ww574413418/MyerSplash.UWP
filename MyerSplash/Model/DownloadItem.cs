using GalaSoft.MvvmLight;
using MyerSplash.Common;
using MyerSplashCustomControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

namespace MyerSplash.Model
{
    public class DownloadItem : ViewModelBase
    {
        private BackgroundDownloader _backgroundDownloader = new BackgroundDownloader();

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
                    _progress = value;
                    RaisePropertyChanged(() => Progress);
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


        public DownloadItem(UnsplashImageBase image)
        {
            this.Image = image;
        }


        public async Task DownloadFullImageAsync(CancellationToken token)
        {
            var url = Image.GetSaveImageUrlFromSettings();

            if (string.IsNullOrEmpty(url)) return;

            ToastService.SendToast("Downloading in background...", 2000);

            var folder = await AppSettings.Instance.GetSavingFolderAsync();

            var fileName = $"{Image.Owner.Name}  {Image.CreateTimeString}";
            var newFile = await folder.CreateFileAsync($"{fileName}.jpg", CreationCollisionOption.GenerateUniqueName);

            _backgroundDownloader.SuccessToastNotification = ToastHelper.CreateToastNotification("Saved:D",
                $"Tap to open {folder.Path}.");

            var downloadOperation = _backgroundDownloader.CreateDownload(new Uri(url), newFile);

            var progress = new Progress<DownloadOperation>();
            try
            {
                await downloadOperation.StartAsync().AsTask(token, progress);
            }
            catch (TaskCanceledException)
            {
                await downloadOperation.ResultFile.DeleteAsync();
                downloadOperation = null;
                throw;
            }
        }
    }
}
