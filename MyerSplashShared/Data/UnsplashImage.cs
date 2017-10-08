using Newtonsoft.Json;
using System;

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

        [JsonProperty("created_at")]
        public string CreateTimeString { get; set; }

        public DateTime CreateTime
        {
            get
            {
                DateTime.TryParse(CreateTimeString, out DateTime time);
                return time;
            }
        }

        public string SimpleCreateTimeString
        {
            get
            {
                return CreateTime.ToString("yyyy-MM-dd hh-mm-ss");
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