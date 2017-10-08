using JP.Utils.Data.Json;
using MyerSplash.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace MyerSplashShared.Service
{
    public class SearchImageService : ImageServiceBase
    {
        public int Count { get; set; } = 20;

        public string Query { get; set; }

        public SearchImageService(UnsplashImageFactory factory, string query) : base(factory)
        {
            Query = query;
        }

        public override async Task<IEnumerable<UnsplashImage>> GetImagesAsync(CancellationToken token)
        {
            var result = await _cloudService.SearchImagesAsync(Page, Count, token, Query);
            if (result.IsRequestSuccessful)
            {
                var rootObj = JsonObject.Parse(result.JsonSrc);
                var resultArray = JsonParser.GetJsonArrayFromJsonObj(rootObj, "results");
                var imageList = _factory.GetImages(resultArray.ToString());
                return imageList;
            }
            else
            {
                return null;
            }
        }
    }
}