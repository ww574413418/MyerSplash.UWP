using JP.Utils.UI;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media;

namespace MyerSplash.Common
{
    public static class TitleBarHelper
    {
        public static void SetUpThemeTitleBar()
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = ColorConverter.HexToColor("#00bebe");
            titleBar.ForegroundColor = Colors.Black;
            titleBar.InactiveBackgroundColor = ColorConverter.HexToColor("#00bebe");
            titleBar.InactiveForegroundColor = Colors.Black;
            titleBar.ButtonBackgroundColor = ColorConverter.HexToColor("#00bebe");
            titleBar.ButtonForegroundColor = Colors.Black;
            titleBar.ButtonInactiveBackgroundColor = ColorConverter.HexToColor("#00bebe");
            titleBar.ButtonInactiveForegroundColor = Colors.Black;
            titleBar.ButtonHoverBackgroundColor = ColorConverter.HexToColor("#FF10D6D6");
            titleBar.ButtonHoverForegroundColor = Colors.Black;
            titleBar.ButtonPressedBackgroundColor = ColorConverter.HexToColor("#FF06AEAE");
        }

        public static void SetUpDarkTitleBar()
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = (App.Current.Resources["TitleBarDarkBrush"] as SolidColorBrush).Color;
            titleBar.ForegroundColor = Colors.White;
            titleBar.InactiveBackgroundColor = (App.Current.Resources["TitleBarDarkBrush"] as SolidColorBrush).Color;
            titleBar.InactiveForegroundColor = Colors.White;
            titleBar.ButtonBackgroundColor = ColorConverter.HexToColor("#00000000");
            titleBar.ButtonForegroundColor = Colors.White;
            titleBar.ButtonInactiveBackgroundColor = ColorConverter.HexToColor("#00000000");
            titleBar.ButtonInactiveForegroundColor = Colors.White;
            titleBar.ButtonHoverBackgroundColor = ColorConverter.HexToColor("#73000000");
            titleBar.ButtonHoverForegroundColor = Colors.White;
            titleBar.ButtonPressedBackgroundColor = ColorConverter.HexToColor("#28000000");
            titleBar.ButtonPressedForegroundColor = Colors.White;
        }
    }
}
