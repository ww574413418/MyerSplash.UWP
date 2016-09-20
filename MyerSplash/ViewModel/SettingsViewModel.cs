using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using MyerSplashShared.API;
using MyerSplashCustomControl;

namespace MyerSplash.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        public static string SAVING_POSITION = "SAVING_POSITION";
        public static string DEFAULT_SAVING_POSITION = "\\Pictures\\MyerSplash (Can'be modified)";

        private RelayCommand _clearCacheCommand;
        public RelayCommand ClearCacheCommand
        {
            get
            {
                if (_clearCacheCommand != null) return _clearCacheCommand;
                return _clearCacheCommand = new RelayCommand(async () =>
                {
                    await ClearCacheAsync();
                });
            }
        }

        private RelayCommand _chooseSavingPositionCommand;
        public RelayCommand ChooseSavingPositionCommand
        {
            get
            {
                if (_chooseSavingPositionCommand != null) return _chooseSavingPositionCommand;
                return _chooseSavingPositionCommand = new RelayCommand(() =>
                  {
                      //FolderPicker savePicker = new FolderPicker();
                      //savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                      //savePicker.FileTypeFilter.Add(".jpg");
                      //var folder = await savePicker.PickSingleFolderAsync();
                      //if (folder != null)
                      //{
                      //    SavingPositionPath = folder.Path;
                      //    LocalSettingHelper.AddValue(SAVING_POSITION, SavingPositionPath);
                      //}
                  });
            }
        }

        private string _savingPositionPath;
        public string SavingPositionPath
        {
            get
            {
                return _savingPositionPath;
            }
            set
            {
                if (_savingPositionPath != value)
                {
                    _savingPositionPath = value;
                    RaisePropertyChanged(() => SavingPositionPath);
                }
            }
        }

        private string _cacheHint;
        public string CacheHint
        {
            get
            {
                return _cacheHint;
            }
            set
            {
                if (_cacheHint != value)
                {
                    _cacheHint = value;
                    RaisePropertyChanged(() => CacheHint);
                }
            }
        }


        public SettingsViewModel()
        {
            SavingPositionPath = DEFAULT_SAVING_POSITION;
            CacheHint = "Clean up cache";
        }

        public async Task CalculateCacheAsync()
        {
            ulong size = 0;
            var tempFiles = await CacheUtil.GetTempFolder().GetItemsAsync();
            foreach (var file in tempFiles)
            {
                var properties = await file.GetBasicPropertiesAsync();
                size += properties.Size;
                CacheHint = $"Clean up cache ({(size / (1024 * 1024)).ToString("f0")} MB)";
            }
        }

        private async Task ClearCacheAsync()
        {
            CacheHint = $"Clean up cache (0 MB)";
            ToastService.SendToast("All clear.", TimeSpan.FromMilliseconds(1000));

            var localFiles = await CacheUtil.GetCachedFileFolder().GetItemsAsync();
            foreach (var file in localFiles)
            {
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }

            var tempFiles = await CacheUtil.GetTempFolder().GetItemsAsync();
            foreach (var file in tempFiles)
            {
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }

        }
    }
}
