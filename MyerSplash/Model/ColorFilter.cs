using GalaSoft.MvvmLight;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace MyerSplash.Model
{
    public class ColorFilter : ViewModelBase
    {
        private Color _color;
        public Color Color
        {
            get
            {
                return _color;
            }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    RaisePropertyChanged(() => Color);
                }
            }
        }

        private SolidColorBrush _brush;
        public SolidColorBrush Brush
        {
            get
            {
                return _brush;
            }
            set
            {
                if (_brush != value)
                {
                    _brush = value;
                    RaisePropertyChanged(() => Brush);
                }
            }
        }

        private string _colorName;
        public string ColorName
        {
            get
            {
                return _colorName;
            }
            set
            {
                if (_colorName != value)
                {
                    _colorName = value;
                    RaisePropertyChanged(() => ColorName);
                }
            }
        }

        public ColorFilter(Color color, string name)
        {
            Color = color;
            Brush = new SolidColorBrush(Color);
            ColorName = name;
        }
    }
}
