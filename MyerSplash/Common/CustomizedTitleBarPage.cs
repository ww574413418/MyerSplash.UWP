using JP.Utils.Helper;
using MyerSplash.UC;
using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MyerSplash.Common
{
    public class CustomizedTitleBarPage : BindablePage
    {
        public CustomizedTitleBarPage()
        {
            
        }

        protected override void SetNavigationBackBtn()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        protected virtual void CustomTitleBar()
        {
            
        }
    }
}
