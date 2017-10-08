using ImageLib;
using ImageLib.Cache.Storage;
using ImageLib.Gif;
using MyerSplashCustomControl;
using System;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyerSplash.View.Uc
{
    public sealed partial class TipsControl : UserControl
    {
        public TipsControl()
        {
            this.InitGif();
            this.InitializeComponent();
        }

        private void InitGif()
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

        private async void LaunchBotButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://t.me/myersplashbot"));
        }
    }
}