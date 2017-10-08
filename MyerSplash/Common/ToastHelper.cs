using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using System.IO;
using Windows.UI.Notifications;

namespace MyerSplash.Common
{
    public static class ToastHelper
    {
        public static ToastNotification CreateToastNotification(string title, string content, string filePath)
        {
            ToastContent toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = title
                            },

                            new AdaptiveText()
                            {
                                Text = content
                            },
                        },

                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = "ms-appx:///Assets/Icon/ic_done_white_48dp.png",
                            HintCrop = ToastGenericAppLogoCrop.Default
                        },

                        HeroImage = new ToastGenericHeroImage()
                        {
                            Source = filePath,
                            AlternateText = Path.GetFileName(filePath)
                        }
                    }
                },

                Actions = new ToastActionsCustom()
                {
                    Buttons =
                    {
                        new ToastButton("Set as wallpaper", new QueryString()
                        {
                            { Key.ACTION_KEY, Value.SET_AS },
                            { Key.FILE_PATH_KEY, filePath }
                        }.ToString())
                    }
                },

                Launch = new QueryString()
                {
                    { Key.ACTION_KEY, Value.DOWNLOADS },
                }.ToString()
            };

            return new ToastNotification(toastContent.GetXml());
        }
    }
}