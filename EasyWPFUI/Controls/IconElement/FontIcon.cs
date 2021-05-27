using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace EasyWPFUI.Controls
{
    public class FontIcon : IconElement
    {
        private TextBlock textBlock;

        #region FontFamily Property

        public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(FontIcon), new FrameworkPropertyMetadata(new FontFamily("Segoe MDL2 Assets")));

        public FontFamily FontFamily
        {
            get
            {
                return (FontFamily)GetValue(FontFamilyProperty);
            }
            set
            {
                SetValue(FontFamilyProperty, value);
            }
        }

        #endregion

        #region FontSize Property

        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(FontIcon), new FrameworkPropertyMetadata(12d));

        public double FontSize
        {
            get
            {
                return (double)GetValue(FontSizeProperty);
            }
            set
            {
                SetValue(FontSizeProperty, value);
            }
        }

        #endregion

        #region FontStyle Property

        public static readonly DependencyProperty FontStyleProperty = DependencyProperty.Register("FontStyle", typeof(FontStyle), typeof(FontIcon), new FrameworkPropertyMetadata(FontStyles.Normal));

        public FontStyle FontStyle
        {
            get
            {
                return (FontStyle)GetValue(FontStyleProperty);
            }
            set
            {
                SetValue(FontStyleProperty, value);
            }
        }

        #endregion

        #region FontWeight Property

        public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(FontIcon), new FrameworkPropertyMetadata(FontWeights.Normal));

        public FontWeight FontWeight
        {
            get
            {
                return (FontWeight)GetValue(FontWeightProperty);
            }
            set
            {
                SetValue(FontWeightProperty, value);
            }
        }

        #endregion

        #region GlyphProperty

        public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register("Glyph", typeof(string), typeof(FontIcon), new FrameworkPropertyMetadata(string.Empty));

        public string Glyph
        {
            get
            {
                return (string)GetValue(GlyphProperty);
            }
            set
            {
                SetValue(GlyphProperty, value);
            }
        }

        #endregion

        #region Methods

        static FontIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FontIcon), new FrameworkPropertyMetadata(typeof(FontIcon)));
            HorizontalAlignmentProperty.OverrideMetadata(typeof(FontIcon), new FrameworkPropertyMetadata(HorizontalAlignment.Center));
            VerticalAlignmentProperty.OverrideMetadata(typeof(FontIcon), new FrameworkPropertyMetadata(VerticalAlignment.Center));
        }

        public FontIcon()
        {

        }

        protected override void InitializeComponent()
        {
            textBlock = new TextBlock();

            textBlock.SetBinding(TextBlock.ForegroundProperty, new Binding() { Source = this, Path = new PropertyPath(ForegroundProperty) });

            textBlock.SetBinding(TextBlock.FontFamilyProperty, new Binding() { Source = this, Path = new PropertyPath(FontFamilyProperty) });
            textBlock.SetBinding(TextBlock.FontSizeProperty, new Binding() { Source = this, Path = new PropertyPath(FontSizeProperty) });
            textBlock.SetBinding(TextBlock.FontStyleProperty, new Binding() { Source = this, Path = new PropertyPath(FontStyleProperty) });
            textBlock.SetBinding(TextBlock.FontWeightProperty, new Binding() { Source = this, Path = new PropertyPath(FontWeightProperty) });
            textBlock.SetBinding(TextBlock.TextProperty, new Binding() { Source = this, Path = new PropertyPath(GlyphProperty) });
            textBlock.SetBinding(TextBlock.VerticalAlignmentProperty, new Binding() { Source = this, Path = new PropertyPath(VerticalAlignmentProperty) });
            textBlock.SetBinding(TextBlock.HorizontalAlignmentProperty, new Binding() { Source = this, Path = new PropertyPath(HorizontalAlignmentProperty) });
            textBlock.SetBinding(TextBlock.SnapsToDevicePixelsProperty, new Binding() { Source = this, Path = new PropertyPath(SnapsToDevicePixelsProperty) });
            textBlock.SetBinding(TextBlock.FocusableProperty, new Binding() { Source = this, Path = new PropertyPath(FocusableProperty) });
            textBlock.SetBinding(TextBlock.FocusVisualStyleProperty, new Binding() { Source = this, Path = new PropertyPath(FocusVisualStyleProperty) });

            AddChild(textBlock);
        }

        #endregion
    }
}
