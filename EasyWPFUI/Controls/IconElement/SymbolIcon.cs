using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace EasyWPFUI.Controls
{
    public class SymbolIcon : IconElement
    {
        private TextBlock textBlock;

        #region Symbol Property

        public static readonly DependencyProperty SymbolProperty = DependencyProperty.Register("Symbol", typeof(Symbol), typeof(SymbolIcon), new FrameworkPropertyMetadata(OnSymbolPropertyChanged));

        private static void OnSymbolPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SymbolIcon icon = (SymbolIcon)d;

            icon.ChangeIcon();
        }

        public Symbol Symbol
        {
            get
            {
                return (Symbol)GetValue(SymbolProperty);
            }
            set
            {
                SetValue(SymbolProperty, value);
            }
        }

        #endregion

        #region Methods

        public SymbolIcon()
        {
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;
        }

        public SymbolIcon(Symbol symbol) : base()
        {
            Symbol = symbol;
        }

        protected override void InitializeComponent()
        {
            textBlock = new TextBlock();

            textBlock.SetBinding(TextBlock.ForegroundProperty, new Binding() { Source = this, Path = new PropertyPath(ForegroundProperty) });

            textBlock.SetBinding(TextBlock.FontFamilyProperty, new Binding() { Source = this, Path = new PropertyPath(FontIcon.FontFamilyProperty) });
            textBlock.SetBinding(TextBlock.FontSizeProperty, new Binding() { Source = this, Path = new PropertyPath(TextBlock.FontSizeProperty) });
            //textBlock.SetBinding(TextBlock.FontStyleProperty, new Binding() { Source = this, Path = new PropertyPath(FontStyleProperty) });
            //textBlock.SetBinding(TextBlock.FontWeightProperty, new Binding() { Source = this, Path = new PropertyPath(FontWeightProperty) });
            //textBlock.SetBinding(TextBlock.TextProperty, new Binding() { Source = this, Path = new PropertyPath(GlyphProperty) });
            textBlock.SetBinding(TextBlock.VerticalAlignmentProperty, new Binding() { Source = this, Path = new PropertyPath(VerticalAlignmentProperty) });
            textBlock.SetBinding(TextBlock.HorizontalAlignmentProperty, new Binding() { Source = this, Path = new PropertyPath(HorizontalAlignmentProperty) });
            textBlock.SetBinding(TextBlock.SnapsToDevicePixelsProperty, new Binding() { Source = this, Path = new PropertyPath(SnapsToDevicePixelsProperty) });
            textBlock.SetBinding(TextBlock.FocusableProperty, new Binding() { Source = this, Path = new PropertyPath(FocusableProperty) });
            textBlock.SetBinding(TextBlock.FocusVisualStyleProperty, new Binding() { Source = this, Path = new PropertyPath(FocusVisualStyleProperty) });

            textBlock.SetBinding(TextBlock.WidthProperty, new Binding() { Source = this, Path = new PropertyPath(WidthProperty) });
            textBlock.SetBinding(TextBlock.HeightProperty, new Binding() { Source = this, Path = new PropertyPath(HeightProperty) });
            textBlock.SnapsToDevicePixels = true;

            AddChild(textBlock);
        }

        private void ChangeIcon()
        {
            if(textBlock != null)
            {
                textBlock.Text = char.ConvertFromUtf32((int)Symbol);
            }
        }

        #endregion
    }
}
