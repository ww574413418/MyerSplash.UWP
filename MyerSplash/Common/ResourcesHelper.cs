using Windows.ApplicationModel.Resources;

namespace MyerSplash.Common
{
    public class ResourcesHelper
    {
        private static ResourceLoader _loader = new ResourceLoader();

        public static string GetResString(string key)
        {
            return _loader.GetString(key);
        }

        public static string GetDicString(string key)
        {
            return App.Current.Resources[key] as string;
        }

        public static double GetDimentionInPixel(string key)
        {
            return (double)App.Current.Resources[key];
        }
    }
}
