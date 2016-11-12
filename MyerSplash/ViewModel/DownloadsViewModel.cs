using GalaSoft.MvvmLight;
using MyerSplash.Model;
using MyerSplashCustomControl;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Windows.Networking.BackgroundTransfer;
using Windows.UI.Xaml;
using System;
using MyerSplash.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.Storage;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;

namespace MyerSplash.ViewModel
{
    public class DownloadsViewModel : ViewModelBase
    {
        private ObservableCollection<DownloadItem> _downloadingImages;
        public ObservableCollection<DownloadItem> DownloadingImages
        {
            get
            {
                return _downloadingImages;
            }
            set
            {
                if (_downloadingImages != value)
                {
                    _downloadingImages = value;
                    RaisePropertyChanged(() => DownloadingImages);
                    _downloadingImages.CollectionChanged += Value_CollectionChanged;
                    NoItemVisibility = value.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                }
            }
        }

        private Visibility _noItemVisibility;
        public Visibility NoItemVisibility
        {
            get
            {
                return _noItemVisibility;
            }
            set
            {
                if (_noItemVisibility != value)
                {
                    _noItemVisibility = value;
                    RaisePropertyChanged(() => NoItemVisibility);
                }
            }
        }

        public DownloadsViewModel()
        {
            var task = RestoreListAsync();
        }

        private async void Value_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NoItemVisibility = DownloadingImages.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            await SaveListAsync();
        }

        public async Task SaveListAsync()
        {
            var str = JsonConvert.SerializeObject(DownloadingImages, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            });
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(CachedFileNames.DownloadListFileName, CreationCollisionOption.OpenIfExists);
            await FileIO.WriteTextAsync(file, str);
        }

#pragma warning disable 4014
        public async Task RestoreListAsync()
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(CachedFileNames.DownloadListFileName, CreationCollisionOption.OpenIfExists);
            if (file != null)
            {
                var str = await FileIO.ReadTextAsync(file);
                if (!string.IsNullOrEmpty(str))
                {
                    var list = JsonConvert.DeserializeObject<ObservableCollection<DownloadItem>>(str, new JsonSerializerSettings()
                    {
                        Error = (s, e) =>
                          {
                              var msg = e.ErrorContext.Error.Message;
                              ToastService.SendToast(msg);
                          },
                        TypeNameHandling = TypeNameHandling.All
                    });
                    if (list != null)
                    {
                        DownloadingImages = list;
                        foreach (var item in DownloadingImages)
                        {
                            item.CheckDownloadStatusAsync();
                        }
                    }
                    else
                    {
                        DownloadingImages = new ObservableCollection<DownloadItem>();
                    }
                }
            }
        }
#pragma warning restore

        public async void AddDownloadingImage(DownloadItem item)
        {
            DownloadingImages.Insert(0, item);
            var list = await BackgroundDownloader.GetCurrentDownloadsAsync();
            ToastService.SendToast(list.Count.ToString());
        }

        public void CancelDownload(DownloadItem item)
        {
            DownloadingImages.Remove(item);
        }
    }
}
