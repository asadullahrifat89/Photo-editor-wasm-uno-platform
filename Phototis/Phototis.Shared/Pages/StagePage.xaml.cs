﻿using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Uno.Storage.Pickers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Pointer = Microsoft.UI.Xaml.Input.Pointer;

namespace Phototis
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StagePage : Page
    {
        #region Fields

        private List<Photo> photos = new List<Photo>();

        private double windowWidth, windowHeight;

        private Photo selectedPhoto;

        bool _isPointerCaptured;
        double _pointerX;
        double _pointerY;
        double _objectLeft;
        double _objectTop;

        private PhotoElement photoElement;
        private PointerPoint currentPointerPoint;
        private Pointer currentPointer;

        #endregion

        #region Ctor

        public StagePage()
        {
            this.InitializeComponent();
            this.Loaded += StagePage_Loaded;
            this.Unloaded += StagePage_Unloaded;
        }

        private void StagePage_Loaded(object sender, RoutedEventArgs e)
        {
            NumberBoxWidth.Value = Window.Current.Bounds.Width - 10;
            NumberBoxHeight.Value = Window.Current.Bounds.Height - 10;

            SizeChanged += StagePage_SizeChanged;
        }

        private void StagePage_Unloaded(object sender, RoutedEventArgs e)
        {
            SizeChanged -= StagePage_SizeChanged;
        }

        private void StagePage_SizeChanged(object sender, SizeChangedEventArgs args)
        {
            windowWidth = args.NewSize.Width - 10;
            windowHeight = args.NewSize.Height - 10;

            WorkspaceScroller.Width = windowWidth;
            WorkspaceScroller.Height = windowHeight;

#if DEBUG
            Console.WriteLine($"View Size: {windowWidth} x {windowHeight}");
#endif
        }

        #endregion

        #region Events

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is List<Photo> photos)
            {
                this.photos = photos;
            }

            ImageContainer.ItemsSource = this.photos;

            base.OnNavigatedTo(e);
        }

        private void NumberBoxWidth_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            Workspace.Width = args.NewValue;

            Console.WriteLine($"Workspace Size: {Workspace.Width} x {Workspace.Height}");
        }

        private void NumberBoxHeight_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            Workspace.Height = args.NewValue;

            Console.WriteLine($"Workspace Size: {Workspace.Width} x {Workspace.Height}");
        }

        private void Workspace_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_isPointerCaptured && photoElement is not null)
            {
                currentPointerPoint = e.GetCurrentPoint(Workspace);
                currentPointer = e.Pointer;

                DragElement(photoElement);
            }

            //Console.WriteLine("Workspace_PointerMoved");
        }

        private void Workspace_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            currentPointerPoint = e.GetCurrentPoint(Workspace);
            currentPointer = e.Pointer;

            // if image drawer is open then insert new new item
            if (ImageDrawerToggle.IsChecked.Value && selectedPhoto is not null)
            {
                PhotoElement photoElement = new PhotoElement() { Width = 400, Height = 400 };
                photoElement.Source = selectedPhoto.DataUrl;

                Canvas.SetLeft(photoElement, currentPointerPoint.Position.X - 200);
                Canvas.SetTop(photoElement, currentPointerPoint.Position.Y - 200);

                photoElement.PointerPressed += PhotoElement_PointerPressed;
                photoElement.PointerReleased += PhotoElement_PointerReleased;

                Workspace.Children.Add(photoElement);

                selectedPhoto = null;
            }

#if DEBUG
            Console.WriteLine("Workspace_PointerPressed");
#endif
        }

        private void Workspace_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // this works for mobile and tablets as cursor is not available
            currentPointerPoint = e.GetCurrentPoint(Workspace);
            currentPointer = e.Pointer;

            if (_isPointerCaptured && photoElement is not null)
            {
                DragElement(photoElement);

                DragRelease(photoElement);
                photoElement = null;
            }

            Console.WriteLine("Workspace_PointerReleased");
        }     

        private void PhotoElement_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            currentPointerPoint = e.GetCurrentPoint(Workspace);
            currentPointer = e.Pointer;

            photoElement = (PhotoElement)sender;

            DragStart(photoElement);
        }

        private void PhotoElement_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            photoElement = (PhotoElement)sender;
            DragRelease(photoElement);
        }

        private void ImageDrawerToggle_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void ImageContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedPhoto = ImageContainer.SelectedItem as Photo;
        }

        //private void GrayScaleSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        //{
        //    Image2.SetGrayScale(e.NewValue);
        //}

        //private void ContrastSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        //{
        //    Image2.SetContrast(e.NewValue);
        //}

        //private void BrightnessSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        //{
        //    Image2.SetBrightness(e.NewValue);
        //}

        //private void SaturationSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        //{
        //    Image2.SetSaturation(e.NewValue);
        //}

        //private void SepiaSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        //{
        //    Image2.SetSepia(e.NewValue);
        //}

        //private void InvertSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        //{
        //    Image2.SetInvert(e.NewValue);
        //}

        //private void HueRotateSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        //{
        //    Image2.SetHueRotate(e.NewValue);
        //}

        //private void BlurSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        //{
        //    Image2.SetBlur(e.NewValue);
        //}

        //private void ResetButton_Click(object sender, RoutedEventArgs e)
        //{
        //    GrayScaleSlider.Value = 0;
        //    ContrastSlider.Value = 100;
        //    BrightnessSlider.Value = 100;
        //    SaturationSlider.Value = 100;
        //    SepiaSlider.Value = 0;
        //    InvertSlider.Value = 0;
        //    HueRotateSlider.Value = 0;
        //    BlurSlider.Value = 0;
        //} 

        #endregion

        #region Methods       

        public void DragStart(UIElement uielement)
        {
            // Drag start of a constuct
            _objectLeft = Canvas.GetLeft(uielement);
            _objectTop = Canvas.GetTop(uielement);

            //var currentPoint = e.GetCurrentPoint(canvas);

            // Remember the pointer position:
            _pointerX = currentPointerPoint.Position.X;
            _pointerY = currentPointerPoint.Position.Y;

            uielement.CapturePointer(currentPointer);
            uielement.Opacity = 0.6d;

            _isPointerCaptured = true;

#if DEBUG
            Console.WriteLine("DragStart");
#endif
        }

        public void DragElement(UIElement uielement)
        {
            if (_isPointerCaptured)
            {
                //var currentPoint = e.GetCurrentPoint(canvas);

                // Calculate the new position of the object:
                double deltaH = currentPointerPoint.Position.X - _pointerX;
                double deltaV = currentPointerPoint.Position.Y - _pointerY;

                _objectLeft = deltaH + _objectLeft;
                _objectTop = deltaV + _objectTop;

                // Update the object position:
                Canvas.SetLeft(uielement, _objectLeft);
                Canvas.SetTop(uielement, _objectTop);

                // Remember the pointer position:
                _pointerX = currentPointerPoint.Position.X;
                _pointerY = currentPointerPoint.Position.Y;
            }
        }

        public void DragRelease(UIElement uielement)
        {
            _isPointerCaptured = false;
            uielement.ReleasePointerCapture(currentPointer);
            uielement.Opacity = 1;

#if DEBUG
            Console.WriteLine("DragRelease");
#endif
        }

        #endregion
    }
}
