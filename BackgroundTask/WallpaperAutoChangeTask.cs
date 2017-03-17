using JP.Utils.Data.Json;
using MyerSplashShared.API;
using MyerSplashShared.Shared;
using MyerSplashShared.Utils;
using System;
using System.Diagnostics;
using Windows.ApplicationModel.Background;
using Windows.Data.Json;

namespace BackgroundTask
{
    public sealed class WallpaperAutoChangeTask : IBackgroundTask
    {
        private const string KEY = "BackgroundWallpaperSource";

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine("===========background task run==============");
            var defer = taskInstance.GetDeferral();
            var url = GetRequestUrl();
            var resp = await CloudService.GetImages(0, 1, CTSFactory.MakeCTS().Token, url);
            if (resp.IsRequestSuccessful)
            {
                var downloadUrl = GetUrlFromJson(resp.JsonSrc);
                var cacheImage = new CachedBitmapSource();
                cacheImage.RemoteUrl = downloadUrl;
                cacheImage.ExpectedFileName = $"{DateTime.Now.ToUniversalTime().Ticks}.jpg";
                Debug.WriteLine("===========start download source image==============");
                await cacheImage.LoadBitmapAsync(false);
                Debug.WriteLine("===========download complete==============");
                var file = cacheImage.File;
                var result = await SimpleWallpaperSetter.SetAsync(file);
                Debug.WriteLine($"===========set wallpaper result={result}==============");
            }
            defer.Complete();
        }

        private string GetRequestUrl()
        {
            return UrlHelper.GetFeaturedImages;
            //var localSettings = ApplicationData.Current.LocalSettings;
            //if (localSettings.Values.ContainsKey(KEY))
            //{
            //    var option = (int)localSettings.Values[KEY];
            //    switch (option)
            //    {
            //        case 1: return UrlHelper.GetFeaturedImages;
            //        case 2: return UrlHelper.GetNewImages;
            //        case 3: return UrlHelper.GetRandomImages;
            //    }
            //}
            //return null;
        }

        private string GetUrlFromJson(string json)
        {
            var array = JsonArray.Parse(json);
            if (array.Count < 1)
            {
                return null;
            }
            var obj = JsonObject.Parse(array[0].ToString());
            var coverPhoto = JsonParser.GetJsonObjFromJsonObj(obj, "cover_photo");
            var urls = JsonParser.GetJsonObjFromJsonObj(coverPhoto, "urls");
            var fullImageUrl = JsonParser.GetStringFromJsonObj(urls, "full");
            return fullImageUrl;
        }
    }
}
