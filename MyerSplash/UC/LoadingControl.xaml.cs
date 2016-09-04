using CompositionHelper;
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
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MyerSplash.UC
{
    public sealed partial class LoadingControl : UserControl
    {
        private Compositor _compositor;
        private Visual _rootVisual;
        private Visual _e1Visual;
        private Visual _e2Visual;

        public LoadingControl()
        {
            this.InitializeComponent();
           
            if(!DesignMode.DesignModeEnabled)
            {
                this.SizeChanged += LoadingControl_SizeChanged;

                _compositor = RootGrid.GetVisual().Compositor;
                _rootVisual = RootGrid.GetVisual();
                _e1Visual = E1.GetVisual();
                _e2Visual = E2.GetVisual();

                Start();
            }
        }

        private void LoadingControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Clip = new RectangleGeometry()
            {
                Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height),
            };
        }

        public void Start()
        {
            StartStory.Begin();
        }

        public void Stop()
        {
            StartStory.Stop();
        }
    }
}
