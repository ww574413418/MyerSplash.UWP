using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using JP.Utils.Data;
using JP.Utils.Framework;
using MyerSplash.Common;
using MyerSplash.Model;
using MyerSplash.View;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using MyerSplashShared.API;
using System.Linq;

namespace MyerSplash.ViewModel
{
    public class MainViewModel : ViewModelBase, INavigable
    {
        private const string FEATURED_CATEGORY_NAME = "FEATURED";
        private const string NEW_CATEGORY_NAME = "NEW";

        private ImageDataViewModel _mainDataVM;
        public ImageDataViewModel MainDataVM
        {
            get
            {
                return _mainDataVM;
            }
            set
            {
                if (_mainDataVM != value)
                {
                    _mainDataVM = value;
                    RaisePropertyChanged(() => MainDataVM);
                }
            }
        }

        private ObservableCollection<UnsplashImageBase> _mainList;
        public ObservableCollection<UnsplashImageBase> MainList
        {
            get
            {
                return _mainList;
            }
            set
            {
                if (_mainList != value)
                {
                    _mainList = value;
                    RaisePropertyChanged(() => MainList);
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
                      await MainDataVM.RetryAsync();
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
                      ShowSecondLayer = true;
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

        private bool _showSecondLayer;
        public bool ShowSecondLayer
        {
            get
            {
                return _showSecondLayer;
            }
            set
            {
                if (_showSecondLayer != value)
                {
                    _showSecondLayer = value;
                    RaisePropertyChanged(() => ShowSecondLayer);
                }
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
                      ShowSecondLayer = true;
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
                    _selectedIndex = value;
                    RaisePropertyChanged(() => SelectedIndex);
                    RaisePropertyChanged(() => SelectedTitle);
                    DrawerOpened = false;
                    if (value == -1)
                    {
                        return;
                    }
                    if (value == 1)
                    {
                        MainDataVM = new ImageDataViewModel(this, UrlHelper.GetNewImages, false);
                    }
                    else if (value == 0)
                    {
                        MainDataVM = new ImageDataViewModel(this, UrlHelper.GetFeaturedImages, true);
                    }
                    else if (value > 1)
                    {
                        MainDataVM = new ImageDataViewModel(this, Categories[value].RequestUrl, false);
                    }
                    if (MainDataVM != null)
                    {
                        var task = RefreshListAsync();
                    }
                }
            }
        }

        public string SelectedTitle
        {
            get
            {
                if (SelectedIndex == -1)
                {
                    if (SearchKeyword == null)
                    {
                        return NEW_CATEGORY_NAME;
                    }
                    else return SearchKeyword.ToUpper();
                }
                if (Categories?.Count > 0)
                {
                    return Categories[SelectedIndex].Title.ToUpper();
                }
                else return NEW_CATEGORY_NAME;
            }
        }

        public MainViewModel()
        {
            MainList = new ObservableCollection<UnsplashImageBase>();

            FooterLoadingVisibility = Visibility.Collapsed;
            NoItemHintVisibility = Visibility.Collapsed;
            NoNetworkHintVisibility = Visibility.Collapsed;
            FooterReloadVisibility = Visibility.Collapsed;
            EndVisibility = Visibility.Collapsed;
            IsRefreshing = true;

            App.MainVM = this;

            SelectedIndex = -1;
        }

        private async Task SearchByKeywordAsync()
        {
            MainDataVM = new SearchResultViewModel(this, UrlHelper.SearchImages, SearchKeyword);
            RaisePropertyChanged(() => SelectedTitle);
            await RefreshListAsync();
        }

        private async Task RestoreMainListDataAsync()
        {
            var file = await CacheUtil.GetCachedFileFolder().TryGetFileAsync(CachedFileNames.MainListFileName);
            if (file != null)
            {
                var list = await SerializerHelper.DeserializeFromJsonByFile<IncrementalLoadingCollection<UnsplashImageBase>>(CachedFileNames.MainListFileName, CacheUtil.GetCachedFileFolder());
                if (list != null)
                {
                    this.MainDataVM = new ImageDataViewModel(this, UrlHelper.GetNewImages, true);
                    list.ToList().ForEach(s => MainDataVM.DataList.Add(s));

                    for (int i = 0; i < MainDataVM.DataList.Count; i++)
                    {
                        var item = MainDataVM.DataList[i];
                        if (i % 2 == 0) item.BackColor = App.Current.Resources["ImageBackBrush1"] as SolidColorBrush;
                        else item.BackColor = App.Current.Resources["ImageBackBrush2"] as SolidColorBrush;
                        var task = item.RestoreDataAsync();
                    }
                }
                else MainDataVM = new ImageDataViewModel(this, UrlHelper.GetNewImages, false);
            }
            else MainDataVM = new ImageDataViewModel(this, UrlHelper.GetNewImages, false);
        }

        private async Task RefreshAllAsync()
        {
            var task1 = GetCategoriesAsync();
            await RefreshListAsync();
            await SaveMainListDataAsync();
        }

        private async Task RefreshListAsync()
        {
            MainDataVM.MainVM = this;
            IsRefreshing = true;
            await MainDataVM.RefreshAsync();
            IsRefreshing = false;
            MainList = MainDataVM.DataList;
        }

        private async Task GetCategoriesAsync()
        {
            if (Categories?.Count > 0) return;

            var result = await CloudService.GetCategories(CTSFactory.MakeCTS(20000).Token);
            if (result.IsRequestSuccessful)
            {
                var list = UnsplashCategory.GenerateListFromJson(result.JsonSrc);
                this.Categories = list;
                this.Categories.Insert(0, new UnsplashCategory()
                {
                    Title = "New",
                });
                this.Categories.Insert(0, new UnsplashCategory()
                {
                    Title = "Featured",
                });
                SelectedIndex = 1;
                await SerializerHelper.SerializerToJson<ObservableCollection<UnsplashCategory>>(list, CachedFileNames.CateListFileName, CacheUtil.GetCachedFileFolder());
            }
        }

        private async Task RestoreCategoriyListAsync()
        {
            this.Categories = await SerializerHelper.DeserializeFromJsonByFile<ObservableCollection<UnsplashCategory>>(CachedFileNames.CateListFileName);
            SelectedIndex = 1;
        }

        private async Task SaveMainListDataAsync()
        {
            if (this.MainDataVM.DataList?.Count > 0)
            {
                await SerializerHelper.SerializerToJson<IncrementalLoadingCollection<UnsplashImageBase>>(this.MainDataVM.DataList, CachedFileNames.MainListFileName, CacheUtil.GetCachedFileFolder());
            }
        }

        public void Activate(object param)
        {

        }

        public void Deactivate(object param)
        {

        }

        public async void OnLoaded()
        {
            if (IsFirstActived)
            {
                IsFirstActived = false;
                //await RestoreCategoriyListAsync();
                await RestoreMainListDataAsync();
                await RefreshAllAsync();
            }
        }
    }
}
