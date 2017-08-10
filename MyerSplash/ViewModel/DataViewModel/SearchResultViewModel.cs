﻿using MyerSplashShared.Service;

namespace MyerSplash.ViewModel
{
    public class SearchResultViewModel : ImageDataViewModel
    {
        private string _query;

        public SearchResultViewModel(string keyword, MainViewModel viewModel, SearchImageService service) :
            base(viewModel, service)
        {
            _query = keyword;
        }
    }
}
