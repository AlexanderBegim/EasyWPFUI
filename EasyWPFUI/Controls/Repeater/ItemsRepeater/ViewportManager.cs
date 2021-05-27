using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EasyWPFUI.Controls
{
    public abstract class ViewportManager
    {
        public abstract UIElement SuggestedAnchor { get; }

        public abstract double HorizontalCacheLength { get; set; }

        public abstract double VerticalCacheLength { get; set; }

        public abstract Rect GetLayoutVisibleWindow();

        public abstract Rect GetLayoutRealizationWindow();

        public abstract void SetLayoutExtent(Rect extent);
        public abstract Point GetOrigin();

        public abstract void OnLayoutChanged(bool isabstractizing);
        public abstract void OnElementPrepared(UIElement element);
        public abstract void OnElementCleared(UIElement element);
        public abstract void OnOwnerMeasuring();
        public abstract void OnOwnerArranged();
        public abstract void OnMakeAnchor(UIElement anchor, bool isAnchorOutsideRealizedRange);

        public abstract void ResetScrollers();

        public abstract UIElement MadeAnchor { get; }
    }
}
