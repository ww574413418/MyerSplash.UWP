using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media;

namespace MyerSplash.Common
{
    public class StatusBarHelper
    {
        private static StatusBar sb = StatusBar.GetForCurrentView();

#pragma warning disable 4014
        public static void SetUpStatusBar()
        {
            sb.HideAsync();
        }
#pragma warning restore
    }
}
