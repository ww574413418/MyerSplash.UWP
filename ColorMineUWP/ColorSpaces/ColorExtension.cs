using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace ColorMineUWP.ColorSpaces
{
    public static class ColorExtension
    {
        public static double GetHue(this Color color)
        {
            var r = color.R / 255d;
            var g = color.G / 255d;
            var b = color.B / 255d;

            var min = Math.Min(Math.Min(r, g), b);
            var max = Math.Max(Math.Max(r, g), b);

            var l = (min + max) / 2;
            var s = (max - min) / (max + min);

            var h = 0d;
            if (r > g && r > b)
            {
                h = (g - b) / (max - min);
            }
            else if (g > r && g > b)
            {
                h = 2d + (b - r) / (max - min);
            }
            else
            {
                h = 4d + (r - g) / (max - min);
            }

            return h * 60;
        }

        public static double GetSaturation(this Color color)
        {
            var r = color.R / 255d;
            var g = color.G / 255d;
            var b = color.B / 255d;

            var min = Math.Min(Math.Min(r, g), b);
            var max = Math.Max(Math.Max(r, g), b);

            var s = (max - min) / (max + min);
            return s;
        }

        public static double GetBrightness(this Color color)
        {
            var r = color.R / 255d;
            var g = color.G / 255d;
            var b = color.B / 255d;

            var min = Math.Min(Math.Min(r, g), b);
            var max = Math.Max(Math.Max(r, g), b);

            var l = (min + max) / 2;
            return l;
        }
    }
}
