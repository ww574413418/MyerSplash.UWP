using JP.Utils.Data;
using System;
using Windows.Globalization;
using Windows.System.UserProfile;

namespace MyerSplash.Common
{
    [Obsolete]
    public static class GlobalLocaleHelper
    {
        public static void SetupLang(string locale)
        {
            if (locale != null)
            {
                ApplicationLanguages.PrimaryLanguageOverride = locale;
                return;
            }
            if (LocalSettingHelper.HasValue("AppLang") == false)
            {
                var lang = GlobalizationPreferences.Languages[0];
                if (lang.Contains("zh"))
                {
                    ApplicationLanguages.PrimaryLanguageOverride = "zh-CN";
                }
                else ApplicationLanguages.PrimaryLanguageOverride = "en-US";

                LocalSettingHelper.AddValue("AppLang", ApplicationLanguages.PrimaryLanguageOverride);
            }
            else ApplicationLanguages.PrimaryLanguageOverride = LocalSettingHelper.GetValue("AppLang");
        }
    }
}