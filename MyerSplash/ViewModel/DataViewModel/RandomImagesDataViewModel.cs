using MyerSplashShared.Service;

namespace MyerSplash.ViewModel.DataViewModel
{
    public class RandomImagesDataViewModel : ImageDataViewModel
    {
        public RandomImagesDataViewModel(MainViewModel viewModel, RandomImageService service)
            : base(viewModel, service)
        {
        }
    }
}
