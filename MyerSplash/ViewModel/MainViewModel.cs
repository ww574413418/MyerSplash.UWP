using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using JP.Utils.Data;
using JP.Utils.Framework;
using MyerSplash.Common;
using MyerSplash.Model;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using MyerSplashShared.API;
using MyerSplash.ViewModel.DataViewModel;
using Windows.Storage;
using System;
using MyerSplashCustomControl;
using MyerSplash.View.Uc;
using JP.Utils.Helper;
using Windows.UI.ViewManagement;
using Microsoft.QueryStringDotNET;
using Windows.System;
using MyerSplash.Data;
using MyerSplashShared.Service;

namespace MyerSplash.ViewModel
{
    public class MainViewModel : ViewModelBase, INavigable
    {
        private const int RANDOM_INDEX = 0;
        private const int FEATURED_INDEX = 2;
        private const int NEW_INDEX = 1;

        private string DefaultTitleName
        {
            get
            {
                switch (AppSettings.Instance.DefaultCategory)
                {
                    case 0: return "RANDOM";
                    case 1: return "NEW";
                    case 2: return "FEATURED";
                    default: return "MyerSplash";
                }
            }
        }

        private ImageDataViewModel _dataVM;
        public ImageDataViewModel DataVM
        {
            get
            {
                return _dataVM;
            }
            set
            {
                if (_dataVM != value)
                {
                    _dataVM = value;
                    RaisePropertyChanged(() => DataVM);
                }
            }
        }

        private ObservableCollection<UnsplashCategory> _categories;
        public ObservableCollection<UnsplashCategory> Categories
        {
            get
            {
                return _categories;
            }
            set
            {
                if (_categories != value)
                {
                    _categories = value;
                    RaisePropertyChanged(() => Categories);
                }
            }
        }

        public bool IsInView { get; set; }

        public bool IsFirstActived { get; set; } = true;

        #region Search
        private bool _showSearchBar;
        public bool ShowSearchBar
        {
            get
            {
                return _showSearchBar;
            }
            set
            {
                if (_showSearchBar != value)
                {
                    _showSearchBar = value;
                    RaisePropertyChanged(() => ShowSearchBar);
                }
            }
        }

        private string _searchKeyword;
        public string SearchKeyword
        {
            get
            {
                return _searchKeyword;
            }
            set
            {
                if (_searchKeyword != value)
                {
                    _searchKeyword = value;
                    RaisePropertyChanged(() => SearchKeyword);
                }
            }
        }

        private RelayCommand _searchCommand;
        public RelayCommand SearchCommand
        {
            get
            {
                if (_searchCommand != null) return _searchCommand;
                return _searchCommand = new RelayCommand(() =>
                  {
                      DrawerOpened = false;
                      ShowSearchBar = true;
                      NavigationService.AddOperation(() =>
                          {
                              if (ShowSearchBar)
                              {
                                  ShowSearchBar = false;
                                  return true;
                              }
                              else return false;
                          });
                  });
            }
        }

        private RelayCommand _hideSearchCommand;
        public RelayCommand HideSearchCommand
        {
            get
            {
                if (_hideSearchCommand != null) return _hideSearchCommand;
                return _hideSearchCommand = new RelayCommand(() =>
                  {
                      ShowSearchBar = false;
                  });
            }
        }

        private RelayCommand _beginSearchCommand;
        public RelayCommand BeginSearchCommand
        {
            get
            {
                if (_beginSearchCommand != null) return _beginSearchCommand;
                return _beginSearchCommand = new RelayCommand(async () =>
                  {
                      if (ShowSearchBar)
                      {
                          SelectedIndex = -1;
                          ShowSearchBar = false;
                          await SearchByKeywordAsync();
                          SearchKeyword = "";
                      }
                  });
            }
        }
        #endregion

        private RelayCommand _refreshCommand;
        public RelayCommand RefreshCommand
        {
            get
            {
                if (_refreshCommand != null) return _refreshCommand;
                return _refreshCommand = new RelayCommand(async () =>
                  {
                      await RefreshAllAsync();
                  });
            }
        }

        private RelayCommand _retryCommand;
        public RelayCommand RetryCommand
        {
            get
            {
                if (_retryCommand != null) return _retryCommand;
                return _retryCommand = new RelayCommand(async () =>
                  {
                      FooterLoadingVisibility = Visibility.Visible;
                      FooterReloadVisibility = Visibility.Collapsed;
                      await DataVM.RetryAsync();
                  });
            }
        }

        private RelayCommand _openDrawerCommand;
        public RelayCommand OpenDrawerCommand
        {
            get
            {
                if (_openDrawerCommand != null) return _openDrawerCommand;
                return _openDrawerCommand = new RelayCommand(() =>
                  {
                      DrawerOpened = !DrawerOpened;
                      if (DrawerOpened)
                      {
                          NavigationService.AddOperation(() =>
                          {
                              if (DrawerOpened)
                              {
                                  DrawerOpened = false;
                                  return true;
                              }
                              else return false;
                          });
                      }
                  });
            }
        }

        private bool _drawerOpened;
        public bool DrawerOpened
        {
            get
            {
                return _drawerOpened;
            }
            set
            {
                if (_drawerOpened != value)
                {
                    _drawerOpened = value;
                    RaisePropertyChanged(() => DrawerOpened);
                }
            }
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get
            {
                return _isRefreshing;
            }
            set
            {
                if (_isRefreshing != value)
                {
                    _isRefreshing = value;
                    RaisePropertyChanged(() => IsRefreshing);
                }
            }
        }

        private Visibility _footerLoadingVisibility;
        public Visibility FooterLoadingVisibility
        {
            get
            {
                return _footerLoadingVisibility;
            }
            set
            {
                if (_footerLoadingVisibility != value)
                {
                    _footerLoadingVisibility = value;
                    RaisePropertyChanged(() => FooterLoadingVisibility);
                }
            }
        }

        private Visibility _endVisiblity;
        public Visibility EndVisibility
        {
            get
            {
                return _endVisiblity;
            }
            set
            {
                if (_endVisiblity != value)
                {
                    _endVisiblity = value;
                    RaisePropertyChanged(() => EndVisibility);
                }
            }
        }

        private Visibility _noItemHintVisibility;
        public Visibility NoItemHintVisibility
        {
            get
            {
                return _noItemHintVisibility;
            }
            set
            {
                if (_noItemHintVisibility != value)
                {
                    _noItemHintVisibility = value;
                    RaisePropertyChanged(() => NoItemHintVisibility);
                }
            }
        }

        private Visibility _noNetworkHintVisibility;
        public Visibility NoNetworkHintVisibility
        {
            get
            {
                return _noNetworkHintVisibility;
            }
            set
            {
                if (_noNetworkHintVisibility != value)
                {
                    _noNetworkHintVisibility = value;
                    RaisePropertyChanged(() => NoNetworkHintVisibility);
                }
            }
        }

        private Visibility _footerReloadVisibility;
        public Visibility FooterReloadVisibility
        {
            get
            {
                return _footerReloadVisibility;
            }
            set
            {
                if (_footerReloadVisibility != value)
                {
                    _footerReloadVisibility = value;
                    RaisePropertyChanged(() => FooterReloadVisibility);
                }
            }
        }

        private RelayCommand _goToSettingsCommand;
        public RelayCommand GoToSettingsCommand
        {
            get
            {
                if (_goToSettingsCommand != null) return _goToSettingsCommand;
                return _goToSettingsCommand = new RelayCommand(() =>
                  {
                      DrawerOpened = false;
                      ShowSettingsUC = true;
                      NavigationService.AddOperation(() =>
                          {
                              if (ShowSettingsUC)
                              {
                                  ShowSettingsUC = false;
                                  return true;
                              }
                              return false;
                          });
                  });
            }
        }

        private bool _showAboutUC;
        public bool ShowAboutUC
        {
            get
            {
                return _showAboutUC;
            }
            set
            {
                if (_showAboutUC != value)
                {
                    _showAboutUC = value;
                    RaisePropertyChanged(() => ShowAboutUC);
                }
            }
        }

        private bool _showDownloadsUC;
        public bool ShowDownloadsUC
        {
            get
            {
                return _showDownloadsUC;
            }
            set
            {
                if (_showDownloadsUC != value)
                {
                    _showDownloadsUC = value;
                    RaisePropertyChanged(() => ShowDownloadsUC);
                }
            }
        }

        private bool _showSettingsUC;
        public bool ShowSettingsUC
        {
            get
            {
                return _showSettingsUC;
            }
            set
            {
                if (_showSettingsUC != value)
                {
                    _showSettingsUC = value;
                    RaisePropertyChanged(() => ShowSettingsUC);
                }
            }
        }

        private RelayCommand _showDownloadsCommand;
        public RelayCommand ShowDownloadsCommand
        {
            get
            {
                if (_showDownloadsCommand != null) return _showDownloadsCommand;
                return _showDownloadsCommand = new RelayCommand(() =>
                  {
                      ShowDownloadsUC = !ShowDownloadsUC;
                      DrawerOpened = false;

                      if (ShowDownloadsUC)
                      {
                          NavigationService.AddOperation(() =>
                          {
                              if (ShowDownloadsUC)
                              {
                                  ShowDownloadsUC = false;
                                  return true;
                              }
                              return false;
                          });
                      }
                  });
            }
        }

        private RelayCommand _goToAboutCommand;
        public RelayCommand GoToAboutCommand
        {
            get
            {
                if (_goToAboutCommand != null) return _goToAboutCommand;
                return _goToAboutCommand = new RelayCommand(() =>
                  {
                      DrawerOpened = false;
                      ShowAboutUC = true;
                      NavigationService.AddOperation(() =>
                          {
                              if (ShowAboutUC)
                              {
                                  ShowAboutUC = false;
                                  return true;
                              }
                              return false;
                          });
                  });
            }
        }

        private RelayCommand _toggleFullScreenCommand;
        public RelayCommand ToggleFullScreenCommand
        {
            get
            {
                if (_toggleFullScreenCommand != null) return _toggleFullScreenCommand;
                return _toggleFullScreenCommand = new RelayCommand(() =>
                  {
                      var isInFullScreen = ApplicationView.GetForCurrentView().IsFullScreenMode;
                      if (!isInFullScreen)
                      {
                          ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
                      }
                      else
                      {
                          ApplicationView.GetForCurrentView().ExitFullScreenMode();
                      }
                      DrawerOpened = false;
                  });
            }
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                if (_selectedIndex != value)
                {
                    var lastValue = value;

                    _selectedIndex = value;
                    RaisePropertyChanged(() => SelectedIndex);
                    RaisePropertyChanged(() => SelectedTitle);
                    DrawerOpened = false;
                    if (value == -1)
                    {
                        return;
                    }

                    // From search to category
                    if (lastValue != -1)
                    {
                        if (value == NEW_INDEX)
                        {
                            DataVM = new ImageDataViewModel(this,
                                new ImageService(Request.GetNewImages, NormalFactory));
                        }
                        else if (value == FEATURED_INDEX)
                        {
                            DataVM = new ImageDataViewModel(this,
                                new ImageService(Request.GetFeaturedImages, FeaturedFactory));
                        }
                        else if (value == RANDOM_INDEX)
                        {
                            DataVM = new RandomImagesDataViewModel(this,
                                new RandomImageService(NormalFactory));
                        }
                        else if (value > NEW_INDEX)
                        {
                            DataVM = new SearchResultViewModel(this, 
                                new SearchImageService(NormalFactory, Categories[value].Title));
                        }
                        if (DataVM != null)
                        {
                            var task = RefreshListAsync();
                        }
                    }
                }
            }
        }

        public string SelectedTitle
        {
            get
            {
                var name = "";
                if (SelectedIndex == -1)
                {
                    if (SearchKeyword == null)
                    {
                        name = DefaultTitleName;
                    }
                    else name = SearchKeyword.ToUpper();
                }
                else if (Categories?.Count > 0)
                {
                    name = Categories[SelectedIndex].Title.ToUpper();
                }
                else name = DefaultTitleName;
                return $"{name}";
            }
        }

        private UnsplashImageFactory _normalFactory;
        public UnsplashImageFactory NormalFactory
        {
            get
            {
                return _normalFactory ?? (_normalFactory = new UnsplashImageFactory(false));
            }
        }

        private UnsplashImageFactory _featuredFactory;
        public UnsplashImageFactory FeaturedFactory
        {
            get
            {
                return _featuredFactory ?? (_featuredFactory = new UnsplashImageFactory(true));
            }
        }

        public MainViewModel()
        {
            FooterLoadingVisibility = Visibility.Collapsed;
            NoItemHintVisibility = Visibility.Collapsed;
            NoNetworkHintVisibility = Visibility.Collapsed;
            FooterReloadVisibility = Visibility.Collapsed;
            EndVisibility = Visibility.Collapsed;
            IsRefreshing = true;
            ShowDownloadsUC = false;

            SelectedIndex = -1;

            DataVM = new ImageDataViewModel(this,
                new ImageService(Request.GetNewImages, NormalFactory));
        }

        private async Task SearchByKeywordAsync()
        {
            var searchService = new SearchImageService(NormalFactory, SearchKeyword);
            DataVM = new SearchResultViewModel(this, searchService);
            RaisePropertyChanged(() => SelectedTitle);
            await RefreshListAsync();
        }

        private async Task RefreshAllAsync()
        {
            var task1 = GetCategoriesAsync();
            await RefreshListAsync();
        }

        private async Task RefreshListAsync()
        {
            IsRefreshing = true;
            await DataVM.RefreshAsync();
            if (SelectedIndex == NEW_INDEX)
            {
                var date = DateTime.Now.ToString("yyyyMMdd");

                if (DataVM.DataList.Count > 0 && DataVM.DataList[0].Image.ID != date)
                {
                    var image = UnsplashImageFactory.CreateRecommendationImage();
                    var imageItem = new ImageItem(image);
                    DataVM.DataList.Insert(0, imageItem);
                    imageItem.Init();
                    await imageItem.DownloadBitmapForListAsync();
                }
            }
            IsRefreshing = false;
        }

        private async Task GetCategoriesAsync()
        {
            if (Categories?.Count > 0) return;
            Categories = await UnsplashCategoryFactory.GetCategoriesAsync();
            SelectedIndex = NEW_INDEX;
        }

        public void Activate(object param)
        {
            var task = HandleLaunchArg(param as string);

            if (DeviceHelper.IsDesktop)
            {
                var key = (string)App.Current.Resources["CoachKey"];
                if (!LocalSettingHelper.HasValue(key))
                {
                    LocalSettingHelper.AddValue(key, true);
                    var uc = new TipsControl();
                    var task2 = PopupService.Instance.ShowAsync(uc);
                }
            }
        }

        private async Task HandleLaunchArg(string arg)
        {
            if (arg == Value.SEARCH)
            {
                ShowSearchBar = true;
            }
            else if (arg == Value.DOWNLOADS)
            {
                ShowDownloadsUC = true;
            }
            else
            {
                var queryStr = QueryString.Parse(arg);
                var action = queryStr[Key.ACTION_KEY];
                if (!queryStr.Contains(Key.FILE_PATH_KEY))
                {
                    return;
                }
                var filePath = queryStr[Key.FILE_PATH_KEY];
                if (filePath != null)
                {
                    switch (action)
                    {
                        case Value.SET_AS:
                            await WallpaperSettingHelper.SetAsBackgroundAsync(await StorageFile.GetFileFromPathAsync(filePath));
                            break;
                        case Value.VIEW:
                            await Launcher.LaunchFileAsync(await StorageFile.GetFileFromPathAsync(filePath));
                            break;
                    }
                }
            }
        }

        public void Deactivate(object param)
        {

        }

        public async void OnLoaded()
        {
            if (IsFirstActived)
            {
                IsFirstActived = false;
                await RefreshAllAsync();
            }
        }
    }
}
