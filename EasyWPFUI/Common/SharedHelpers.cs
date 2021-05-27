using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Media;
using EasyWPFUI.Controls;

namespace EasyWPFUI.Common
{
    internal static class SharedHelpers
    {
        public static void RaiseAutomationPropertyChangedEvent(UIElement element, object oldValue, object newValue)
        {
            AutomationPeer peer = FrameworkElementAutomationPeer.FromElement(element);

            if (peer != null)
            {
                peer.RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldValue, newValue);
            }
        }

        public static IconElement MakeIconElementFrom(IconSource iconSource)
        {
            if (iconSource is FontIconSource fontIconSource)
            {
                FontIcon fontIcon = new FontIcon();

                fontIcon.Glyph = fontIconSource.Glyph;
                fontIcon.FontSize = fontIconSource.FontSize;

                if (fontIconSource.Foreground != null && fontIconSource.Foreground != DependencyProperty.UnsetValue)
                {
                    fontIcon.Foreground = fontIconSource.Foreground;
                }

                if (fontIconSource.FontFamily != null && fontIconSource.FontFamily != DependencyProperty.UnsetValue)
                {
                    fontIcon.FontFamily = fontIconSource.FontFamily;
                }

                fontIcon.FontWeight = fontIconSource.FontWeight;
                fontIcon.FontStyle = fontIconSource.FontStyle;

                return fontIcon;
            }
            else if (iconSource is PathIconSource pathIconSource)
            {
                PathIcon pathIcon = new PathIcon();

                if (pathIconSource.Data != null)
                {
                    pathIcon.Data = pathIcon.Data;
                }

                if (pathIconSource.Foreground != null && pathIconSource.Foreground != DependencyProperty.UnsetValue)
                {
                    pathIcon.Foreground = pathIconSource.Foreground;
                }

                return pathIcon;
            }
            else if (iconSource is ImageIconSource imageIconSource)
            {
                ImageIcon imageIcon = new ImageIcon();

                if (imageIconSource.ImageSource != null)
                {
                    imageIcon.Source = imageIconSource.ImageSource;
                }

                if (imageIconSource.Foreground != null && imageIconSource.Foreground != DependencyProperty.UnsetValue)
                {
                    imageIcon.Foreground = imageIconSource.Foreground;
                }

                return imageIcon;
            }
            else if (iconSource is BitmapIconSource bitmapIconSource)
            {
                BitmapIcon bitmapIcon = new BitmapIcon();

                if (bitmapIconSource.UriSource != null)
                {
                    bitmapIcon.UriSource = bitmapIconSource.UriSource;
                }

                bitmapIcon.ShowAsMonochrome = bitmapIconSource.ShowAsMonochrome;

                if (bitmapIconSource.Foreground != null && bitmapIconSource.Foreground != DependencyProperty.UnsetValue)
                {
                    bitmapIcon.Foreground = bitmapIconSource.Foreground;
                }

                return bitmapIcon;
            }
            else if(iconSource is SymbolIconSource symbolSource)
            {
                SymbolIcon icon = new SymbolIcon();

                icon.Symbol = symbolSource.Symbol;

                if(symbolSource.Foreground is Brush newForeground)
                {
                    icon.Foreground = newForeground;
                }

                return icon;
            }

            return null;
        }

        public static bool IndexOf(this UIElementCollection collection, UIElement element, out int index)
        {
            int i = collection.IndexOf(element);
            if (i >= 0)
            {
                index = i;
                return true;
            }
            else
            {
                index = 0;
                return false;
            }
        }

        public static T GetAncestorOfType<T>(DependencyObject firstGuess) where T : DependencyObject
        {
            DependencyObject obj = firstGuess;
            T matchedAncestor = null;
            while (obj != null && matchedAncestor == null)
            {
                matchedAncestor = obj as T;
                obj = VisualTreeHelper.GetParent(obj);
            }

            if (matchedAncestor != null)
            {
                return matchedAncestor;
            }
            else
            {
                return null;
            }
        }
    }
}
