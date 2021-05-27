using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace EasyWPFUI.Controls
{
    public class SymbolIconSource : IconSource
    {
        #region Symbol Property

        public static readonly DependencyProperty SymbolProperty = DependencyProperty.Register("Symbol", typeof(Symbol), typeof(SymbolIconSource), new FrameworkPropertyMetadata());

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

        public IconElement CreateIconElementCore()
        {
            SymbolIcon symbolicon = new SymbolIcon();
            symbolicon.Symbol = Symbol;

            if(Foreground is Brush newForeground)
            {
                symbolicon.Foreground = newForeground;
            }

            return symbolicon;
        }

        public override DependencyProperty GetIconElementPropertyCore(DependencyProperty sourceProperty)
        {
            if(sourceProperty == SymbolProperty)
            {
                return SymbolIcon.SymbolProperty;
            }

            return base.GetIconElementPropertyCore(sourceProperty);
        }
    }
}
