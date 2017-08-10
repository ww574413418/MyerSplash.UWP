using GalaSoft.MvvmLight;
using MyerSplashShared.Utils;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace MyerSplash.Common
{
    public class AppSettings : ViewModelBase
    {
        public ApplicationDataContainer LocalSettings { get; set; }

        public bool EnableTile
        {
            get
            {
                return ReadSettings(nameof(EnableTile), true);
            }
            set
            {
                SaveSettings(nameof(EnableTile), value);
                RaisePropertyChanged(() => EnableTile);
                if (!value)
                {
                    LiveTileUpdater.CleanUpTile();
                }
            }
        }

        public bool EnableQuickDownload
        {
            get
            {
                return ReadSettings(nameof(EnableQuickDownload), false);
            }
            set
            {
                SaveSettings(nameof(EnableQuickDownload), value);
                RaisePropertyChanged(() => EnableQuickDownload);
            }
        }

        public bool EnableScaleAnimation
        {
            get
            {
                return ReadSettings(nameof(EnableScaleAnimation), true);
            }
            set
            {
                SaveSettings(nameof(EnableScaleAnimation), value);
                RaisePropertyChanged(() => EnableScaleAnimation);
            }
        }

        public string SaveFolderPath
        {
            get
            {
                return ReadSettings(nameof(SaveFolderPath), "");
            }
            set
            {
                SaveSettings(nameof(SaveFolderPath), value);
                RaisePropertyChanged(() => SaveFolderPath);
            }
        }

        public int DefaultCategory
        {
            get
            {
                return ReadSettings(nameof(DefaultCategory), 1);
            }
            set
            {
                SaveSettings(nameof(DefaultCategory), value);
                RaisePropertyChanged(() => DefaultCategory);
            }
        }

        public int BackgroundWallpaperSource
        {
            get
            {
                return ReadSettings(nameof(BackgroundWallpaperSource), 0);
            }
            set
            {
                SaveSettings(nameof(BackgroundWallpaperSource), value);
                RaisePropertyChanged(() => BackgroundWallpaperSource);
                switch (value)
                {
                    case 0:
                        var task0 = BackgroundTaskRegister.UnregisterAsync();
                        break;
                    case 1:
                    // fall through
                    case 2:
                    // fall through
                    case 3:
                        var task1 = BackgroundTaskRegister.RegisterAsync();
                        break;
                }
            }
        }

        public int LoadQuality
        {
            get
            {
                return ReadSettings(nameof(LoadQuality), 0);
            }
            set
            {
                SaveSettings(nameof(LoadQuality), value);
                RaisePropertyChanged(() => LoadQuality);
            }
        }

        public int SaveQuality
        {
            get
            {
                return ReadSettings(nameof(SaveQuality), 1);
            }
            set
            {
                SaveSettings(nameof(SaveQuality), value);
                RaisePropertyChanged(() => SaveQuality);
            }
        }

        public AppSettings()
        {
            LocalSettings = ApplicationData.Current.LocalSettings;
        }

        public async Task<StorageFolder> GetSavingFolderAsync()
        {
            var folder = await KnownFolders.PicturesLibrary.CreateFolderAsync("MyerSplash", CreationCollisionOption.OpenIfExists);
            return folder;
        }

        public async Task<StorageFolder> GetWallpaperFolderAsync()
        {
            var folder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync("WallpapersTemp", CreationCollisionOption.OpenIfExists);
            return folder;
        }

        private void SaveSettings(string key, object value)
        {
            LocalSettings.Values[key] = value;
        }

        private T ReadSettings<T>(string key, T defaultValue)
        {
            if (LocalSettings.Values.ContainsKey(key))
            {
                return (T)LocalSettings.Values[key];
            }
            if (defaultValue != null)
            {
                return defaultValue;
            }
            return default(T);
        }

        private static readonly Lazy<AppSettings> lazy = new Lazy<AppSettings>(() => new AppSettings());

        public static AppSettings Instance { get { return lazy.Value; } }
    }
}
