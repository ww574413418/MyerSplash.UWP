using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace MyerSplashShared.Utils
{
    public static class CacheUtil
    {
        public static async Task CleanUpAsync()
        {
            var tempFolder = GetCachedFileFolder();
            var items = await tempFolder.GetItemsAsync();
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
