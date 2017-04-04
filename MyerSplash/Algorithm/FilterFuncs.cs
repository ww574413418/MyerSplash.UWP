using ColorMine.ColorSpaces;
using ColorMine.ColorSpaces.Comparisons;
using CompositionHelper.Helper;
using MyerSplash.Model;
using System;
using System.Diagnostics;

namespace MyerSplash.Algorithm
{
    public static class FilterFuncs
    {
        public static Func<UnsplashImageBase, ColorFilter, bool> FilterByRGB = (image, filter) =>
        {
            var color = image.ColorValue.ToColor();

            var imageR = color.R;
            var imageG = color.G;
            var imageB = color.B;

            var filterR = filter.Color.R;
            var filterG = filter.Color.G;
            var filterB = filter.Color.B;

            var distance = Math.Sqrt(Math.Pow(imageR - filterR, 2) + Math.Pow(imageG - filterG, 2)
                + Math.Pow(imageB - filterB, 2));

            return distance < 100;
        };

        public static Func<UnsplashImageBase, ColorFilter, bool> FilterByLAB = (image, filter) =>
        {
            var imageColor = image.ColorValue.ToColor();
            var imageColorRgb = new Rgb() { R = imageColor.R, G = imageColor.G, B = imageColor.B };
            var expectedColorRgb = new Rgb() { R = filter.Color.R, G = filter.Color.G, B = filter.Color.B };
            var com = new CieDe2000Comparison();
            var result = com.Compare(imageColorRgb, expectedColorRgb);

            Debug.WriteLine($"=================Result:{result}");

            return result < 10;
        };
    }
}
