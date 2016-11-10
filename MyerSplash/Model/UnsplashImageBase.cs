using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using JP.Utils.Network;
using JP.Utils.UI;
using MyerSplash.Common;
using MyerSplash.Interface;
using MyerSplashCustomControl;
using MyerSplashShared.Shared;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Json;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace MyerSplash.Model
{
    public abstract class UnsplashImageBase : ViewModelBase, IUnsplashImage, IUnsplashImageFeatured, IParseFromJson
    {
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
                    if (ColorConverter.IsLight(value.Color))
                    {
                        InfoForeColor = new SolidColorBrush(Colors.Black);
                        InfoForeColor = new SolidColorBrush(Colors.Black);
                        InfoForeColor = new SolidColorBrush(Colors.Black);
                        BtnForeColor = new SolidColorBrush(Colors.White);
                    }
                    else
                    {
                        InfoForeColor = new SolidColorBrush(Colors.White);
                        InfoForeColor = new SolidColorBrush(Colors.White);
                        InfoForeColor = new SolidColorBrush(Colors.White);
                        BtnForeColor = new SolidColorBrush(Colors.Black);
                    }
                }
            }
        }

        private SolidColorBrush _infoForeColor;
        [IgnoreDataMember]
        public SolidColorBrush InfoForeColor
        {
            get
            {
                return _infoForeColor;
            }
            set
            {
                if (_infoForeColor != value)
                {
                    _infoForeColor = value;
                    RaisePropertyChanged(() => InfoForeColor);
                }
            }
        }

        private SolidColorBrush _btnForeColor;
        [IgnoreDataMember]
        public SolidColorBrush BtnForeColor
        {
            get
            {
                return _btnForeColor;
            }
            set
            {
                if (_btnForeColor != value)
                {
                    _btnForeColor = value;
                    RaisePropertyChanged(() => BtnForeColor);
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

        private RelayCommand _shareCommand;
        public RelayCommand ShareCommand
        {
            get
            {
                if (_shareCommand != null) return _shareCommand;
                return _shareCommand = new RelayCommand(() =>
                  {
                      DataTransferManager.ShowShareUI();
                  });
            }
        }

        private RelayCommand _copyUrlCommand;
        public RelayCommand CopyUrlCommand
        {
            get
            {
                if (_copyUrlCommand != null) return _copyUrlCommand;
                return _copyUrlCommand = new RelayCommand(() =>
                  {
                      DataPackage dataPackage = new DataPackage();
                      dataPackage.SetText(GetSaveImageUrlFromSettings());
                      Clipboard.SetContent(dataPackage);
                      ToastService.SendToast("Copied.");
                  });
            }
        }

        private RelayCommand _navigateHomeCommand;
        public RelayCommand NavigateHomeCommand
        {
            get
            {
                if (_navigateHomeCommand != null) return _navigateHomeCommand;
                return _navigateHomeCommand = new RelayCommand(async () =>
                  {
                      if (!string.IsNullOrEmpty(Owner.HomePageUrl))
                      {
                          await Launcher.LaunchUriAsync(new Uri(Owner.HomePageUrl));
                      }
                  });
            }
        }

        private RelayCommand _downloadCommand;
        public RelayCommand DownloadCommand
        {
            get
            {
                if (_downloadCommand != null) return _downloadCommand;
                return _downloadCommand = new RelayCommand(() =>
                  {
                      var downloaditem = new DownloadItem(this);
                      var task = downloaditem.DownloadFullImageAsync(CTSFactory.MakeCTS());
                      App.VMLocator.DownloadsVM.AddDownloadingImage(downloaditem);
                  });
            }
        }

        public string ShareText => $"Share {this.Owner.Name}'s amazing photo from MyerSplash app. {FullImageUrl}";

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

            requestData.SetText(ShareText);

            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(ShareText);
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

        public abstract void ParseObjectFromJsonString(string json);

        public abstract void ParseObjectFromJsonObject(JsonObject json);
    }
}
