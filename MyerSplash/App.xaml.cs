using Microsoft.QueryStringDotNET;
using MyerSplash.Common;
using MyerSplash.View;
using MyerSplash.ViewModel;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MyerSplash
{
    sealed partial class App : Application
    {
        public static MainViewModel MainVM { get; set; }

        public static ViewModelLocator VMLocator
        {
            get
            {
                return Current.Resources["Locator"] as ViewModelLocator;
            }
        }

        public static AppSettings AppSettings
        {
            get
            {
                return Current.Resources["AppSettings"] as AppSettings;
            }
        }

        public static string GetAppVersion()
        {
            var packageVersion = Package.Current.Id.Version;
            var version = $"{packageVersion.Major}.{packageVersion.Minor}.{packageVersion.Build}";
            return version;
        }

        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.UnhandledException += App_UnhandledException;
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
        }

#pragma warning disable 1998
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            if (e.PrelaunchActivated) return;
            var task = JumpListHelper.SetupJumpList();
            CreateFrameAndNavigate(e.Arguments);
        }
#pragma warning restore    

        private Frame CreateFrameAndNavigate(string arg)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();
                rootFrame.Background = App.Current.Resources["MyerSplashDarkColor"] as SolidColorBrush;
                Window.Current.Content = rootFrame;
            }

            rootFrame.Navigate(typeof(MainPage), arg);
            Window.Current.Activate();

            TitleBarHelper.SetUpLightTitleBar();

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            SystemNavigationManager.GetForCurrentView().BackRequested -= App_BackRequested;
            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;

            return rootFrame;
        }

        protected override void OnActivated(IActivatedEventArgs e)
        {
            string arg = null;
            if (e is ToastNotificationActivatedEventArgs)
            {
                var toastActivationArgs = e as ToastNotificationActivatedEventArgs;
                var args = QueryString.Parse(toastActivationArgs.Argument);
                arg = args[Key.ACTION_KEY];
                if (args.Contains(Key.FILE_PATH_KEY))
                {
                    var filePath = args[Key.FILE_PATH_KEY];
                    if (filePath != null)
                    {
                        arg = toastActivationArgs.Argument;
                    }
                }
            }
            CreateFrameAndNavigate(arg);
        }

        private void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = NavigationService.GoBack();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }
    }
}
