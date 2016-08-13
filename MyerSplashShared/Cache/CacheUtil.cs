using JP.API;
using JP.Utils.Data;
using JP.Utils.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MyerSplashShared.API
{
    public static class CacheUtil
    {
        public static async Task CleanUpAsync()
        {
            var items = await GetCachedFileFolder().GetItemsAsync();
            foreach (var item in items)
            {
                await item.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
        }

        public static StorageFolder GetCachedFileFolder()
        {
            return ApplicationData.Current.LocalFolder;
        }

        public static StorageFolder GetTempFolder()
        {
            return ApplicationData.Current.TemporaryFolder;
        }
    }
}
