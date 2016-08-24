using JP.Utils.Helper;
using MyerSplash.UC;
using System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MyerSplash.Common
{
    public class CustomizedTitleBarPage : BindablePage
    {
        protected TitleBarControl TitleBarUC;

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
            if(DeviceHelper.IsDesktop)
            {
                CustomTitleBar();
            }
        }

        private void CustomTitleBar()
        {
            var currentContent = this.Content as Grid;
            if(currentContent==null)
            {
                throw new ArgumentNullException("The root element of the page should be Grid.");
            }

            TitleBarUC = new TitleBarControl();
            TitleBarUC.OnClickBackBtn += () =>
              {
                  if (Frame.CanGoBack) Frame.GoBack();
              };
            (currentContent as Grid).Children.Add(TitleBarUC);
            Grid.SetColumnSpan(TitleBarUC, 5);
            Grid.SetRowSpan(TitleBarUC, 5);
        }
    }
}
