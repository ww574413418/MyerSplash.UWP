﻿using JP.Utils.UI;
using MyerSplash.Common;
using MyerSplash.Model;
using MyerSplash.ViewModel;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace MyerSplash.View.Uc
{
    public sealed partial class ImageListControl : UserControl
    {
        public event Action<ImageItem, FrameworkElement> OnClickItemStarted;
        public event Action<ScrollViewer> OnScrollViewerViewChanged;

        private const float SCALE_ANIMATION_FACTOR = 1.05f;

        private MainViewModel MainVM
        {
            get
            {
                return this.DataContext as MainViewModel;
            }
        }

        private Visual _tappedContainerVisual;
        private Visual _listVisual;
        private Compositor _compositor;

        public int TargetOffsetX;
        public int TargetOffsetY;

        private ScrollViewer _scrollViewer;
        private FrameworkElement _tappedContainer;

        public bool Refreshing
        {
            get { return (bool)GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }

        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register("Refreshing", typeof(bool), typeof(ImageListControl),
                new PropertyMetadata(false, OnRefreshingPropertyChanged));

        private static void OnRefreshingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ImageListControl;
            if ((bool)e.NewValue)
            {
                control.ShowRefreshing();
            }
            else control.HideRefreshing();
        }

        public ImageListControl()
        {
            this.InitializeComponent();
            this._compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            this._listVisual = ImageGridView.GetVisual();
        }

        private void ImageGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as ImageItem;
            TapItem(item);
        }

        private bool CheckListImageDownloaded(ImageItem image)
        {
            return !string.IsNullOrEmpty(image.ListImageBitmap.LocalPath);
        }

        private void TapItem(ImageItem image)
        {
            if (!CheckListImageDownloaded(image))
            {
                return;
            }

            _tappedContainer = ImageGridView.ContainerFromItem(image) as FrameworkElement;

            var rootGrid = (_tappedContainer as GridViewItem).ContentTemplateRoot as Grid;
            var contentGrid = rootGrid.Children[0] as Grid;

            _tappedContainerVisual = ElementCompositionPreview.GetElementVisual(_tappedContainer);

            var maskBorder = contentGrid.Children[2] as FrameworkElement;
            var img = contentGrid.Children[1] as FrameworkElement;

            ToggleItemPointOverAnimation(maskBorder, img, false);

            OnClickItemStarted?.Invoke(image, contentGrid);
        }

        public void ScrollToTop()
        {
            ImageGridView.GetScrollViewer().ChangeView(null, 0, null);
        }

        #region List Animation
        private void AdaptiveGridView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            int index = args.ItemIndex;

            if (!args.InRecycleQueue)
            {
                args.ItemContainer.Loaded -= ItemContainer_Loaded;
                args.ItemContainer.Loaded += ItemContainer_Loaded;
            }
        }

        private void ItemContainer_Loaded(object sender, RoutedEventArgs e)
        {
            var itemsPanel = (ItemsWrapGrid)ImageGridView.ItemsPanelRoot;
            var itemContainer = (GridViewItem)sender;
            var itemIndex = ImageGridView.IndexFromContainer(itemContainer);

            // Don't animate if we're not in the visible viewport
            if (itemIndex >= itemsPanel.FirstVisibleIndex && itemIndex <= itemsPanel.LastVisibleIndex)
            {
                var itemVisual = itemContainer.GetVisual();
                var delayIndex = itemIndex - itemsPanel.FirstVisibleIndex;

                itemVisual.Opacity = 0f;
                itemVisual.SetTranslation(new Vector3(50, 0, 0));

                // Create KeyFrameAnimations
                var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
                offsetAnimation.InsertKeyFrame(1f, 0f);
                offsetAnimation.Duration = TimeSpan.FromMilliseconds(700);
                offsetAnimation.DelayTime = TimeSpan.FromMilliseconds((delayIndex * 30));

                var fadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
                fadeAnimation.InsertKeyFrame(1f, 1f);
                fadeAnimation.Duration = TimeSpan.FromMilliseconds(700);
                fadeAnimation.DelayTime = TimeSpan.FromMilliseconds(delayIndex * 30);

                // Start animations
                itemVisual.StartAnimation(itemVisual.GetTranslationXPropertyName(), offsetAnimation);
                itemVisual.StartAnimation("Opacity", fadeAnimation);
            }
            itemContainer.Loaded -= ItemContainer_Loaded;
        }
        #endregion

        private void ImageGridView_Loaded(object sender, RoutedEventArgs e)
        {
            _scrollViewer = ImageGridView.GetScrollViewer();
            _scrollViewer.ViewChanging -= ScrollViewer_ViewChanging;
            _scrollViewer.ViewChanging += ScrollViewer_ViewChanging;
        }

        private void ScrollViewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            OnScrollViewerViewChanged?.Invoke(sender as ScrollViewer);
        }

        private void RootGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var rootGrid = sender as Grid;

            rootGrid.PointerEntered += RootGrid_PointerEntered;
            rootGrid.PointerExited += RootGrid_PointerExited;

            var maskBorder = rootGrid.Children[2] as FrameworkElement;
            var maskVisual = maskBorder.GetVisual();
            maskVisual.Opacity = 0f;
        }

        private void RootGrid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType != PointerDeviceType.Mouse)
            {
                return;
            }
            var rootGrid = sender as Grid;
            var maskBorder = rootGrid.Children[2] as FrameworkElement;
            var img = rootGrid.Children[1] as FrameworkElement;

            ToggleItemPointOverAnimation(maskBorder, img, false);
        }

        private void RootGrid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType != PointerDeviceType.Mouse)
            {
                return;
            }
            var rootGrid = sender as Grid;
            var maskBorder = rootGrid.Children[2] as FrameworkElement;
            var img = rootGrid.Children[1] as FrameworkElement;
            var btn = rootGrid.FindName("DownloadBtn") as Button;

            var unsplashImage = rootGrid.DataContext as ImageItem;
            if (unsplashImage.DownloadStatus != DownloadStatus.Pending)
            {
                btn.Visibility = Visibility.Collapsed;
            }
            else
            {
                btn.Visibility = Visibility.Visible;
            }
            if (!CheckListImageDownloaded(unsplashImage))
            {
                btn.Visibility = Visibility.Collapsed;
            }

            ToggleItemPointOverAnimation(maskBorder, img, true);
        }

        private void RootGrid_Unloaded(object sender, RoutedEventArgs e)
        {
            var rootGrid = sender as Grid;
            rootGrid.PointerEntered -= RootGrid_PointerEntered;
            rootGrid.PointerExited -= RootGrid_PointerExited;
        }

        private ScalarKeyFrameAnimation CreateScaleAnimation(bool show)
        {
            var scaleAnimation = _compositor.CreateScalarKeyFrameAnimation();
            scaleAnimation.InsertKeyFrame(1f, show ? SCALE_ANIMATION_FACTOR : 1f);
            scaleAnimation.Duration = TimeSpan.FromMilliseconds(1000);
            scaleAnimation.StopBehavior = AnimationStopBehavior.LeaveCurrentValue;
            return scaleAnimation;
        }

        private ScalarKeyFrameAnimation CreateFadeAnimation(bool show)
        {
            var fadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
            fadeAnimation.InsertKeyFrame(1f, show ? 1f : 0f);
            fadeAnimation.Duration = TimeSpan.FromMilliseconds(500);

            return fadeAnimation;
        }

        private void ToggleItemPointOverAnimation(FrameworkElement mask, FrameworkElement img, bool show)
        {
            var maskVisual = mask.GetVisual();
            var imgVisual = img.GetVisual();

            var fadeAnimation = CreateFadeAnimation(show);
            var scaleAnimation = CreateScaleAnimation(show);

            if (imgVisual.CenterPoint.X == 0 && imgVisual.CenterPoint.Y == 0)
            {
                imgVisual.CenterPoint = new Vector3((float)mask.ActualWidth / 2, (float)mask.ActualHeight / 2, 0f);
            }

            maskVisual.StartAnimation("Opacity", fadeAnimation);
            if (AppSettings.Instance.EnableScaleAnimation)
            {
                imgVisual.StartAnimation("Scale.x", scaleAnimation);
                imgVisual.StartAnimation("Scale.y", scaleAnimation);
            }
        }

        private void RootGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var rootGrid = sender as Grid;
            rootGrid.Clip = new RectangleGeometry()
            {
                Rect = new Rect(0, 0, rootGrid.ActualWidth, rootGrid.ActualHeight)
            };
        }

        private void ShowRefreshing()
        {
            if (_scrollViewer != null)
            {
                _scrollViewer.ChangeView(null, 0, null);
            }
            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, 100f);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(300);

            _listVisual.StartAnimation(_listVisual.GetTranslationYPropertyName(), offsetAnimation);
            LoadingControl.Visibility = Visibility.Visible;
            LoadingControl.Start();
        }

        private void HideRefreshing()
        {
            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, 0f);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(300);

            _listVisual.StartAnimation(_listVisual.GetTranslationYPropertyName(), offsetAnimation);
            LoadingControl.Visibility = Visibility.Collapsed;
            LoadingControl.Stop();
        }

        private async void RootGrid_DragStarting(UIElement sender, DragStartingEventArgs args)
        {
            var image = (sender as FrameworkElement).DataContext as ImageItem;
            var file = await StorageFile.GetFileFromPathAsync(image.ListImageBitmap.LocalPath);
            if (file == null)
            {
                args.Cancel = true;
                return;
            }
            args.Data.SetStorageItems(new List<StorageFile>() { file });
            args.Data.RequestedOperation = DataPackageOperation.Copy;
            args.Data.SetText(image.ShareText);
        }

        private void RootGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var image = (sender as FrameworkElement).DataContext as ImageItem;
            TapItem(image);
        }

        private void DownloadBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
        }
    }
}
