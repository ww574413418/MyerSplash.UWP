using JP.Utils.Data;
using JP.Utils.Debug;
using JP.Utils.Network;
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
            set
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

        public CachedBitmapSource()
        {

        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task LoadBitmapAsync()
        {
            if (Bitmap == null)
            {
                Bitmap = new BitmapImage();
            }
            if (!string.IsNullOrEmpty(LocalPath))
            {
                var folder = GetCachedFileFolder();
                var file = await folder.TryGetItemAsync(LocalPath);
                if (file != null)
                {
                    var buffer = await FileIO.ReadBufferAsync(file as StorageFile);
                    await Bitmap.SetSourceAsync(buffer.AsStream().AsRandomAccessStream());
                    return;
                }
                else
                {
                    await DownloadFromRemoteUrlAsync();
                }
            }
            else
            {
                await DownloadFromRemoteUrlAsync();
            }
        }

        private async Task DownloadFromRemoteUrlAsync()
        {
            var cachedFolder = GetCachedFileFolder();

            if (!string.IsNullOrEmpty(ExpectedFileName))
            {
                var file = await cachedFolder.TryGetFileAsync(ExpectedFileName);
                if (file != null)
                {
                    LocalPath = file.Path;
                    await SetSourceAsync(file);
                    return;
                }
            }
            else
            {
                ExpectedFileName = "temp.jpg";
            }
            using (var stream = await FileDownloadUtil.GetIRandomAccessStreamFromUrlAsync(this.RemoteUrl, CTSFactory.MakeCTS(20000).Token))
            {
                var file = await SaveStreamIntoFileAsync(stream.AsStream(), ExpectedFileName, cachedFolder);
                if (file != null)
                {
                    LocalPath = file.Path;
                }
                stream.Seek(0);
                if (stream != null)
                {
                    await Bitmap.SetSourceAsync(stream);
                }
            }
        }

        public async Task SetSourceAsync(IRandomAccessStream source)
        {
            if (Bitmap == null)
            {
                Bitmap = new BitmapImage();
            }
            await Bitmap.SetSourceAsync(source);
            RaisePropertyChanged(nameof(Bitmap));
        }

        public async Task SetSourceAsync(StorageFile source)
        {
            if (Bitmap == null)
            {
                Bitmap = new BitmapImage();
            }
            using (var fs = await source.OpenAsync(FileAccessMode.Read))
            {
                await SetSourceAsync(fs);
            }
        }

        private StorageFolder GetCachedFileFolder()
        {
            return ApplicationData.Current.LocalFolder;
        }

        /// <summary>
        /// 保存Stream到目标文件夹，并制定文件名
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="expectedFileName"></param>
        /// <param name="destinationFolder"></param>
        /// <returns></returns>
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
                var task = ExceptionHelper.WriteRecordAsync(e, nameof(CachedBitmapSource), nameof(SaveStreamIntoFileAsync), expectedFileName);
                return null;
            }
        }
    }
}
