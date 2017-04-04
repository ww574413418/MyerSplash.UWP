using System.Collections.Generic;
using System.Threading.Tasks;
using MyerSplash.Model;
using MyerSplashShared.API;
using Windows.Data.Json;
using JP.Utils.Data.Json;
using System;

namespace MyerSplash.ViewModel
{
    public class SearchResultViewModel : ImageDataViewModel
    {
        private string _keyword;

        public SearchResultViewModel(string url, string keyword, Func<UnsplashImageBase, bool> filter) :
            base(url, false, filter)
        {
            _keyword = keyword;
        }

        protected async override Task<IEnumerable<UnsplashImageBase>> RequestAsync(int pageIndex)
        {
            var result = await CloudService.SearchImages(pageIndex, (int)20, CTSFactory.MakeCTS(10000).Token, _keyword);
            if (result.IsRequestSuccessful)
            {
                var rootObj = JsonObject.Parse(result.JsonSrc);
                var resultArray = JsonParser.GetJsonArrayFromJsonObj(rootObj, "results");
                var list = UnsplashImage.ParseListFromJson(resultArray.ToString());
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
