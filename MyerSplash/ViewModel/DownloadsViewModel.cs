using GalaSoft.MvvmLight;
using MyerSplash.Model;
using MyerSplashCustomControl;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Windows.Networking.BackgroundTransfer;
using Windows.UI.Xaml;
using System;

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
                    value.CollectionChanged += Value_CollectionChanged;
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
            DownloadingImages = new ObservableCollection<DownloadItem>();
        }

        private void Value_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NoItemVisibility = DownloadingImages.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
        }

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
