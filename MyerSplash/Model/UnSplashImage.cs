using JP.Utils.Data.Json;
using System.Collections.ObjectModel;
using Windows.Data.Json;
using System;

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
                image.ParseObjectFromJsonString(item.ToString());
                list.Add(image);
            }
            return list;
        }

        public override void ParseObjectFromJsonObject(JsonObject obj)
        {
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
            this.Owner = new UnsplashUser();
            this.Owner.ParseObjectFromJsonObject(userObj);
            this.ID = id;
            this.Likes = (int)likes;
            this.CreateTime = DateTime.Parse(time);
        }

        public override void ParseObjectFromJsonString(string json)
        {
            var obj = JsonObject.Parse(json);
            ParseObjectFromJsonObject(obj);
        }
    }
}
