using GalaSoft.MvvmLight;

namespace MyerSplash.Model
{
    public class ImageExif : ViewModelBase
    {
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

        private double _exposureTime;
        public double ExposureTime
        {
            get
            {
                return _exposureTime;
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

        private double _aperture;
        public double Aperture
        {
            get
            {
                return _aperture;
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

        private int _iso;
        public int Iso
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
                }
            }
        }

        public ImageExif()
        {

        }
    }
}
