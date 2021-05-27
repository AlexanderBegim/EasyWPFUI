using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Data;

namespace EasyWPFUI.Controls
{
    [TypeConverter(typeof(IconElementTypeConverter))]
    public class IconElement : FrameworkElement
    {
        private Grid rootGrid;

        #region Foreground Property

        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register("Foreground", typeof(Brush), typeof(IconElement), new FrameworkPropertyMetadata(OnForegroundPropertyChanged));

        private static void OnForegroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        public Brush Foreground
        {
            get
            {
                return (Brush)GetValue(ForegroundProperty);
            }
            set
            {
                SetValue(ForegroundProperty, value);
            }
        }

        #endregion

        #region Methods

        public IconElement()
        {
            rootGrid = new Grid();
            rootGrid.Background = Brushes.Transparent;

            InitializeComponent();

            AddVisualChild(rootGrid);

            Loaded += OnIconElementLoaded;
        }

        protected virtual void InitializeComponent()
        {

        }

        protected void AddChild(UIElement child)
        {
            if(child is TextBlock textBlock && textBlock.GetValue(TextBlock.FontSizeProperty) == null)
            {
                textBlock.SetValue(TextBlock.FontSizeProperty, 12);
            }
            rootGrid.Children.Add(child);
        }

        protected void ClearChild(UIElement child)
        {
            rootGrid.Children.Remove(child);
        }

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index)
        {
            return rootGrid;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            rootGrid.Measure(availableSize);
            return rootGrid.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            rootGrid.Arrange(new Rect(new Point(0, 0), finalSize));
            return base.ArrangeOverride(finalSize);
        }

        private void OnIconElementLoaded(object sender, RoutedEventArgs e)
        {
            if (Foreground == null)
            {
                SetResourceReference(ForegroundProperty, "SystemControlPageTextBaseHighBrush");
            }
        }

        #endregion
    }

    internal class IconElementTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if(sourceType == typeof(string))
            {
                return true;
            }

            return false;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if(destinationType == typeof(SymbolIcon) || destinationType == typeof(FontIcon))
            {
                return true;
            }

            return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string iconValue && Enum.TryParse(iconValue, out Symbol symbol))
            {
                return new SymbolIcon(symbol);
            }
            else if(value is string glyph)
            {
                return new FontIcon()
                {
                    Glyph = glyph
                };
            }

            throw GetConvertFromException(value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
