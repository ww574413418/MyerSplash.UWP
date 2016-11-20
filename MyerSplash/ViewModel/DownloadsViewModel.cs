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

namespace MyerSplash.ViewModel
{
    public class DownloadsViewModel : ViewModelBase
    {
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
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(CachedFileNames.DownloadListFileName, CreationCollisionOption.OpenIfExists);
            await FileIO.WriteTextAsync(file, str);
        }

#pragma warning disable
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
                              ToastService.SendToast(msg, 5000);
                          },
                        TypeNameHandling = TypeNameHandling.All
                    });
                    if (list != null)
                    {
                        DownloadingImages = list;
                        var downloadOpeations = await BackgroundDownloader.GetCurrentDownloadsAsync();
                        foreach (var item in DownloadingImages)
                        {
                            item.IsMenuOn = false;
                            item.CheckDownloadStatusAsync(downloadOpeations);
                            item.OnMenuStatusChanged += Item_OnMenuStatusChanged;
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
            if (DownloadingImages == null)
            {
                DownloadingImages = new ObservableCollection<DownloadItem>();
            }

            DownloadingImages.Insert(0, item);
            item.OnMenuStatusChanged += Item_OnMenuStatusChanged;
            var list = await BackgroundDownloader.GetCurrentDownloadsAsync();
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
            PopupService.Instance.TryToHide();
            DownloadItemsInternal(item => item.DisplayIndex == (int)DisplayMenu.Retry);
        }

        public void DeleteDownloading()
        {
            PopupService.Instance.TryToHide();
            DownloadItemsInternal(item => item.DisplayIndex == (int)DisplayMenu.Downloading);
        }

        public void DeleteDownloaded()
        {
            PopupService.Instance.TryToHide();
            DownloadItemsInternal(item => item.DisplayIndex == (int)DisplayMenu.SetAs);
        }
    }
}
