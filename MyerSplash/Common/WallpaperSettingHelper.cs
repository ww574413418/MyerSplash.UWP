using MyerSplash.View.Uc;
using MyerSplashCustomControl;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System.UserProfile;

namespace MyerSplash.Common
{
    public static class WallpaperSettingHelper
    {
        public static async Task SetAsBackgroundAsync(StorageFile savedFile)
        {
            var uc = new LoadingTextControl() { LoadingText = "Setting background and lockscreen..." };
            await PopupService.Instance.ShowAsync(uc);

            var file = await PrepareImageFileAsync(savedFile);
            if (file != null)
            {
                var result = await UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(file);

                //var task = file.DeleteAsync(StorageDeleteOption.PermanentDelete);

                if (result)
                {
                    ToastService.SendToast("Set as background and lock screen successfully.");
                }
                else
                {
                    ToastService.SendToast("Fail to set both. #API ERROR.");
                }
            }

            PopupService.Instance.TryHide(500);
        }

        public static async Task SetAsLockscreenAsync(StorageFile savedFile)
        {
            var uc = new LoadingTextControl() { LoadingText = "Setting background and lockscreen..." };
            await PopupService.Instance.ShowAsync(uc);

            var file = await PrepareImageFileAsync(savedFile);
            if (file != null)
            {
                var result = await UserProfilePersonalizationSettings.Current.TrySetLockScreenImageAsync(file);

                //var task = file.DeleteAsync(StorageDeleteOption.PermanentDelete);

                if (result)
                {
                    ToastService.SendToast("Set as background and lock screen successfully.");
                }
                else
                {
                    ToastService.SendToast("Fail to set both. #API ERROR.");
                }
            }

            PopupService.Instance.TryHide(500);
        }

        public static async Task SetBothAsync(StorageFile savedFile)
        {
            var uc = new LoadingTextControl() { LoadingText = "Setting background and lockscreen..." };
            await PopupService.Instance.ShowAsync(uc);

            var file = await PrepareImageFileAsync(savedFile);
            if (file != null)
            {
                var result1 = await UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(file);
                var result2 = await UserProfilePersonalizationSettings.Current.TrySetLockScreenImageAsync(file);

                //var task = file.DeleteAsync(StorageDeleteOption.PermanentDelete);

                if (result1 && result2)
                {
                    ToastService.SendToast("Set as background and lock screen successfully.");
                }
                else
                {
                    ToastService.SendToast("Fail to set both. #API ERROR.");
                }
            }

            PopupService.Instance.TryHide(500);
        }

        private static async Task<StorageFile> PrepareImageFileAsync(StorageFile resultFile)
        {
            if (!UserProfilePersonalizationSettings.IsSupported())
            {
                ToastService.SendToast("Your device can't set wallpaper.");
                return null;
            }
            if (resultFile != null)
            {
                StorageFile file = null;

                //WTF, the file should be copy to LocalFolder to make the setting wallpaer api work.
                var folder = ApplicationData.Current.LocalFolder;
                var oldFile = await folder.TryGetItemAsync(resultFile.Name) as StorageFile;
                if (oldFile != null)
                {
                    await resultFile.CopyAndReplaceAsync(oldFile);
                    file = oldFile;
                }
                else
                {
                    file = await resultFile.CopyAsync(folder);
                }
                return file;
            }
            return null;
        }
    }
}