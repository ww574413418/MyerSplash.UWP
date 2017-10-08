using JP.Utils.Data.Json;
using System;
using System.Diagnostics;
using System.IO;
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
            if (!UserProfilePersonalizationSettings.IsSupported())
            {
                return false;
            }

            try
            {
                var client = new HttpClient();
                var result = await client.GetAsync(url);
                result.EnsureSuccessStatusCode();
                var str = await result.Content.ReadAsStringAsync();
                var json = JsonObject.TryParse(str, out var obj);
                if (obj == null)
                {
                    return false;
                }
                var downloadUrl = JsonParser.GetStringFromJsonObj(obj, "url");
                if (!string.IsNullOrEmpty(downloadUrl))
                {
                    var fileName = Path.GetFileName(downloadUrl);

                    var pictureLib = await KnownFolders.PicturesLibrary.CreateFolderAsync("MyerSplash", CreationCollisionOption.OpenIfExists);
                    var targetFolder = await pictureLib.CreateFolderAsync("Auto-change wallpapers", CreationCollisionOption.OpenIfExists);
                    var localFolder = ApplicationData.Current.LocalFolder;

                    StorageFile file;
                    file = (StorageFile)await localFolder.TryGetItemAsync(fileName);

                    // Download
                    if (file == null)
                    {
                        Debug.WriteLine($"===========url {downloadUrl}==============");

                        var imageResp = await client.GetAsync(downloadUrl);
                        using (var stream = await imageResp.Content.ReadAsStreamAsync())
                        {
                            Debug.WriteLine($"===========download complete==============");

                            file = await targetFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                            var bytes = new byte[stream.Length];
                            await stream.ReadAsync(bytes, 0, (int)stream.Length);
                            await FileIO.WriteBytesAsync(file, bytes);

                            // File must be in local folder
                            file = await file.CopyAsync(localFolder, fileName, NameCollisionOption.ReplaceExisting);

                            Debug.WriteLine($"===========save complete==============");
                        }
                        if (file != null)
                        {
                            var setResult = false;
                            var value = (int)ApplicationData.Current.LocalSettings.Values["BackgroundWallpaperSource"];
                            switch (value)
                            {
                                case 0:
                                    break;

                                case 1:
                                    setResult = await UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(file);
                                    break;

                                case 2:
                                    setResult = await UserProfilePersonalizationSettings.Current.TrySetLockScreenImageAsync(file);
                                    break;

                                case 3:
                                    var setDesktopResult = await UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(file);
                                    var setLockscreenResult = await UserProfilePersonalizationSettings.Current.TrySetLockScreenImageAsync(file);
                                    setResult = setDesktopResult && setLockscreenResult;
                                    break;
                            }
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