using MyerSplashShared.API;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JP.Utils.Data.Json;
using Windows.Data.Json;

namespace MyerSplash.Data
{
    public class UnsplashImage : ModelBase
    {
        public string ID { get; set; }

        [JsonProperty("urls")]
        public UnsplashUrl Urls { get; set; }

        private string _title;
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

        [JsonProperty("color")]
        public string ColorValue { get; set; }

        private double _width;
        [JsonProperty("width")]
        public double Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (_width != value)
                {
                    _width = value;
                    RaisePropertyChanged(() => Width);
                }
            }
        }

        private double _height;
        [JsonProperty("height")]
        public double Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (_height != value)
                {
                    _height = value;
                    RaisePropertyChanged(() => Height);
                }
            }
        }

        private ImageExif _exif;
        [JsonProperty("exif")]
        public ImageExif Exif
        {
            get
            {
                return _exif;
            }
            set
            {
                if (_exif != value)
                {
                    _exif = value;
                    RaisePropertyChanged(() => Exif);
                }
            }
        }

        private ImageLocation _location;
        [JsonProperty("location")]
        public ImageLocation Location
        {
            get
            {
                return _location;
            }
            set
            {
                if (_location != value)
                {
                    _location = value;
                    RaisePropertyChanged(() => Location);
                }
            }
        }

        private UnsplashUser _owner;
        [JsonProperty("user")]
        public UnsplashUser Owner
        {
            get
            {
                return _owner;
            }
            set
            {
                if (_owner != value)
                {
                    _owner = value;
                    RaisePropertyChanged(() => Owner);
                }
            }
        }

        private bool _liked;
        [JsonProperty("liked_by_user")]
        public bool Liked
        {
            get
            {
                return _liked;
            }
            set
            {
                if (_liked != value)
                {
                    _liked = value;
                    RaisePropertyChanged(() => Liked);
                }
            }
        }

        private int _likes;
        [JsonProperty("likes")]
        public int Likes
        {
            get
            {
                return _likes;
            }
            set
            {
                if (_likes != value)
                {
                    _likes = value;
                    RaisePropertyChanged(() => Likes);
                }
            }
        }

        private DateTime _createTime;
        public DateTime CreateTime
        {
            get
            {
                return _createTime;
            }
            set
            {
                if (_createTime != value)
                {
                    _createTime = value;
                    RaisePropertyChanged(() => CreateTime);
                }
            }
        }

        public string CreateTimeString
        {
            get
            {
                return _createTime.ToString("yyyy-MM-dd hh-mm-ss");
            }
        }

        private bool _isUnsplash;
        public bool IsUnsplash
        {
            get
            {
                return _isUnsplash;
            }
            set
            {
                if (_isUnsplash != value)
                {
                    _isUnsplash = value;
                    RaisePropertyChanged(() => IsUnsplash);
                }
            }
        }

        public UnsplashImage()
        {
            IsUnsplash = true;
        }
    }
}
