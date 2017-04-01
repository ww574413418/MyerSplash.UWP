using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using JP.Utils.Data;
using JP.Utils.Framework;
using MyerSplash.Common;
using MyerSplash.Model;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using MyerSplashShared.API;
using MyerSplash.ViewModel.DataViewModel;
using System.Collections.Generic;
using Windows.Storage;
using System;
using Newtonsoft.Json;
using MyerSplashCustomControl;
using MyerSplash.UC;
using JP.Utils.Helper;
using Windows.UI.ViewManagement;
using Windows.UI;
using JP.Utils.UI;

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

        private ObservableCollection<ColorFilter> _colorFilterList;
        public ObservableCollection<ColorFilter> ColorFilterList
        {
            get
            {
                return _colorFilterList;
            }
            set
            {
                if (_colorFilterList != value)
                {
                    _colorFilterList = value;
                    RaisePropertyChanged(() => ColorFilterList);
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

        public ColorFilter ColorToFilter { get; set; }

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
                      ShowDownloadsUC = true;
                      DrawerOpened = false;
                      NavigationService.AddOperation(() =>
                      {
                          if (ShowDownloadsUC)
                          {
                              ShowDownloadsUC = false;
                              return true;
                          }
                          return false;
                      });
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
                            DataVM = new ImageDataViewModel(UrlHelper.GetNewImages, false, null);
                        }
                        else if (value == FEATURED_INDEX)
                        {
                            DataVM = new ImageDataViewModel(UrlHelper.GetFeaturedImages, true, null);
                        }
                        else if (value == RANDOM_INDEX)
                        {
                            DataVM = new RandomImagesDataViewModel(UrlHelper.GetRandomImages, false, null);
                        }
                        else if (value > NEW_INDEX)
                        {
                            DataVM = new ImageDataViewModel(Categories[value].RequestUrl, false, null);
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
                    if (SearchKeyword != null)
                    {
                        name = SearchKeyword.ToUpper();
                    }
                    else if (ColorToFilter != null)
                    {
                        name = ColorToFilter.ColorName.Replace("#", "");
                    }
                }
                else if (Categories?.Count > 0)
                {
                    name = Categories[SelectedIndex].Title.ToUpper();
                }

                if (string.IsNullOrEmpty(name))
                {
                    name = DefaultTitleName;
                }
                return $"# {name}";
            }
        }

        private RelayCommand<ColorFilter> _tapColorCommand;
        public RelayCommand<ColorFilter> TapColorCommand
        {
            get
            {
                if (_tapColorCommand != null) return _tapColorCommand;
                return _tapColorCommand = new RelayCommand<ColorFilter>(async (filter) =>
                  {
                      ShowSearchBar = false;
                      SelectedIndex = -1;
                      ColorToFilter = filter;
                      RaisePropertyChanged(() => SelectedTitle);
                      await SearchByColorAsync();
                  });
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

            App.MainVM = this;

            SelectedIndex = -1;
        }

        private async Task SearchByKeywordAsync()
        {
            DataVM = new SearchResultViewModel(UrlHelper.SearchImages, SearchKeyword);
            RaisePropertyChanged(() => SelectedTitle);
            await RefreshListAsync();
        }

        private async Task SearchByColorAsync()
        {
            DataVM = new RandomImagesDataViewModel(UrlHelper.GetRandomImages, false, (image) =>
             {
                 var imageR = image.MajorColor.Color.R;
                 var imageG = image.MajorColor.Color.G;
                 var imageB = image.MajorColor.Color.B;

                 var filterR = ColorToFilter.Color.R;
                 var filterG = ColorToFilter.Color.G;
                 var filterB = ColorToFilter.Color.B;

                 var distance = Math.Sqrt(Math.Pow(imageR - filterR, 2) + Math.Pow(imageG - filterG, 2)
                     + Math.Pow(imageB - filterB, 2));

                 return distance < 50;
             });
            if (DataVM != null)
            {
                await RefreshListAsync();
            }
        }

        private async Task RestoreMainListDataAsync()
        {
            InitDataVM();
            if (AppSettings.Instance.DefaultCategory == 0)
            {
                return;
            }
            var file = await CacheUtil.GetCachedFileFolder().TryGetFileAsync(CachedFileNames.MainListFileName);
            if (file != null)
            {
                var str = await FileIO.ReadTextAsync(file);
                var list = JsonConvert.DeserializeObject<List<UnsplashImage>>(str);
                if (list != null)
                {
                    list.ForEach(s => DataVM.DataList.Add(s));

                    for (int i = 0; i < DataVM.DataList.Count; i++)
                    {
                        var item = DataVM.DataList[i];
                        if (i % 2 == 0) item.BackColor = Application.Current.Resources["ImageBackBrush1"] as SolidColorBrush;
                        else item.BackColor = Application.Current.Resources["ImageBackBrush2"] as SolidColorBrush;
                        var task = item.RestoreDataAsync();
                    }

                    return;
                }
            }
        }

        private void InitDataVM()
        {
            if (!string.IsNullOrEmpty(_launcherArg))
            {
                if (_launcherArg == Constant.RANDOM_KEY)
                {
                    DataVM = new RandomImagesDataViewModel(UrlHelper.GetRandomImages, false, null);
                    return;
                }
                else if (_launcherArg == Constant.SEARCH_KEY)
                {
                    ShowSearchBar = true;
                }
            }
            switch (AppSettings.Instance.DefaultCategory)
            {
                case 0:
                    {
                        DataVM = new RandomImagesDataViewModel(UrlHelper.GetRandomImages, false, null);
                    }; break;
                case 1:
                    {
                        DataVM = new ImageDataViewModel(UrlHelper.GetNewImages, false, null);
                    }; break;
                case 2:
                    {
                        DataVM = new ImageDataViewModel(UrlHelper.GetFeaturedImages, true, null);
                    }; break;
            }
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
            IsRefreshing = false;
            if (SelectedIndex == 1)
            {
                await SaveMainListDataAsync();
            }
        }

        private async Task GetCategoriesAsync()
        {
            if (Categories?.Count > 0) return;

            var result = await CloudService.GetCategories(CTSFactory.MakeCTS().Token);
            if (result.IsRequestSuccessful)
            {
                var list = UnsplashCategory.GenerateListFromJson(result.JsonSrc);
                this.Categories = list;
                this.Categories.Insert(0, new UnsplashCategory()
                {
                    Title = "Featured",
                });
                this.Categories.Insert(0, new UnsplashCategory()
                {
                    Title = "New",
                });
                this.Categories.Insert(0, new UnsplashCategory()
                {
                    Title = "Random",
                });
                SelectedIndex = App.AppSettings.DefaultCategory;
                await SerializerHelper.SerializerToJson<ObservableCollection<UnsplashCategory>>(list, CachedFileNames.CateListFileName, CacheUtil.GetCachedFileFolder());
            }
        }

        private async Task RestoreCategoriyListAsync()
        {
            this.Categories = await SerializerHelper.DeserializeFromJsonByFile<ObservableCollection<UnsplashCategory>>(CachedFileNames.CateListFileName);
            SelectedIndex = App.AppSettings.DefaultCategory;
        }

        private async Task SaveMainListDataAsync()
        {
            if (this.DataVM.DataList?.Count > 0)
            {
                var list = new List<UnsplashImage>();
                foreach (var item in DataVM.DataList)
                {
                    if (item is UnsplashImage)
                    {
                        list.Add(item as UnsplashImage);
                    }
                }
                if (list.Count > 0)
                {
                    ToastService.SendToast("Fetched :D");

                    var str = JsonConvert.SerializeObject(list, new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.All
                    });
                    var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(CachedFileNames.MainListFileName, CreationCollisionOption.OpenIfExists);
                    await FileIO.WriteTextAsync(file, str);
                }
            }
        }

        private string _launcherArg;

        public void Activate(object param)
        {
            _launcherArg = param as string;
            if (_launcherArg == Constant.SEARCH_KEY)
            {
                ShowSearchBar = true;
            }
            if (DeviceHelper.IsDesktop)
            {
                if (!LocalSettingHelper.HasValue("TIPS252"))
                {
                    LocalSettingHelper.AddValue("TIPS252", true);
                    var uc = new TipsControl();
                    var task = PopupService.Instance.ShowAsync(uc);
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
                PrepareColorFilterList();
                await RestoreMainListDataAsync();
                await RefreshAllAsync();
            }
        }

        private void PrepareColorFilterList()
        {
            ColorFilterList = new ObservableCollection<ColorFilter>();
            ColorFilterList.Add(new ColorFilter("#f44336".ToColor(), "RED"));
            ColorFilterList.Add(new ColorFilter("#fec006".ToColor(), "AMBER"));
            ColorFilterList.Add(new ColorFilter("#00bbd3".ToColor(), "CYAN"));
            ColorFilterList.Add(new ColorFilter("#2095f2".ToColor(), "BLUE"));
            ColorFilterList.Add(new ColorFilter("#3e50b4".ToColor(), "INDIGO"));
            ColorFilterList.Add(new ColorFilter("#5f7c8a".ToColor(), "GREY"));
        }
    }
}
