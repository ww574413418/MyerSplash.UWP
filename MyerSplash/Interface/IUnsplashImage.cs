using MyerSplash.Model;
using MyerSplashShared.Shared;
using System;
using Windows.UI.Xaml.Media;

namespace MyerSplash.Interface
{
    public interface IUnsplashImage
    {
        string ID { get; set; }

        string RawImageUrl { get; set; }

        string FullImageUrl { get; set; }

        string RegularImageUrl { get; set; }

        string SmallImageUrl { get; set; }

        string ThumbImageUrl { get; set; }

        CachedBitmapSource ListImageBitmap { get; set; }

        CachedBitmapSource LargeBitmap { get; set; }

        SolidColorBrush MajorColor { get; set; }

        SolidColorBrush BackColor { get; set; }

        string ColorValue { get; set; }

        double Width { get; set; }

        double Height { get; set; }

        UnsplashUser Owner { get; set; }

        bool Liked { get; set; }

        int Likes { get; set; }

        string LikesString { get; }

        DateTime CreateTime { get; set; }

        string CreateTimeString { get; }
    }
}
