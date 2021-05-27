using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using EasyWPFUI.Common;

namespace EasyWPFUI.Controls
{
    public class NavigationViewItemSeparator : NavigationViewItemBase
    {
        #region NavigationSeparatorLineState Property

        public static readonly DependencyPropertyKey NavigationSeparatorLineStatePropertyKey = DependencyProperty.RegisterReadOnly("NavigationSeparatorLineState", typeof(string), typeof(NavigationViewItemSeparator), new FrameworkPropertyMetadata());

        public static readonly DependencyProperty NavigationSeparatorLineStateProperty = NavigationSeparatorLineStatePropertyKey.DependencyProperty;

        public string NavigationSeparatorLineState
        {
            get
            {
                return (string)GetValue(NavigationSeparatorLineStateProperty);
            }
            internal set
            {
                SetValue(NavigationSeparatorLineStatePropertyKey, value);
            }
        }

        #endregion


        static NavigationViewItemSeparator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationViewItemSeparator), new FrameworkPropertyMetadata(typeof(NavigationViewItemSeparator)));
        }

        public NavigationViewItemSeparator()
        {

        }

        public override void OnApplyTemplate()
        {
            // Stop UpdateVisualState before template is applied. Otherwise the visual may not the same as we expect
            m_appliedTemplate = false;

            base.OnApplyTemplate();

            if (GetTemplateChild(c_rootGrid) is Grid rootGrid)
            {
                m_rootGrid = rootGrid;
            }

            if(GetSplitView() is SplitView splitView)
            {
                splitView.IsPaneOpenChanged += OnSplitViewPropertyChanged;
                splitView.DisplayModeChanged += OnSplitViewPropertyChanged;

                UpdateIsClosedCompact(false);
            }

            m_appliedTemplate = true;
            UpdateVisualState(false /*useTransition*/);
            UpdateItemIndentation();
        }

        protected override void OnNavigationViewItemBasePositionChanged()
        {
            UpdateVisualState(false /*useTransition*/);
        }

        protected override void OnNavigationViewItemBaseDepthChanged()
        {
            UpdateItemIndentation();
        }

        private void OnSplitViewPropertyChanged(DependencyObject sender, DependencyProperty args)
        {
            UpdateIsClosedCompact(true);
        }

        private void UpdateVisualState(bool useTransitions)
        {
            if(m_appliedTemplate)
            {
                string groupName = "NavigationSeparatorLineStates";
                string stateName = Position != NavigationViewRepeaterPosition.TopPrimary && Position != NavigationViewRepeaterPosition.TopFooter ?
                    (m_isClosedCompact ? "HorizontalLineCompact" : "HorizontalLine")
                    : "VerticalLine";

                VisualStateManager.GoToState(this, stateName, false /*useTransitions*/);

                NavigationSeparatorLineState = stateName;
            }
        }

        private void UpdateItemIndentation()
        {
            if(m_rootGrid != null)
            {
                Thickness oldMargin = m_rootGrid.Margin;
                int newLeftMargin = Depth * c_itemIndentation;

                m_rootGrid.Margin = new Thickness(newLeftMargin, oldMargin.Top, oldMargin.Right, oldMargin.Bottom);
            }
        }

        private void UpdateIsClosedCompact(bool updateVisualState)
        {
            if(GetSplitView() is SplitView splitView)
            {
                m_isClosedCompact = !splitView.IsPaneOpen && (splitView.DisplayMode == SplitViewDisplayMode.CompactOverlay || splitView.DisplayMode == SplitViewDisplayMode.CompactInline);

                if (updateVisualState)
                {
                    UpdateVisualState(false /*useTransition*/);
                }
            }
        }


        private const string c_rootGrid = "NavigationViewItemSeparatorRootGrid";

        private bool m_appliedTemplate = false;
        private bool m_isClosedCompact = false;
        private Grid m_rootGrid;
    }
}
