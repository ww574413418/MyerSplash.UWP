using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyerSplash.Data;
using MyerSplashShared.API;
using MyerSplashShared.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject.Service
{
    [TestClass]
    public class ImageServiceTest
    {
        private ImageServiceBase _newImageService = new ImageService(Request.GetNewImages,
            new UnsplashImageFactory(false));

        private ImageServiceBase _featuredImageService = new ImageService(Request.GetFeaturedImages,
            new UnsplashImageFactory(true));

        private ImageServiceBase _randomImageService = new RandomImageService(new UnsplashImageFactory(false));

        private SearchImageService _searchImageService = new SearchImageService(new UnsplashImageFactory(false));

        [TestMethod]
        public async Task TestGetNewImages()
        {
            var result = await _newImageService.GetImagesAsync(CTSFactory.MakeCTS().Token);
            Assert.IsTrue(result?.Count() > 0);
        }

        [TestMethod]
        public async Task TestGetFeatureImages()
        {
            var result = await _featuredImageService.GetImagesAsync(CTSFactory.MakeCTS().Token);
            Assert.IsTrue(result?.Count() > 0);
        }

        [TestMethod]
        public async Task TestGetRandomImages()
        {
            var result = await _randomImageService.GetImagesAsync(CTSFactory.MakeCTS().Token);
            Assert.IsTrue(result?.Count() > 0);
        }

        [TestMethod]
        public async Task TestSearchHasResultImages()
        {
            _searchImageService.Query = "sea";
            var result = await _searchImageService.GetImagesAsync(CTSFactory.MakeCTS().Token);
            Assert.IsTrue(result?.Count() > 0);
        }

        [TestMethod]
        public async Task TestSearchHasNoResultImages()
        {
            _searchImageService.Query = "dwerewrwefsdfwe";
            var result = await _searchImageService.GetImagesAsync(CTSFactory.MakeCTS().Token);
            Assert.IsTrue(result?.Count() == 0);
        }
    }
}
