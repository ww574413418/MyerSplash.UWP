using GalaSoft.MvvmLight;
using JP.Utils.Data.Json;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Xaml.Media.Imaging;
using System;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using System.Runtime.Serialization;
using JP.Utils.UI;
using System.Threading;
using Windows.Networking.BackgroundTransfer;
using MyerSplash.Common;
using GalaSoft.MvvmLight.Command;
using MyerSplashCustomControl;
using JP.Utils.Data;
using MyerSplash.ViewModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using System.Collections.Generic;
using MyerSplashShared.Shared;
using MyerSplash.Interface;

namespace MyerSplash.Model
{
    public class UnsplashImage : UnsplashImageBase
    {
        public UnsplashImage()
        {
        }

        public static ObservableCollection<UnsplashImage> ParseListFromJson(string json)
        {
            var list = new ObservableCollection<UnsplashImage>();
            var array = JsonArray.Parse(json);
            foreach (var item in array)
            {
                var image = new UnsplashImage();
                image.ParseObjectFromJson(item.ToString());
                list.Add(image);
            }
            return list;
        }

        protected override void ParseObjectFromJson(string json)
        {
            var obj = JsonObject.Parse(json);

            var isFeatured = JsonParser.GetBooleanFromJsonObj(obj, "featured", false);
            
            var urls = JsonParser.GetJsonObjFromJsonObj(obj, "urls");
            var smallImageUrl = JsonParser.GetStringFromJsonObj(urls, "small");
            var fullImageUrl = JsonParser.GetStringFromJsonObj(urls, "full");
            var regularImageUrl = JsonParser.GetStringFromJsonObj(urls, "regular");
            var thumbImageUrl = JsonParser.GetStringFromJsonObj(urls, "thumb");
            var rawImageUrl = JsonParser.GetStringFromJsonObj(urls, "raw");
            var color = JsonParser.GetStringFromJsonObj(obj, "color");
            var width = JsonParser.GetNumberFromJsonObj(obj, "width");
            var height = JsonParser.GetNumberFromJsonObj(obj, "height");
            var userObj = JsonParser.GetJsonObjFromJsonObj(obj, "user");
            var userName = JsonParser.GetStringFromJsonObj(userObj, "name");
            var id = JsonParser.GetStringFromJsonObj(obj, "id");
            var likes = JsonParser.GetNumberFromJsonObj(obj, "likes");
            var time = JsonParser.GetStringFromJsonObj(obj, "created_at");

            this.SmallImageUrl = smallImageUrl;
            this.FullImageUrl = fullImageUrl;
            this.RegularImageUrl = regularImageUrl;
            this.ThumbImageUrl = thumbImageUrl;
            this.RawImageUrl = rawImageUrl;
            this.ColorValue = color;
            this.Width = width;
            this.Height = height;
            this.Owner = new UnsplashUser() { Name = userName };
            this.ID = id;
            this.Likes = (int)likes;
            this.CreateTime = DateTime.Parse(time);
        }
    }
}
