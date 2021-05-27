using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EasyWPFUI.Extensions
{
    public static class ControlExtensions
    {
        public static UIElement ContentTemplateRoot(this ContentControl element)
        {
            return element.Content as UIElement;
        }

        public static GeneralTransform AltTransformToVisual(this Visual parent, Visual element)
        {
            if(parent.FindCommonVisualAncestor(element) != null)
            {
                return parent.TransformToVisual(element);
            }
            else
            {
                return Transform.Identity;
            }
        }
    }
}
