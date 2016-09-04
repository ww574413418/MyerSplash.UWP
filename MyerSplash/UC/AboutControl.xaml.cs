using CompositionHelper;
using MyerSplash.Common;
using MyerSplash.Interface;
using MyerSplash.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MyerSplash.UC
{
    public sealed partial class AboutControl : NavigableUserControl
    {
        private AboutViewModel AboutVM { get; set; }

        public AboutControl()
        {
            this.InitializeComponent();
            this.DataContext = AboutVM = new AboutViewModel();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Shown = false;
        }
    }
}
