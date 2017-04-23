using GalaSoft.MvvmLight;

namespace MyerSplash.Model
{
    public class ImageLocation : ViewModelBase
    {
        private string _city;
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
