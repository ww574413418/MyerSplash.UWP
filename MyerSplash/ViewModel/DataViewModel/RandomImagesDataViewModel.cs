using System.Collections.Generic;
using System.Threading.Tasks;
using MyerSplash.Model;
using MyerSplashShared.API;

namespace MyerSplash.ViewModel.DataViewModel
{
    public class RandomImagesDataViewModel : ImageDataViewModel
    {
        public RandomImagesDataViewModel(string url, bool featured)
            : base(url, featured)
        {

        }

        protected async override Task<IEnumerable<UnsplashImageBase>> RequestAsync(int pageIndex)
        {
            var result = await CloudService.GetRandomImages(20, CTSFactory.MakeCTS(5000).Token);
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
