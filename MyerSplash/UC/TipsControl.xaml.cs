using ImageLib;
using ImageLib.Cache.Storage;
using ImageLib.Gif;
using MyerSplashCustomControl;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyerSplash.UC
{
    public sealed partial class TipsControl : UserControl
    {
        public TipsControl()
        {
            this.Init();
            this.InitializeComponent();
        }

        private void Init()
        {
            var config = new ImageConfig.Builder()
                .LimitedStorageCache(ApplicationData.Current.LocalCacheFolder, "cache", new SHA1CacheGenerator(), 1024 * 1024 * 1024)
                .NewApi(false)
                .AddDecoder<GifDecoder>()
                .Build();
            ImageLoader.Initialize(config);
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            PopupService.Instance.TryHide();
        }
    }
}
