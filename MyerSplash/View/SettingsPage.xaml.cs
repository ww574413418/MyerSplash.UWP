using MyerSplash.Common;
using MyerSplash.UC;
using MyerSplash.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MyerSplash.View
{
    public sealed partial class SettingsPage : CustomizedTitleBarPage
    {
        private SettingsViewModel SettingsVM { get; set; }

        public SettingsPage()
        {
            this.InitializeComponent();
            this.DataContext = SettingsVM = new SettingsViewModel();
        }

        private void TitleBar_OnClickBackBtn()
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            TitleBarHelper.SetUpDarkTitleBar();
        }
    }
}
