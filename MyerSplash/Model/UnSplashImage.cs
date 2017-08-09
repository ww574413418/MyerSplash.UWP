using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using JP.Utils.UI;
using MyerSplash.Common;
using MyerSplashShared.API;
using MyerSplashShared.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Linq;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
using Newtonsoft.Json;
using JP.Utils.Data.Json;
using Windows.Data.Json;

namespace MyerSplash.Model
{
    public class UnsplashImage : ViewModelBase
    {
        public string ID { get; set; }

        [JsonProperty("urls")]
        public UnsplashUrl Urls { get; set; }

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

        public string LocationString
        {
            get
            {
                if (Location == null || Location.City == null || Location.Country == null)
                {
                    return "Unknown";
                }
                return $"{Location.City}, {Location.Country}";
            }
        }

        public string SizeString
        {
            get
            {
                return $"{Width} x {Height}";
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

        private SolidColorBrush _backColorBrush;
        [IgnoreDataMember]
        public SolidColorBrush BackColorBrush
        {
            get
            {
                return _backColorBrush;
            }
            set
            {
                if (_backColorBrush != value)
                {
                    _backColorBrush = value;
                    RaisePropertyChanged(() => BackColorBrush);
                }
            }
        }

        [IgnoreDataMember]
        public Thickness NameThickness
        {
            get
            {
                if (IsUnsplash)
                {
                    return new Thickness(0, 0, 0, 2);
                }
                else
                {
                    return new Thickness(0);
                }
            }
        }

        [JsonProperty("color")]
        public string ColorValue { get; set; }

        private double _width;
        [JsonProperty("width")]
        public double Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (_width != value)
                {
                    _width = value;
                    RaisePropertyChanged(() => Width);
                }
            }
        }

        private double _height;
        [JsonProperty("height")]
        public double Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (_height != value)
                {
                    _height = value;
                    RaisePropertyChanged(() => Height);
                }
            }
        }

        private ImageExif _exif;
        [JsonProperty("exif")]
        public ImageExif Exif
        {
            get
            {
                return _exif;
            }
            set
            {
                if (_exif != value)
                {
                    _exif = value;
                    RaisePropertyChanged(() => Exif);
                }
            }
        }

        private ImageLocation _location;
        [JsonProperty("location")]
        public ImageLocation Location
        {
            get
            {
                return _location;
            }
            set
            {
                if (_location != value)
                {
                    _location = value;
                    RaisePropertyChanged(() => Location);
                    RaisePropertyChanged(() => LocationString);
                }
            }
        }

        private UnsplashUser _owner;
        [JsonProperty("user")]
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
        [JsonProperty("liked_by_user")]
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
        [JsonProperty("likes")]
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
                return _createTime.ToString("yyyy-MM-dd hh-mm-ss");
            }
        }

        private RelayCommand _shareCommand;
        [IgnoreDataMember]
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
        [IgnoreDataMember]
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
                  });
            }
        }

        private RelayCommand _navigateHomeCommand;
        [IgnoreDataMember]
        public RelayCommand NavigateHomeCommand
        {
            get
            {
                if (_navigateHomeCommand != null) return _navigateHomeCommand;
                return _navigateHomeCommand = new RelayCommand(async () =>
                  {
                      if (!string.IsNullOrEmpty(Owner.Links.HomePageUrl))
                      {
                          await Launcher.LaunchUriAsync(new Uri(Owner.Links.HomePageUrl));
                      }
                  });
            }
        }

        private RelayCommand _downloadCommand;
        [IgnoreDataMember]
        public RelayCommand DownloadCommand
        {
            get
            {
                if (_downloadCommand != null) return _downloadCommand;
                return _downloadCommand = new RelayCommand(() =>
                  {
                      var downloaditem = new DownloadItem(this);
                      var task = downloaditem.DownloadFullImageAsync(CTSFactory.MakeCTS());
                      var task2 = App.VMLocator.DownloadsVM.AddDownloadingImageAsync(downloaditem);
                  });
            }
        }

        private bool _isUnsplash;
        public bool IsUnsplash
        {
            get
            {
                return _isUnsplash;
            }
            set
            {
                if (_isUnsplash != value)
                {
                    _isUnsplash = value;
                    RaisePropertyChanged(() => IsUnsplash);
                }
            }
        }

        public Visibility LikesVisibility
        {
            get
            {
                if (IsUnsplash)
                {
                    return Visibility.Visible;
                }
                else return Visibility.Collapsed;
            }
        }

        public Visibility RecommendationVisibility
        {
            get
            {
                if (IsUnsplash)
                {
                    return Visibility.Collapsed;
                }
                else return Visibility.Visible;
            }
        }

        public Visibility ExifThumbVisibility
        {
            get
            {
                if (IsUnsplash)
                {
                    return Visibility.Visible;
                }
                else return Visibility.Collapsed;
            }
        }

        public string PhotoByText
        {
            get
            {
                if (IsUnsplash)
                {
                    return "photo by";
                }
                else return "recommended by";
            }
        }

        [IgnoreDataMember]
        public StorageFile DownloadedFile { get; set; }

        public DownloadStatus DownloadStatus { get; set; } = DownloadStatus.Pending;

        public string ShareText => $"Share {Owner.Name}'s amazing photo from MyerSplash app. {Urls.Full}";

        public UnsplashImage()
        {
            ListImageBitmap = new CachedBitmapSource();
            LargeBitmap = new CachedBitmapSource();
            IsUnsplash = true;
        }

        public string GetFileNameForDownloading()
        {
            var fileName = $"{Owner.Name}  {CreateTimeString}.jpg";
            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (var c in invalidChars)
            {
                if (fileName.Contains(c))
                {
                    fileName = fileName.Replace(c.ToString(), "");
                }
            }
            return fileName;
        }

        public async Task SetDataRequestData(DataRequest request)
        {
            DataPackage requestData = request.Data;
            requestData.Properties.Title = "Share photo";
            requestData.Properties.ContentSourceWebLink = new Uri(Urls.Full);
            requestData.Properties.ContentSourceApplicationLink = new Uri(Urls.Full);

            requestData.SetText(ShareText);

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
        }

        public async Task RestoreDataAsync()
        {
            this.MajorColor = new SolidColorBrush(ColorValue.ToColor());
            await DownloadImgForListAsync();
        }

        public async Task DownloadImgForListAsync()
        {
            var url = GetListImageUrlFromSettings();

            if (string.IsNullOrEmpty(url)) return;

            var task = CheckAndGetDownloadedFileAsync();

            ListImageBitmap.ExpectedFileName = this.ID + ".jpg";
            ListImageBitmap.RemoteUrl = url;
            await ListImageBitmap.LoadBitmapAsync();
        }

        public async Task CheckAndGetDownloadedFileAsync()
        {
            var name = GetFileNameForDownloading();
            var folder = await KnownFolders.PicturesLibrary.CreateFolderAsync("MyerSplash", CreationCollisionOption.OpenIfExists);
            if (folder != null)
            {
                var file = await folder.TryGetItemAsync(name) as StorageFile;
                if (file != null)
                {
                    var pro = await file.GetBasicPropertiesAsync();
                    if (pro.Size > 10)
                    {
                        this.DownloadStatus = DownloadStatus.Ok;
                        DownloadedFile = file;
                    }
                }
            }
        }

        public string GetListImageUrlFromSettings()
        {
            var quality = App.AppSettings.LoadQuality;
            switch (quality)
            {
                case 0: return Urls.Regular;
                case 1: return Urls.Small;
                case 2: return Urls.Thumb;
                default: return "";
            }
        }

        public string GetSaveImageUrlFromSettings()
        {
            var quality = App.AppSettings.SaveQuality;
            switch (quality)
            {
                case 0: return Urls.Raw;
                case 1: return Urls.Full;
                case 2: return Urls.Regular;
                default: return "";
            }
        }

        public async Task GetExifInfoAsync()
        {
            var result = await CloudService.GetImageDetail(ID, CTSFactory.MakeCTS().Token);
            if (result.IsRequestSuccessful)
            {
                JsonObject.TryParse(result.JsonSrc, out JsonObject json);
                if (json != null)
                {
                    var exifObject = JsonParser.GetJsonObjFromJsonObj(json, "exif");
                    if (exifObject != null)
                    {
                        Exif = JsonConvert.DeserializeObject<ImageExif>(exifObject.ToString());
                    }

                    var locationObj = JsonParser.GetJsonObjFromJsonObj(json, "location");
                    if (locationObj != null)
                    {
                        Location = JsonConvert.DeserializeObject<ImageLocation>(locationObj.ToString());
                    }
                }
            }
        }
    }
}
