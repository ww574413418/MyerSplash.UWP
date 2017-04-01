using System.Collections.Generic;
using System.Threading.Tasks;
using MyerSplash.Model;
using MyerSplashShared.API;
using System;

namespace MyerSplash.ViewModel.DataViewModel
{
    public class RandomImagesDataViewModel : ImageDataViewModel
    {
        public RandomImagesDataViewModel(string url, bool featured, Func<UnsplashImageBase, bool> filter)
            : base(url, featured, filter)
        {

        }

        protected async override Task<IEnumerable<UnsplashImageBase>> RequestAsync(int pageIndex)
        {
            var result = await CloudService.GetRandomImages((int)20, CTSFactory.MakeCTS(5000).Token);
            if (result.IsRequestSuccessful)
            {
                var list = UnsplashImage.ParseListFromJson(result.JsonSrc);
                UpdateHintVisibility(list);
                return list;
            }
            else
            {
                throw new APIException();
            }
        }
    }
}
