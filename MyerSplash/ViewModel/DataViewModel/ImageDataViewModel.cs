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

namespace MyerSplash.ViewModel
{
    public class ImageDataViewModel : DataViewModelBase<UnsplashImage>
    {
        private string _requestUrl;
        protected UnsplashImageFactory _imageFactory;

        public ImageDataViewModel(string url, bool featured)
        {
            _requestUrl = url;
            _imageFactory = new UnsplashImageFactory(featured);
        }

        protected override void ClickItem(UnsplashImage item)
        {

        }

        protected void UpdateHintVisibility(IEnumerable<UnsplashImage> list)
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

        protected async override Task<IEnumerable<UnsplashImage>> GetList(int pageIndex)
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

        protected async override void LoadMoreItemCompleted(IEnumerable<UnsplashImage> list, int pagingIndex)
        {
            var tasks = new List<Task>();
            for (var i = 0; i < list.Count(); i++)
            {
                var item = list.ElementAt(i);
                item.BackColorBrush = new SolidColorBrush(item.ColorValue.ToColor());
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

            if (pagingIndex == 1)
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

        protected async virtual Task<IEnumerable<UnsplashImage>> RequestAsync(int pageIndex)
        {
            var cts = CTSFactory.MakeCTS(15000);
            try
            {
                var result = await CloudService.GetImages(pageIndex, (int)20u, cts.Token, _requestUrl);
                if (result.IsRequestSuccessful)
                {
                    IEnumerable<UnsplashImage> list = _imageFactory.GetImages(result.JsonSrc);
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
