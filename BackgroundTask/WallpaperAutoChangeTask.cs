using MyerSplashShared.Utils;
using System.Diagnostics;
using Windows.ApplicationModel.Background;

namespace BackgroundTask
{
    public sealed class WallpaperAutoChangeTask : IBackgroundTask
    {
        private const string KEY = "BackgroundWallpaperSource";
        private const string URL = "http://juniperphoton.net/schedule/Wallpaper/GetWallpaper/v2";

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine("===========background task run==============");
            var defer = taskInstance.GetDeferral();
            var result = await SimpleWallpaperSetter.DownloadAndSetAsync(URL);
            Debug.WriteLine($"===========result {result}==============");
            defer.Complete();
        }
    }
}