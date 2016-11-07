using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using JP.API;
using JP.Utils.Debug;
using JP.Utils.Helper;
using MyerSplash.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace MyerSplash.ViewModel
{
    public abstract class DataViewModelBase<T> : ViewModelBase
    {
        public static int DEFAULT_PAGE_INDEX => 1;

        //public static uint DEFAULT_PER_PAGE
        //{
        //    get
        //    {
        //        if (DeviceHelper.IsMobile) return 10u;
        //        else if (Window.Current.CoreWindow.Bounds.Width >= 1500)
        //        {
        //            return 40u;
        //        }
        //        else return 20u;
        //    }
        //}

        private int PageIndex { get; set; } = DEFAULT_PAGE_INDEX;

        /// <summary>
        /// 列表增量加载完成后发生，此回调在UI线程进行
        /// </summary>
        public event Action<IEnumerable<T>, int> OnLoadIncrementalDataCompleted;
        public event Action<bool> OnHasMoreItemChanged;

        /// <summary>
        /// 实现了增量加载的 List
        /// 应该这样初始化：
        /// DataList = new IncrementalLoadingCollection<T>(count =>
        ///{
        ///    return Task.Run(() => GetIncrementalListData(pageIndex++));
        ///});
        /// 其中加载数据的逻辑都在：GetIncrementalListData(int pageIndex) 
        /// </summary>
        private IncrementalLoadingCollection<T> _dataList;
        public IncrementalLoadingCollection<T> DataList
        {
            get
            {
                return _dataList;
            }
            set
            {
                _dataList = value;
                RaisePropertyChanged(() => DataList);
            }
        }

        private RelayCommand<T> _viewDetailCommand;
        public RelayCommand<T> ViewDetailCommand
        {
            get
            {
                if (_viewDetailCommand != null) return _viewDetailCommand;
                return _viewDetailCommand = new RelayCommand<T>((data) =>
                {
                    try
                    {
                        if (data == null) return;
                        ClickItem(data);
                    }
                    catch (Exception e)
                    {
                        var task = Logger.LogAsync(e);
                    }
                });
            }
        }

        public DataViewModelBase()
        {
            DataList = new IncrementalLoadingCollection<T>(count =>
            {
                return Task.Run(() => GetIncrementalListData(PageIndex++));
            });
        }

        public async Task<bool> RefreshAsync()
        {
            try
            {
                if (DataList.IsBusy)
                {
                    return false;
                }

                PageIndex = DEFAULT_PAGE_INDEX;

                if (DataList == null)
                {
                    DataList = new IncrementalLoadingCollection<T>(count =>
                    {
                        return GetIncrementalListData(PageIndex++);
                    });
                }

                await DataList.LoadMoreItemsAsync(20u);

                return true;
            }
            catch (Exception e)
            {
                var task = Logger.LogAsync(e);
                return false;
            }
        }

        public async Task RetryAsync()
        {
            await DataList.LoadMoreItemsAsync(20u);
        }

        private async Task<ResultData<T>> GetIncrementalListData(int pageIndex)
        {
            IEnumerable<T> newList = new List<T>();
            bool HasMoreItems = false;
            try
            {
                newList = await GetList(pageIndex);

                if (newList == null || newList.Count() == 0)
                {
                    HasMoreItems = false;
                }
                else if (newList.Count() > 0)
                {
                    HasMoreItems = true;
                }

                await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    OnHasMoreItemChanged?.Invoke(HasMoreItems);
                    LoadMoreItemCompleted(newList, pageIndex);
                });
            }
            catch (Exception e)
            {
                var task = Logger.LogAsync(e);
            }
            return new ResultData<T>() { Data = newList, HasMoreItems = HasMoreItems };
        }

        protected abstract Task<IEnumerable<T>> GetList(int pageIndex);

        protected abstract void ClickItem(T item);

        protected abstract void LoadMoreItemCompleted(IEnumerable<T> list, int index);
    }
}
