﻿using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Text;
using Uno.Foundation;
using Uno.UI.Runtime.WebAssembly;

namespace Phototis
{
    [HtmlElement("img")]
    public sealed class ImageElement : FrameworkElement
    {
        #region Ctor

        public ImageElement()
        {
            this.SetHtmlAttribute("draggable", "false");
            this.SetHtmlAttribute("loading", "lazy");
            this.SetCssStyle("object-fit", "contain");            
        }

        #endregion

        #region Properties

        private string _Id;

        public string Id
        {
            get { return _Id; }
            set
            {
                _Id = value;

                if (!_Id.IsNullOrBlank())
                {
                    this.SetHtmlAttribute("id", _Id);
                }
            }
        }


        private double _Grayscale = 0;

        public double Grayscale
        {
            get { return _Grayscale; }
            set
            {
                _Grayscale = value;
                SetFilter();
            }
        }

        private double _Contrast = 100;

        public double Contrast
        {
            get { return _Contrast; }
            set
            {
                _Contrast = value;
                SetFilter();
            }
        }

        private double _Brightness = 100;

        public double Brightness
        {
            get { return _Brightness; }
            set
            {
                _Brightness = value;
                SetFilter();
            }
        }

        private double _Saturation = 100;

        public double Saturation
        {
            get { return _Saturation; }
            set
            {
                _Saturation = value;
                SetFilter();
            }
        }

        private double _Sepia = 0;

        public double Sepia
        {
            get { return _Sepia; }
            set
            {
                _Sepia = value;
                SetFilter();
            }
        }

        private double _Invert = 0;

        public double Invert
        {
            get { return _Invert; }
            set
            {
                _Invert = value;
                SetFilter();
            }
        }

        private double _Hue = 0;

        public double Hue
        {
            get { return _Hue; }
            set
            {
                _Hue = value;
                SetFilter();
            }
        }

        private double _Blur = 0;

        public double Blur
        {
            get { return _Blur; }
            set
            {
                _Blur = value;
                SetFilter();
            }
        }

        private double _Opacity = 1;

        public new double Opacity
        {
            get { return _Opacity; }
            set
            {
                _Opacity = value;
                SetFilter();
            }
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(string), typeof(ImageElement), new PropertyMetadata(default(string), OnSourceChanged));

        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        private static void OnSourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (dependencyObject is ImageElement image)
            {
                var encodedSource = WebAssemblyRuntime.EscapeJs("" + args.NewValue);
                image.SetHtmlAttribute("src", encodedSource);
                image.SetFilter();
            }
        }

        #endregion

        #region Methods

        public void SetDefaults()
        {
            _Grayscale = 0;
            _Contrast = 100;
            _Brightness = 100;
            _Saturation = 100;
            _Sepia = 0;
            _Invert = 0;
            _Hue = 0;
            _Blur = 0;
            _Opacity = 1;

            SetFilter();
        }

        public string GetCssFilter()
        {
            return $"grayscale({_Grayscale}%) contrast({_Contrast}%) brightness({_Brightness}%) saturate({_Saturation}%) sepia({_Sepia}%) invert({_Invert}%) hue-rotate({_Hue}deg) blur({_Blur}px)";
        }

        public void SetFilter()
        {
            this.SetCssStyle("filter", GetCssFilter() + $" drop-shadow(0 0 0.75rem crimson)");
            this.SetCssStyle("opacity", $"{_Opacity}");
        }

        #endregion
    }
}