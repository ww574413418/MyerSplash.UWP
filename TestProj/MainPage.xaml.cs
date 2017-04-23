using MyerSplashShared.Utils;
using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TestProj
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var file = await KnownFolders.PicturesLibrary.CreateFileAsync("test.jpg", CreationCollisionOption.GenerateUniqueName);
            await FileDownloader.GetStreamFromUrlAsync("https://images.unsplash.com/photo-1492329856248-4d21e5165e6e?ixlib=rb-0.3.5&q=100&fm=jpg&crop=entropy&cs=tinysrgb&s=37af915f62521c4b5766c48863866dc8", null, file);
        }
    }
}
