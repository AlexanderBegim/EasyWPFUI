using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace EasyWPFUI.Controls
{
    public class FontIconSource : IconSource
    {
        #region Glyph Property

        public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register("Glyph", typeof(string), typeof(FontIconSource), new FrameworkPropertyMetadata());

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

        #region FontSize Property

        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(FontIconSource), new FrameworkPropertyMetadata(20d));

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

        #region FontFamily Property

        public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(FontIconSource), new FrameworkPropertyMetadata());

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

        #region FontWeight Property

        public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(FontIconSource), new FrameworkPropertyMetadata(FontWeights.Normal));

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

        #region FontStyle Property

        public static readonly DependencyProperty FontStyleProperty = DependencyProperty.Register("FontStyle", typeof(FontStyle), typeof(FontIconSource), new FrameworkPropertyMetadata(FontStyles.Normal));

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
    }
}
