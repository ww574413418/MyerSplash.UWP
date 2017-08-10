using MyerSplash.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyerSplashShared.Service
{
    public class RandomImageService : ImageServiceBase
    {
        public int Count { get; set; } = 20;

        public RandomImageService(UnsplashImageFactory factory) : base(factory)
        {
        }

        public RandomImageService(int count, UnsplashImageFactory factory) : base(factory)
        {
            Count = count;
        }

        public override async Task<IEnumerable<UnsplashImage>> GetImagesAsync(CancellationToken token)
        {
            var result = await _cloudService.GetRandomImagesAsync(Count, token);
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
