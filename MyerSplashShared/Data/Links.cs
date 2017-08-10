using Newtonsoft.Json;

namespace MyerSplash.Data
{
    public class Links : ModelBase
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
