using GalaSoft.MvvmLight;

namespace MyerSplash.Model
{
    public class ImageExif : ViewModelBase
    {
        private const string DEFAULT = "Unknown";

        private string _model;
        public string Model
        {
            get
            {
                return _model;
            }
            set
            {
                if (_model != value)
                {
                    _model = value;
                    RaisePropertyChanged(() => Model);
                }
            }
        }

        private string _exposureTime;
        public string ExposureTime
        {
            get
            {
                return $"{_exposureTime}s";
            }
            set
            {
                if (_exposureTime != value)
                {
                    _exposureTime = value;
                    RaisePropertyChanged(() => ExposureTime);
                }
            }
        }

        private string _aperture;
        public string Aperture
        {
            get
            {
                return $"f/{_aperture}";
            }
            set
            {
                if (_aperture != value)
                {
                    _aperture = value;
                    RaisePropertyChanged(() => Aperture);
                }
            }
        }

        private int? _iso;
        public int? Iso
        {
            get
            {
                return _iso;
            }
            set
            {
                if (_iso != value)
                {
                    _iso = value;
                    RaisePropertyChanged(() => Iso);
                    RaisePropertyChanged(() => IsoString);
                }
            }
        }

        public string IsoString
        {
            get
            {
                if (Iso == null) return DEFAULT;
                return Iso.ToString();
            }
        }

        public ImageExif()
        {
            Model = DEFAULT;
            ExposureTime = DEFAULT;
            Aperture = DEFAULT;
        }
    }
}
