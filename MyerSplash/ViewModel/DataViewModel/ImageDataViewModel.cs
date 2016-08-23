using JP.Utils.Debug;
using JP.Utils.UI;
using MyerSplash.Model;
using MyerSplashCustomControl;
using MyerSplashShared.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
using System.Runtime.Serialization;

namespace MyerSplash.ViewModel
{
    public class ImageDataViewModel : DataViewModelBase<UnsplashImageBase>
    {
        [IgnoreDataMember]
        public MainViewModel MainVM { get; set; }

        public string RequestUrl { get; set; }

        public bool Featured { get; set; } = false;

        public ImageDataViewModel(MainViewModel mainVM, string url, bool featured)
        {
            this.MainVM = mainVM;
            this.RequestUrl = url;
            this.Featured = featured;
        }

        public ImageDataViewModel(string url, bool featured)
        {
            this.RequestUrl = url;
            this.Featured = featured;
        }

        protected override void ClickItem(UnsplashImageBase item)
        {

        }

        protected async override Task<IEnumerable<UnsplashImageBase>> GetList(int pageIndex)
        {
            try
            {
                if (pageIndex >= 2)
                {
                    MainVM.ShowFooterLoading = Visibility.Visible;
                }

                var result = await CloudService.GetImages(pageIndex, (int)DEFAULT_PER_PAGE, CTSFactory.MakeCTS(10000).Token, RequestUrl);
                if (result.IsRequestSuccessful)
                {
                    if (Featured)
                    {
                        var list = UnsplashFeaturedImage.ParseListFromJson(result.JsonSrc);
                        return list;
                    }
                    else
                    {
                        var list = UnsplashImage.ParseListFromJson(result.JsonSrc);
                        return list;
                    }
                }
                else
                {
                    throw new APIException();
                }
            }
            catch (APIException)
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    MainVM.ShowFooterLoading = Visibility.Collapsed;
                    MainVM.ShowFooterReloadGrid = Visibility.Visible;
                    MainVM.IsRefreshing = false;

                    if (MainVM.MainList?.Count == 0)
                        MainVM.ShowNoItemHint = Visibility.Visible;
                    else MainVM.ShowNoItemHint = Visibility.Collapsed;

                    ToastService.SendToast("Request failed.");
                });
                return new List<UnsplashImage>();
            }
            catch (TaskCanceledException)
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    MainVM.ShowFooterLoading = Visibility.Collapsed;
                    MainVM.ShowFooterReloadGrid = Visibility.Visible;
                    MainVM.IsRefreshing = false;

                    if (MainVM.MainList.Count == 0)
                        MainVM.ShowNoItemHint = Visibility.Visible;
                    else MainVM.ShowNoItemHint = Visibility.Collapsed;

                    ToastService.SendToast("Request timeout.");
                });
                return new List<UnsplashImage>();
            }
            catch (Exception e)
            {
                var task = Logger.LogAsync(e);
                return new List<UnsplashImage>();
            }
        }

        protected async override void LoadMoreItemCompleted(IEnumerable<UnsplashImageBase> list, int index)
        {
            var tasks = new List<Task>();
            for (var i = 0; i < list.Count(); i++)
            {
                var item = list.ElementAt(i);

                if (i % 2 == 0) item.BackColor = new SolidColorBrush(ColorConverter.HexToColor("#FF2E2E2E").Value);
                else item.BackColor = new SolidColorBrush(ColorConverter.HexToColor("#FF383838").Value);
                item.MajorColor = new SolidColorBrush(ColorConverter.HexToColor(item.ColorValue).Value);

                tasks.Add(item.DownloadImgForListAsync());
            }
            await Task.WhenAll(tasks);
        }
    }
}
