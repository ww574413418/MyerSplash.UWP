using Newtonsoft.Json;

namespace MyerSplash.Data
{
    public class ImageLocation : ModelBase
    {
        private string _city;
        [JsonProperty("city")]
        public string City
        {
            get
            {
                return _city;
            }
            set
            {
                if (_city != value)
                {
                    _city = value;
                    RaisePropertyChanged(() => City);
                }
            }
        }

        private string _country;
        [JsonProperty("country")]
        public string Country
        {
            get
            {
                return _country;
            }
            set
            {
                if (_country != value)
                {
                    _country = value;
                    RaisePropertyChanged(() => Country);
                }
            }
        }

        public ImageLocation()
        {

        }
    }
}
