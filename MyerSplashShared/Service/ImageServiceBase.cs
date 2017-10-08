using MyerSplash.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyerSplashShared.Service
{
    public abstract class ImageServiceBase : IService
    {
        protected CloudService _cloudService = new CloudService();
        protected UnsplashImageFactory _factory;

        public int Page { get; set; } = 1;

        public ImageServiceBase(UnsplashImageFactory factory)
        {
            _factory = factory;
        }

        public abstract Task<IEnumerable<UnsplashImage>> GetImagesAsync(CancellationToken token);
    }
}