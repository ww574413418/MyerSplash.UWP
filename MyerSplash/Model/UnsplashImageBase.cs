using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using JP.Utils.Data.Json;
using JP.Utils.UI;
using MyerSplash.Common;
using MyerSplash.Interface;
using MyerSplashShared.API;
using MyerSplashShared.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Json;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Linq;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;

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

        public string ColorValue { get; set; }

        private double _width;
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
                return _createTime.ToString("yyyy-MM-dd hh-mm-ss");
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

        public string ShareText => $"Share {this.Owner.Name}'s amazing photo from MyerSplash app. {FullImageUrl}";

        public UnsplashImageBase()
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
            requestData.Properties.ContentSourceWebLink = new Uri(FullImageUrl);
            requestData.Properties.ContentSourceApplicationLink = new Uri(FullImageUrl);

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

        public async Task GetExifInfoAsync()
        {
            var result = await CloudService.GetImageDetail(ID, MyerSplashShared.API.CTSFactory.MakeCTS().Token);
            if (result.IsRequestSuccessful)
            {
                ParseExif(result.JsonSrc);
            }
        }

        private void ParseExif(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return;
            }
            var result = JsonObject.TryParse(json, out JsonObject obj);
            if (!result)
            {
                return;
            }

            Width = JsonParser.GetNumberFromJsonObj(obj, "width");
            Height = JsonParser.GetNumberFromJsonObj(obj, "height");

            Exif = new ImageExif();

            var exifObj = JsonParser.GetJsonObjFromJsonObj(obj, "exif");
            Exif.Model = JsonParser.GetStringFromJsonObj(exifObj, "model");
            Exif.ExposureTime = JsonParser.GetStringFromJsonObj(exifObj, "exposure_time");
            Exif.Aperture = JsonParser.GetStringFromJsonObj(exifObj, "aperture");
            Exif.Iso = (int)JsonParser.GetNumberFromJsonObj(exifObj, "iso");

            var location = new ImageLocation();
            var locationObj = JsonParser.GetJsonObjFromJsonObj(obj, "location");
            location.City = JsonParser.GetStringFromJsonObj(locationObj, "city");
            location.Country = JsonParser.GetStringFromJsonObj(locationObj, "country");

            Location = location;
        }

        public abstract void ParseObjectFromJsonString(string json);

        public abstract void ParseObjectFromJsonObject(JsonObject json);
    }
}
