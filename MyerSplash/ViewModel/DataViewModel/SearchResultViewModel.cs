using MyerSplashShared.Service;

namespace MyerSplash.ViewModel.DataViewModel
{
    public class SearchResultViewModel : ImageDataViewModel
    {
        public SearchResultViewModel(MainViewModel viewModel, SearchImageService service) :
            base(viewModel, service)
        {
        }
    }
}