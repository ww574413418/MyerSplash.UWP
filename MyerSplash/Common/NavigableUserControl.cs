using CompositionHelper;
using MyerSplash.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyerSplash.Common
{
    public class NavigableUserControl : UserControl, INavigableUserControl
    {
        public bool Shown
        {
            get { return (bool)GetValue(ShownProperty); }
            set { SetValue(ShownProperty, value); }
        }

        public static readonly DependencyProperty ShownProperty =
            DependencyProperty.Register("Shown", typeof(bool), typeof(NavigableUserControl),
                new PropertyMetadata(false, OnShownPropertyChanged));

        private static void OnShownPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as INavigableUserControl;
            if ((bool)e.NewValue) control.OnShow();
            else control.OnHide();
            control.ToggleAnimation();
        }

        private Compositor _compositor;
        private Visual _rootVisual;

        public NavigableUserControl()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                InitComposition();
                this.SizeChanged += UserControlBase_SizeChanged;
            }
        }

        private void UserControlBase_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!Shown)
            {
                _rootVisual.Offset = new Vector3(0f, (float)e.NewSize.Height, 0f);
            }
        }

        private void InitComposition()
        {
            _compositor = this.GetVisual().Compositor;
            _rootVisual = this.GetVisual();
            _rootVisual.Offset = new Vector3(0f, (float)this.ActualHeight, 0f);
        }

        public void OnHide()
        {
            App.MainVM.ShowSecondLayer = false;
        }

        public void OnShow()
        {
        }

        public void ToggleAnimation()
        {
            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, Shown ? 0f : (float)this.ActualHeight);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(800);

            var batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            _rootVisual.StartAnimation("Offset.y", offsetAnimation);
            batch.Completed += Batch_Completed;
            batch.End();
        }

        private void Batch_Completed(object sender, CompositionBatchCompletedEventArgs args)
        {
            if (!Shown)
            {
                //this.Visibility = Visibility.Collapsed;
            }
        }
    }
}
