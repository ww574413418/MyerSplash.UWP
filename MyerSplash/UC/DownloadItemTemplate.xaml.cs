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

        public DownloadItemTemplate()
        {
            this.InitializeComponent();

            _compositor = this.GetVisual().Compositor;
        }

        private void SetAsBtn_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void RootGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RootGrid.Clip = new RectangleGeometry()
            {
                Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height)
            };
        }
    }
}
