using GalaSoft.MvvmLight;
using JP.Utils.Data;
using JP.Utils.UI;
using MyerSplash.Common;
using MyerSplash.Interface;
using MyerSplash.ViewModel;
using MyerSplashCustomControl;
using MyerSplashShared.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Json;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;

namespace MyerSplash.Model
{
    public abstract class UnsplashImageBase : ViewModelBase, IUnsplashImage, IUnsplashImageFeatured
    {
        private BackgroundDownloader _backgroundDownloader = new BackgroundDownloader();

        public string ID { get; set; }

        public string RawImageUrl { get; set; }

        public string FullImageUrl { get; set; }

        public string RegularImageUrl { get; set; }

        public string SmallImageUrl { get; set; }

        public string ThumbImageUrl { get; set; }

        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    RaisePropertyChanged(() => Title);
                }
            }
        }

        private CachedBitmapSource _listImageBitmap;
        [IgnoreDataMember]
        public CachedBitmapSource ListImageBitmap
        {
            get
            {
                return _listImageBitmap;
            }
            set
            {
                if (_listImageBitmap != value)
                {
                    _listImageBitmap = value;
                    RaisePropertyChanged(() => ListImageBitmap);
                }
            }
        }

        private CachedBitmapSource _largeBitmap;
        [IgnoreDataMember]
        public CachedBitmapSource LargeBitmap
        {
            get
            {
                return _largeBitmap;
            }
            set
            {
                if (_largeBitmap != value)
                {
                    _largeBitmap = value;
                    RaisePropertyChanged(() => LargeBitmap);
                }
            }
        }

        private SolidColorBrush _majorColor;
        [IgnoreDataMember]
        public SolidColorBrush MajorColor
        {
            get
            {
                return _majorColor;
            }
            set
            {
                if (_majorColor != value)
                {
                    _majorColor = value;
                    RaisePropertyChanged(() => MajorColor);
                }
            }
        }

        private SolidColorBrush _backColor;
        [IgnoreDataMember]
        public SolidColorBrush BackColor
        {
            get
            {
                return _backColor;
            }
            set
            {
                if (_backColor != value)
                {
                    _backColor = value;
                    RaisePropertyChanged(() => BackColor);
                }
            }
        }

        public string ColorValue { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        private UnsplashUser _owner;
        public UnsplashUser Owner
        {
            get
            {
                return _owner;
            }
            set
            {
                if (_owner != value)
                {
                    _owner = value;
                    RaisePropertyChanged(() => Owner);
                }
            }
        }

        private bool _liked;
        public bool Liked
        {
            get
            {
                return _liked;
            }
            set
            {
                if (_liked != value)
                {
                    _liked = value;
                    RaisePropertyChanged(() => Liked);
                }
            }
        }

        private int _likes;
        public int Likes
        {
            get
            {
                return _likes;
            }
            set
            {
                if (_likes != value)
                {
                    _likes = value;
                    RaisePropertyChanged(() => Likes);
                    RaisePropertyChanged(() => LikesString);
                }
            }
        }

        public string LikesString
        {
            get
            {
                return Likes.ToString();
            }
        }

        private DateTime _createTime;
        public DateTime CreateTime
        {
            get
            {
                return _createTime;
            }
            set
            {
                if (_createTime != value)
                {
                    _createTime = value;
                    RaisePropertyChanged(() => CreateTime);
                }
            }
        }

        public string CreateTimeString
        {
            get
            {
                return _createTime.ToString("yyyy-MM-dd");
            }
        }

        public UnsplashImageBase()
        {
            ListImageBitmap = new CachedBitmapSource();
            LargeBitmap = new CachedBitmapSource();
        }

        public async Task SetDataRequestData(DataRequest request)
        {
            DataPackage requestData = request.Data;
            requestData.Properties.Title = "Share photo";
            requestData.Properties.ContentSourceWebLink = new Uri(FullImageUrl);
            requestData.Properties.ContentSourceApplicationLink = new Uri(FullImageUrl);

            var shareText = $"Share {this.Owner.Name}'s amazing photo from MyerSplash UWP app. {FullImageUrl}";

            requestData.SetText(shareText);

            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(shareText);
            Clipboard.SetContent(dataPackage);

            DataRequestDeferral deferral = request.GetDeferral();

            var file = await StorageFile.GetFileFromPathAsync(ListImageBitmap.LocalPath);
            if (file != null)
            {
                List<IStorageItem> imageItems = new List<IStorageItem>();
                imageItems.Add(file);
                requestData.SetStorageItems(imageItems);

                RandomAccessStreamReference imageStreamRef = RandomAccessStreamReference.CreateFromFile(file);
                requestData.SetBitmap(imageStreamRef);
                requestData.Properties.Thumbnail = imageStreamRef;
            }

            deferral.Complete();
        }

        public async Task RestoreDataAsync()
        {
            this.MajorColor = new SolidColorBrush(ColorConverter.HexToColor(ColorValue).Value);
            await DownloadImgForListAsync();
        }

        public async Task DownloadImgForListAsync()
        {
            var url = GetListImageUrlFromSettings();

            if (string.IsNullOrEmpty(url)) return;

            ListImageBitmap.ExpectedFileName = this.ID + ".jpg";
            ListImageBitmap.RemoteUrl = url;
            await ListImageBitmap.LoadBitmapAsync();
        }

        public string GetListImageUrlFromSettings()
        {
            var quality = App.AppSettings.LoadQuality;
            switch (quality)
            {
                case 0: return RegularImageUrl;
                case 1: return SmallImageUrl;
                case 2: return ThumbImageUrl;
                default: return "";
            }
        }

        public string GetSaveImageUrlFromSettings()
        {
            var quality = App.AppSettings.SaveQuality;
            switch (quality)
            {
                case 0: return RawImageUrl;
                case 1: return FullImageUrl;
                case 2: return RegularImageUrl;
                default: return "";
            }
        }

        public async Task DownloadFullImageAsync(CancellationToken token)
        {
            var url = GetSaveImageUrlFromSettings();

            if (string.IsNullOrEmpty(url)) return;

            ToastService.SendToast("Downloading in background...", 2000);

            StorageFolder folder = null;
            if (LocalSettingHelper.HasValue(SettingsViewModel.SAVING_POSITION))
            {
                var path = LocalSettingHelper.GetValue(SettingsViewModel.SAVING_POSITION);
                if (path == SettingsViewModel.DEFAULT_SAVING_POSITION)
                {
                    folder = await KnownFolders.PicturesLibrary.CreateFolderAsync("MyerSplash", CreationCollisionOption.OpenIfExists);
                }
                else
                {
                    folder = await StorageFolder.GetFolderFromPathAsync(path);
                }
            }
            if (folder == null)
            {
                folder = await KnownFolders.PicturesLibrary.CreateFolderAsync("MyerSplash", CreationCollisionOption.OpenIfExists);
            }
            var fileName = $"{Owner.Name}  {CreateTimeString}";
            var newFile = await folder.CreateFileAsync($"{fileName}.jpg", CreationCollisionOption.GenerateUniqueName);

            //backgroundDownloader.FailureToastNotification = ToastHelper.CreateToastNotification("Failed to download :-(", "You may cancel it. Otherwise please check your network.");
            _backgroundDownloader.SuccessToastNotification = ToastHelper.CreateToastNotification("Saved:D",
                $"You can find it in {folder.Path}.");

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

        protected abstract void ParseObjectFromJson(string json);
    }
}
