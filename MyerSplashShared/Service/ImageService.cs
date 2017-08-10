using JP.API;
using MyerSplash.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyerSplashShared.Service
{
    public class ImageService : ImageServiceBase
    {
        protected string RequestUrl { get; set; }

        public int Count { get; set; } = 20;

        public ImageService(string url, UnsplashImageFactory factory) : base(factory)
        {
            RequestUrl = url;
        }

        public Task<CommonRespMsg> GetImageDetailAsync(string id, CancellationToken token)
        {
            return _cloudService.GetImageDetailAsync(id, token);
        }

        public override async Task<IEnumerable<UnsplashImage>> GetImagesAsync(CancellationToken token)
        {
            var result = await _cloudService.GetImagesAsync(Page, Count, token, RequestUrl);
            if (result.IsRequestSuccessful)
            {
                var imageList = _factory.GetImages(result.JsonSrc);
                return imageList;
            }
            else
            {
                return null;
            }
        }
    }
}
