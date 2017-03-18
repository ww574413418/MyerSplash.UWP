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
using System.Diagnostics;
using MyerSplash.LiveTile;
using Windows.UI;

namespace MyerSplash.ViewModel
{
    public class ImageDataViewModel : DataViewModelBase<UnsplashImageBase>
    {
        public string RequestUrl { get; set; }

        public bool Featured { get; set; } = false;

        public ImageDataViewModel(string url, bool featured)
        {
            this.RequestUrl = url;
            this.Featured = featured;
        }

        protected override void ClickItem(UnsplashImageBase item)
        {

        }

        protected void UpdateHintVisibility(IEnumerable<UnsplashImageBase> list)
        {
            var task = RunOnUiThread(() =>
              {
                  // No items at all
                  if (DataList.Count == 0)
                  {
                      if (list.Count() == 0)
                      {
                          App.MainVM.NoItemHintVisibility = Visibility.Visible;
                      }
                  }
                  else App.MainVM.NoItemHintVisibility = Visibility.Collapsed;

                  // Has loaded items but no more
                  if (list.Count() == 0)
                  {
                      App.MainVM.FooterLoadingVisibility = Visibility.Collapsed;
                      App.MainVM.EndVisibility = Visibility.Visible;
                  }
                  //There are more items
                  else
                  {
                      App.MainVM.FooterLoadingVisibility = Visibility.Visible;
                      App.MainVM.EndVisibility = Visibility.Collapsed;
                  }

                  return;
              });
        }

        protected async override Task<IEnumerable<UnsplashImageBase>> GetList(int pageIndex)
        {
            try
            {
                if (pageIndex >= 2)
                {
                    App.MainVM.FooterLoadingVisibility = Visibility.Visible;
                }

                return await RequestAsync(pageIndex);
            }
            catch (APIException)
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    App.MainVM.FooterLoadingVisibility = Visibility.Collapsed;
                    App.MainVM.IsRefreshing = false;

                    if (App.MainVM.DataVM.DataList?.Count == 0)
                    {
                        App.MainVM.NoItemHintVisibility = Visibility.Visible;
                    }
                    else
                    {
                        App.MainVM.NoItemHintVisibility = Visibility.Collapsed;
                        App.MainVM.FooterReloadVisibility = Visibility.Visible;
                    }

                    ToastService.SendToast("Request failed.");
                });
                return new List<UnsplashImage>();
            }
            catch (TaskCanceledException)
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    App.MainVM.FooterLoadingVisibility = Visibility.Collapsed;
                    App.MainVM.IsRefreshing = false;

                    if (App.MainVM.DataVM.DataList?.Count == 0)
                    {
                        App.MainVM.NoItemHintVisibility = Visibility.Visible;
                    }
                    else
                    {
                        App.MainVM.NoItemHintVisibility = Visibility.Collapsed;
                        App.MainVM.FooterReloadVisibility = Visibility.Visible;
                    }

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
                item.BackColor = new SolidColorBrush(item.ColorValue.ToColor());
                item.MajorColor = new SolidColorBrush(item.ColorValue.ToColor());

                tasks.Add(item.DownloadImgForListAsync());
            }

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception)
            {

            }

            if (index == 1)
            {
                await UpdateLiveTileAsync();
            }
        }

        private async Task UpdateLiveTileAsync()
        {
            var list = new List<string>();

            if (DataList == null) return;

            foreach (var item in DataList)
            {
                list.Add(item.ListImageBitmap.LocalPath);
            }
            if (App.AppSettings.EnableTile && list.Count > 0)
            {
                Debug.WriteLine("About to update tile.");
                await LiveTileUpdater.UpdateImagesTileAsync(list);
            }
        }

        protected async virtual Task<IEnumerable<UnsplashImageBase>> RequestAsync(int pageIndex)
        {
#if DEBUG
            var cts = CTSFactory.MakeCTS();
#else
            var cts = CTSFactory.MakeCTS(15000);
#endif
            try
            {
                var result = await CloudService.GetImages(pageIndex, (int)20u, cts.Token, RequestUrl);
                if (result.IsRequestSuccessful)
                {
                    IEnumerable<UnsplashImageBase> list = null;
                    if (Featured)
                    {
                        list = UnsplashFeaturedImage.ParseListFromJson(result.JsonSrc);
                    }
                    else
                    {
                        list = UnsplashImage.ParseListFromJson(result.JsonSrc);
                    }
                    await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        UpdateHintVisibility(list);
                    });
                    return list;
                }
                else throw new ArgumentNullException();
            }
            catch (Exception e)
            {
                await Logger.LogAsync(e);
                return new List<UnsplashImage>();
            }
        }
    }
}
