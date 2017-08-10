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
using Windows.Storage;
using System.Threading.Tasks;
using JP.Utils.Debug;
using GalaSoft.MvvmLight.Command;
using MyerSplash.UC;
using System.Linq;
using MyerSplash.Data;

namespace MyerSplash.ViewModel
{
    public class DownloadsViewModel : ViewModelBase
    {
        public static string CACHED_FILE_NAME => "DownloadList.list";

        private DownloadItem _menuOpenedItem;

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

        private RelayCommand _clearCommand;
        public RelayCommand ClearCommand
        {
            get
            {
                if (_clearCommand != null) return _clearCommand;
                return _clearCommand = new RelayCommand(async () =>
                  {
                      DownloadingImages?.Clear();
                      await SaveListAsync();
                  });
            }
        }

        private RelayCommand _deleteFABCommand;
        public RelayCommand DeleteFABCommand
        {
            get
            {
                if (_deleteFABCommand != null) return _deleteFABCommand;
                return _deleteFABCommand = new RelayCommand(async () =>
                  {
                      var dd = new DeleteDialogControl();
                      await PopupService.Instance.ShowAsync(dd);
                  });
            }
        }

        public DownloadsViewModel()
        {
            var task = RestoreListAsync();
        }

        private async void Value_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NoItemVisibility = DownloadingImages.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    await (item as DownloadItem).AwaitGuidCreatedAsync();
                }
            }
            await SaveListAsync();
        }

        public async Task SaveListAsync()
        {
            var str = JsonConvert.SerializeObject(DownloadingImages, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                Error = async (s, e) =>
                  {
                      await Logger.LogAsync(e.ErrorContext.Error);
                  }
            });
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(CACHED_FILE_NAME, CreationCollisionOption.OpenIfExists);
            await FileIO.WriteTextAsync(file, str);
        }

#pragma warning disable
        public async Task RestoreListAsync()
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(CACHED_FILE_NAME, CreationCollisionOption.OpenIfExists);
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
                          },
                        TypeNameHandling = TypeNameHandling.All
                    });
                    if (list != null)
                    {
                        DownloadingImages = list;
                        var downloadTasks = await BackgroundDownloader.GetCurrentDownloadsAsync();
                        foreach (var item in DownloadingImages)
                        {
                            item.IsMenuOn = false;
                            item.CheckDownloadStatusAsync(downloadTasks);
                            item.OnMenuStatusChanged += Item_OnMenuStatusChanged;
                            item.ImageItem.DownloadBitmapForListAsync();
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

        public async Task<DownloadItem> AddDownloadingImageAsync(DownloadItem item)
        {
            if (DownloadingImages == null)
            {
                DownloadingImages = new ObservableCollection<DownloadItem>();
            }

            var existItem = DownloadingImages.Where(s =>
             {
                 return s.ImageItem.Image.ID == item.ImageItem.Image.ID;
             }).FirstOrDefault();

            if (existItem != null)
            {
                if (existItem.DisplayIndex == (int)DisplayMenu.Retry)
                {
                    return existItem;
                }
            }

            DownloadingImages.Insert(0, item);
            item.OnMenuStatusChanged += Item_OnMenuStatusChanged;
            var list = await BackgroundDownloader.GetCurrentDownloadsAsync();

            return item;
        }

        private void Item_OnMenuStatusChanged(DownloadItem item, bool menuOpened)
        {
            if (_menuOpenedItem != null && menuOpened && _menuOpenedItem != item)
            {
                _menuOpenedItem.IsMenuOn = false;
            }
            if (menuOpened)
            {
                _menuOpenedItem = item;
            }
        }

        public void DeleteDownload(DownloadItem item)
        {
            DownloadingImages.Remove(item);
        }

        private void DownloadItemsInternal(Func<DownloadItem, bool> canDownload)
        {
            for (int i = 0; i < DownloadingImages.Count; i++)
            {
                var item = DownloadingImages[i];
                if (canDownload(item))
                {
                    DownloadingImages.Remove(item);
                    i--;
                }
            }
        }

        public void DeleteFailed()
        {
            PopupService.Instance.TryHide();
            DownloadItemsInternal(item => item.DisplayIndex == (int)DisplayMenu.Retry);
        }

        public void DeleteDownloading()
        {
            PopupService.Instance.TryHide();
            DownloadItemsInternal(item => item.DisplayIndex == (int)DisplayMenu.Downloading);
        }

        public void DeleteDownloaded()
        {
            PopupService.Instance.TryHide();
            DownloadItemsInternal(item => item.DisplayIndex == (int)DisplayMenu.SetAs);
        }
    }
}
