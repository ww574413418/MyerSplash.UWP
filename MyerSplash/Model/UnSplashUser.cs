using GalaSoft.MvvmLight;
using JP.Utils.Data.Json;
using MyerSplashShared.Shared;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Data.Json;
using System;

namespace MyerSplash.Model
{
    public class UnsplashUser : ViewModelBase, IParseFromJson
    {
        private CachedBitmapSource _avatarBitmap;
        [IgnoreDataMember]
        public CachedBitmapSource AvatarBitmap
        {
            get
            {
                return _avatarBitmap;
            }
            set
            {
                if (_avatarBitmap != value)
                {
                    _avatarBitmap = value;
                    RaisePropertyChanged(() => AvatarBitmap);
                }
            }
        }

        private string _name;
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

        public string Id { get; set; }

        public string AvatarUrl { get; set; }

        public string HomePageUrl { get; set; }

        private string _bio;
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

        public async Task DownloadAvatarAsync()
        {
            AvatarBitmap = new CachedBitmapSource();
            AvatarBitmap.RemoteUrl = AvatarUrl;
            AvatarBitmap.ExpectedFileName = Id;
            await AvatarBitmap.LoadBitmapAsync();
        }

        #region Static method

        public void ParseObjectFromJsonObject(JsonObject obj)
        {
            var id = JsonParser.GetStringFromJsonObj(obj, "id");
            var name = JsonParser.GetStringFromJsonObj(obj, "name");
            var bio = JsonParser.GetStringFromJsonObj(obj, "bio");
            var profile_image = JsonParser.GetJsonObjFromJsonObj(obj, "profile_image");
            var image = JsonParser.GetStringFromJsonObj(profile_image, "medium");
            var links = JsonParser.GetJsonObjFromJsonObj(obj, "links");
            var homeUrl = JsonParser.GetStringFromJsonObj(links, "html");

            this.Name = name;
            this.Id = id;
            this.Bio = bio;
            this.AvatarUrl = image;
            this.HomePageUrl = homeUrl;
        }

        public void ParseObjectFromJsonString(string json)
        {
            var obj = JsonObject.Parse(json);
            ParseObjectFromJsonObject(obj);
        }
        #endregion
    }
}
