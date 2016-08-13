using JP.Utils.UI;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace MyerSplash.Common
{
    public static class TitleBarHelper
    {
        public static void SetUpThemeTitleBar()
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = Colors.Transparent;
            titleBar.ForegroundColor = Colors.Black;
            titleBar.InactiveBackgroundColor = Colors.Transparent;
            titleBar.InactiveForegroundColor = Colors.Black;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonForegroundColor = Colors.Black;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveForegroundColor = Colors.Black;
            titleBar.ButtonHoverBackgroundColor = ColorConverter.HexToColor("#31FFFFFF");
            titleBar.ButtonHoverForegroundColor = Colors.Black;
            titleBar.ButtonPressedBackgroundColor = ColorConverter.HexToColor("#5EFFFFFF");
        }

        public static void SetUpBlackTitleBar()
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = ColorConverter.HexToColor("#01000000");
            titleBar.ForegroundColor = Colors.White;
            titleBar.InactiveBackgroundColor = ColorConverter.HexToColor("#01000000");
            titleBar.InactiveForegroundColor = Colors.White;
            titleBar.ButtonBackgroundColor = ColorConverter.HexToColor("#01000000");
            titleBar.ButtonForegroundColor = Colors.White;
            titleBar.ButtonInactiveBackgroundColor = ColorConverter.HexToColor("#01000000");
            titleBar.ButtonInactiveForegroundColor = Colors.White;
            titleBar.ButtonHoverBackgroundColor = ColorConverter.HexToColor("#B6202020");
            titleBar.ButtonHoverForegroundColor = Colors.White;
            titleBar.ButtonPressedBackgroundColor = ColorConverter.HexToColor("#EA101010");
            titleBar.ButtonPressedForegroundColor = Colors.White;
        }
    }
}
