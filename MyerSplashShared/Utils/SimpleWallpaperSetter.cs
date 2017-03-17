using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System.UserProfile;

namespace MyerSplashShared.Utils
{
    public static class SimpleWallpaperSetter
    {
        public static async Task<bool> SetAsync(StorageFile file)
        {
            if (UserProfilePersonalizationSettings.IsSupported())
            {
                var folder = ApplicationData.Current.LocalFolder;
                var oldFile = await folder.TryGetItemAsync(file.Name) as StorageFile;
                if (oldFile != null)
                {
                    await oldFile.DeleteAsync();
                }
                file = await file.CopyAsync(folder);
                var result = await UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(file);
                Debug.WriteLine($"===========set result {result}==============");
                return result;
            }
            return false;
        }
    }
}
