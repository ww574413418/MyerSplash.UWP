using CompositionHelper;
using CompositionHelper.Animation.Fluent;
using MyerSplash.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class DownloadItemTemplate : UserControl
    {
        private Compositor _compositor;

        private DownloadItem DownloadItem
        {
            get
            {
                return this.DataContext as DownloadItem;
            }
        }

        public DownloadItemTemplate()
        {
            this.InitializeComponent();
            _compositor = this.GetVisual().Compositor;
        }
    }
}
