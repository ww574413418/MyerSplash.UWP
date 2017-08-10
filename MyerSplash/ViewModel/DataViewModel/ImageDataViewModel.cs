using JP.Utils.Debug;
using MyerSplashCustomControl;
using MyerSplashShared.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using System.Diagnostics;
using MyerSplash.LiveTile;
using MyerSplash.Data;
using MyerSplash.Model;
using MyerSplashShared.Service;

namespace MyerSplash.ViewModel
{
    public class ImageDataViewModel : DataViewModelBase<ImageItem>
    {
        protected MainViewModel _mainViewModel;
        protected ImageServiceBase _imageService;

        public ImageDataViewModel(MainViewModel viewModel, ImageServiceBase service)
        {
            _mainViewModel = viewModel;
            _imageService = service;
        }

        protected override void ClickItem(ImageItem item)
        {

        }

        protected IEnumerable<ImageItem> CreateImageItems(IEnumerable<UnsplashImage> images)
        {
            var list = new List<ImageItem>();
            foreach (var i in images)
            {
                list.Add(new ImageItem(i));
            }
            return list;
        }

        protected void UpdateHintVisibility(IEnumerable<ImageItem> list)
        {
            var task = RunOnUiThread(() =>
              {
                  // No items at all
                  if (DataList.Count == 0)
                  {
                      if (list.Count() == 0)
                      {
                          _mainViewModel.NoItemHintVisibility = Visibility.Visible;
                      }
                  }
                  else _mainViewModel.NoItemHintVisibility = Visibility.Collapsed;

                  // Has loaded items but no more
                  if (list.Count() == 0)
                  {
                      _mainViewModel.FooterLoadingVisibility = Visibility.Collapsed;
                      _mainViewModel.EndVisibility = Visibility.Visible;
                  }
                  //There are more items
                  else
                  {
                      _mainViewModel.FooterLoadingVisibility = Visibility.Visible;
                      _mainViewModel.EndVisibility = Visibility.Collapsed;
                  }

                  return;
              });
        }

        protected async override Task<IEnumerable<ImageItem>> GetList(int pageIndex)
        {
            try
            {
                if (pageIndex >= 2)
                {
                    _mainViewModel.FooterLoadingVisibility = Visibility.Visible;
                }

                return await RequestAsync(pageIndex);
            }
            catch (APIException)
            {
                await RunOnUiThread(() =>
                {
                    _mainViewModel.FooterLoadingVisibility = Visibility.Collapsed;
                    _mainViewModel.IsRefreshing = false;

                    if (_mainViewModel.DataVM.DataList?.Count == 0)
                    {
                        _mainViewModel.NoItemHintVisibility = Visibility.Visible;
                    }
                    else
                    {
                        _mainViewModel.NoItemHintVisibility = Visibility.Collapsed;
                        _mainViewModel.FooterReloadVisibility = Visibility.Visible;
                    }

                    ToastService.SendToast("Request failed.");
                });
                return new List<ImageItem>();
            }
            catch (TaskCanceledException)
            {
                await RunOnUiThread(() =>
                {
                    _mainViewModel.FooterLoadingVisibility = Visibility.Collapsed;
                    _mainViewModel.IsRefreshing = false;

                    if (_mainViewModel.DataVM.DataList?.Count == 0)
                    {
                        _mainViewModel.NoItemHintVisibility = Visibility.Visible;
                    }
                    else
                    {
                        _mainViewModel.NoItemHintVisibility = Visibility.Collapsed;
                        _mainViewModel.FooterReloadVisibility = Visibility.Visible;
                    }

                    ToastService.SendToast("Request timeout.");
                });
                return new List<ImageItem>();
            }
            catch (Exception e)
            {
                var task = Logger.LogAsync(e);
                return new List<ImageItem>();
            }
        }

        protected async override void LoadMoreItemCompleted(IEnumerable<ImageItem> list, int pagingIndex)
        {
            var tasks = new List<Task>();
            foreach (var item in list)
            {
                item.Init();
                tasks.Add(item.DownloadBitmapForListAsync());
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

        protected async virtual Task<IEnumerable<ImageItem>> RequestAsync(int pageIndex)
        {
            var cts = CTSFactory.MakeCTS(15000);
            try
            {
                _imageService.Page = pageIndex;
                var result = await _imageService.GetImagesAsync(cts.Token);
                if (result != null)
                {
                    var list = CreateImageItems(result);
                    UpdateHintVisibility(list);
                    return list;
                }
                else throw new ArgumentNullException();
            }
            catch (Exception e)
            {
                await Logger.LogAsync(e);
                return new List<ImageItem>();
            }
        }
    }
}
