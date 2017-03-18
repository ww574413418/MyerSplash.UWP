using JP.Utils.Data.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.System.UserProfile;

namespace MyerSplashShared.Utils
{
    public static class SimpleWallpaperSetter
    {
        public static async Task<bool> DownloadAndSetAsync(string url)
        {
            try
            {
                var client = new HttpClient();
                var result = await client.GetAsync(url);
                result.EnsureSuccessStatusCode();
                var str = await result.Content.ReadAsStringAsync();
                JsonObject obj;
                var json = JsonObject.TryParse(str, out obj);
                if (obj != null)
                {
                    var downloadUrl = JsonParser.GetStringFromJsonObj(obj, "url");
                    if (!string.IsNullOrEmpty(downloadUrl))
                    {
                        Debug.WriteLine($"===========url {downloadUrl}==============");

                        var imageResp = await client.GetAsync(downloadUrl);
                        var stream = await imageResp.Content.ReadAsStreamAsync();

                        Debug.WriteLine($"===========download complete==============");

                        var folder = ApplicationData.Current.LocalFolder;
                        StorageFile file = null;
                        try
                        {
                            file = await folder.CreateFileAsync("test.jpg", CreationCollisionOption.ReplaceExisting);
                            var bytes = new byte[stream.Length];
                            await stream.ReadAsync(bytes, 0, (int)stream.Length);
                            await FileIO.WriteBytesAsync(file, bytes);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e);
                        }

                        Debug.WriteLine($"===========save complete==============");

                        if (UserProfilePersonalizationSettings.IsSupported())
                        {
                            var setResult = await UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(file);
                            Debug.WriteLine($"===========TrySetWallpaperImageAsync result{setResult}=============");
                            return setResult;
                        }
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
