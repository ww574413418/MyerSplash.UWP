using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace MyerSplash.Model
{
    public class UnsplashCategory : ViewModelBase
    {
        private int _id;
        [JsonProperty("id")]
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    RaisePropertyChanged(() => Id);
                }
            }
        }

        private string _title;
        [JsonProperty("title")]
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    RaisePropertyChanged(() => Title);
                }
            }
        }

        private int _photoCount;
        [JsonProperty("photo_count")]
        public int PhotoCount
        {
            get
            {
                return _photoCount;
            }
            set
            {
                if (_photoCount != value)
                {
                    _photoCount = value;
                    RaisePropertyChanged(() => PhotoCount);
                }
            }
        }

        [JsonProperty("links")]
        public Links Links { get; set; }

        public UnsplashCategory()
        {

        }
    }
}
