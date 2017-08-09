using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace MyerSplash.Model
{
    public class Links : ViewModelBase
    {
        [JsonProperty("html")]
        private string _homePageUrl;
        public string HomePageUrl
        {
            get
            {
                return _homePageUrl;
            }
            set
            {
                if (_homePageUrl != value)
                {
                    _homePageUrl = value;
                    RaisePropertyChanged(() => HomePageUrl);
                }
            }
        }

        [JsonProperty("photos")]
        public string Photos { get; set; }
    }
}
