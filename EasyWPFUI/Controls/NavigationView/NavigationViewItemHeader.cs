using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace EasyWPFUI.Controls
{
    public class NavigationViewItemHeader : NavigationViewItemBase
    {
        static NavigationViewItemHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationViewItemHeader), new FrameworkPropertyMetadata(typeof(NavigationViewItemHeader)));
        }

        public NavigationViewItemHeader()
        {

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild(c_rootGrid) is Grid rootGrid)
            {
                m_rootGrid = rootGrid;
            }

            if (GetSplitView() is SplitView splitView)
            {
                splitView.IsPaneOpenChanged += OnSplitViewPropertyChanged;
                splitView.DisplayModeChanged += OnSplitViewPropertyChanged;

                UpdateIsClosedCompact();
            }

            UpdateVisualState(false /*useTransitions*/);
            UpdateItemIndentation();

            // var visual = ElementCompositionPreview.GetElementVisual(this);
            // NavigationView.CreateAndAttachHeaderAnimation(visual);
        }

        protected override void OnNavigationViewItemBaseDepthChanged()
        {
            UpdateItemIndentation();
        }

        private void OnSplitViewPropertyChanged(DependencyObject sender, DependencyProperty args)
        {
            UpdateIsClosedCompact();
        }

        private void UpdateIsClosedCompact()
        {
            if (GetSplitView() is SplitView splitView)
            {
                // Check if the pane is closed and if the splitview is in either compact mode.
                m_isClosedCompact = !splitView.IsPaneOpen && (splitView.DisplayMode == SplitViewDisplayMode.CompactOverlay || splitView.DisplayMode == SplitViewDisplayMode.CompactInline);
                UpdateVisualState(true /*useTransitions*/);
            }
        }

        private void UpdateItemIndentation()
        {
            // Update item indentation based on its depth
            if (m_rootGrid != null)
            {
                Thickness oldMargin = m_rootGrid.Margin;
                int newLeftMargin = Depth * c_itemIndentation;
                m_rootGrid.Margin = new Thickness(newLeftMargin, oldMargin.Top, oldMargin.Right, oldMargin.Bottom);
            }
        }

        private void UpdateVisualState(bool useTransitions)
        {
            VisualStateManager.GoToState(this, m_isClosedCompact && IsTopLevelItem ? "HeaderTextCollapsed" : "HeaderTextVisible", useTransitions);
        }

        private const string c_rootGrid = "NavigationViewItemHeaderRootGrid";

        private bool m_isClosedCompact = false;
        private Grid m_rootGrid;
    }
}
