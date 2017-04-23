using JP.Utils.Data;
using JP.Utils.Debug;
using MyerSplashShared.API;
using MyerSplashShared.Utils;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace MyerSplashShared.Shared
{
    public class CachedBitmapSource : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private BitmapImage _bitmap;
        [IgnoreDataMember]
        public BitmapImage Bitmap
        {
            get
            {
                return _bitmap;
            }
            private set
            {
                if (_bitmap != value)
                {
                    _bitmap = value;
                    RaisePropertyChanged(nameof(Bitmap));
                }
            }
        }

        public string RemoteUrl { get; set; }

        public string LocalPath { get; set; }

        public string ExpectedFileName { get; set; }

        [IgnoreDataMember]
        public StorageFile File { get; set; }

        public CachedBitmapSource()
        {

        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task LoadBitmapAsync(bool setBitmap = true)
        {
            if (!string.IsNullOrEmpty(LocalPath))
            {
                var file = await StorageFile.GetFileFromPathAsync(LocalPath);
                if (file != null)
                {
                    await SetImageSourceAsync(file as StorageFile);
                    return;
                }
            }
            await DownloadFromRemoteUrlAsync(setBitmap);
        }

        private async Task DownloadFromRemoteUrlAsync(bool setBitmap = true)
        {
            var cachedFolder = ApplicationData.Current.TemporaryFolder;

            if (!string.IsNullOrEmpty(ExpectedFileName))
            {
                var file = await cachedFolder.TryGetFileAsync(ExpectedFileName);
                if (file != null)
                {
                    LocalPath = file.Path;
                    File = file;
                    await SetImageSourceAsync(file);
                    return;
                }
            }
            else
            {
                ExpectedFileName = GenerateRandomFileName();
            }
            using (var stream = await FileDownloader.GetIRandomAccessStreamFromUrlAsync(this.RemoteUrl, CTSFactory.MakeCTS().Token))
            {
                var file = await SaveStreamIntoFileAsync(stream.AsStream(), ExpectedFileName, cachedFolder);
                if (file != null)
                {
                    LocalPath = file.Path;
                    File = file;
                }
                stream.Seek(0);
                if (stream != null && setBitmap)
                {
                    await SetImageSourceAsync(stream);
                }
            }
        }

        public async Task SetImageSourceAsync(IRandomAccessStream source)
        {
            Bitmap = new BitmapImage();
            await Bitmap.SetSourceAsync(source);
        }

        public async Task SetImageSourceAsync(StorageFile source)
        {
            using (var fs = await source.OpenAsync(FileAccessMode.Read))
            {
                await SetImageSourceAsync(fs);
            }
        }

        public void SetBitmap(BitmapImage targetBitmap)
        {
            Bitmap = targetBitmap;
        }

        private string GenerateRandomFileName()
        {
            return DateTime.Now.ToFileTime().ToString() + ".jpg";
        }

        private async Task<StorageFile> SaveStreamIntoFileAsync(Stream stream, string expectedFileName,
            StorageFolder destinationFolder)
        {
            try
            {
                var file = await destinationFolder.CreateFileAsync(expectedFileName, CreationCollisionOption.ReplaceExisting);
                var bytes = new byte[stream.Length];
                await stream.ReadAsync(bytes, 0, (int)stream.Length);
                await FileIO.WriteBytesAsync(file, bytes);
                return file;
            }
            catch (Exception e)
            {
                var task = Logger.LogAsync(e);
                return null;
            }
        }
    }
}
