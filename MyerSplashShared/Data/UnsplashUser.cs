using Newtonsoft.Json;

namespace MyerSplash.Data
{
    public class UnsplashUser : ModelBase
    {
        private string _name;
        [JsonProperty("name")]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged(() => Name);
                }
            }
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("links")]
        private Links _links;
        public Links Links
        {
            get
            {
                return _links;
            }
            set
            {
                if (_links != value)
                {
                    _links = value;
                    RaisePropertyChanged(() => Links);
                }
            }
        }

        private string _bio;
        [JsonProperty("bio")]
        public string Bio
        {
            get
            {
                return _bio;
            }
            set
            {
                if (_bio != value)
                {
                    _bio = value;
                    RaisePropertyChanged(() => Bio);
                }
            }
        }

        public UnsplashUser()
        {

        }
    }
}
