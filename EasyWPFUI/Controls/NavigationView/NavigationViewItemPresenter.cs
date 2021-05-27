using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using EasyWPFUI.Common;

namespace EasyWPFUI.Controls.Primitives
{
    public class NavigationViewItemPresenter : ContentControl
    {
        #region Icon Property

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(IconElement), typeof(NavigationViewItemPresenter), new FrameworkPropertyMetadata());

        public IconElement Icon
        {
            get
            {
                return (IconElement)GetValue(IconProperty);
            }
            set
            {
                SetValue(IconProperty, value);
            }
        }

        #endregion

        #region TemplateSettings Property

        public static readonly DependencyProperty TemplateSettingsProperty = DependencyProperty.Register("TemplateSettings", typeof(NavigationViewItemPresenterTemplateSettings), typeof(NavigationViewItemPresenter), new FrameworkPropertyMetadata());

        public NavigationViewItemPresenterTemplateSettings TemplateSettings
        {
            get
            {
                return (NavigationViewItemPresenterTemplateSettings)GetValue(TemplateSettingsProperty);
            }
            set
            {
                SetValue(TemplateSettingsProperty, value);
            }
        }

        #endregion

        #region CornerRadius Property

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(NavigationViewItemPresenter), new FrameworkPropertyMetadata());

        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }
            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }

        #endregion

        #region PointerState Property

        public static readonly DependencyPropertyKey PointerStatePropertyKey = DependencyProperty.RegisterReadOnly("PointerState", typeof(string), typeof(NavigationViewItemPresenter), new FrameworkPropertyMetadata());

        public static readonly DependencyProperty PointerStateProperty = PointerStatePropertyKey.DependencyProperty;

        public string PointerState
        {
            get
            {
                return (String)GetValue(PointerStateProperty);
            }
            internal set
            {
                SetValue(PointerStatePropertyKey, value);
            }
        }

        #endregion

        #region IconState Property

        public static readonly DependencyPropertyKey IconStatePropertyKey = DependencyProperty.RegisterReadOnly("IconState", typeof(string), typeof(NavigationViewItemPresenter), new FrameworkPropertyMetadata());

        public static readonly DependencyProperty IconStateProperty = IconStatePropertyKey.DependencyProperty;

        public string IconState
        {
            get
            {
                return (String)GetValue(IconStateProperty);
            }
            internal set
            {
                SetValue(IconStatePropertyKey, value);
            }
        }

        #endregion

        #region ChevronState Property

        public static readonly DependencyPropertyKey ChevronStatePropertyKey = DependencyProperty.RegisterReadOnly("ChevronState", typeof(string), typeof(NavigationViewItemPresenter), new FrameworkPropertyMetadata());

        public static readonly DependencyProperty ChevronStateProperty = ChevronStatePropertyKey.DependencyProperty;

        public string ChevronState
        {
            get
            {
                return (String)GetValue(ChevronStateProperty);
            }
            internal set
            {
                SetValue(ChevronStatePropertyKey, value);
            }
        }

        #endregion

        #region PaneAndTopLevelItemState Property

        public static readonly DependencyPropertyKey PaneAndTopLevelItemStatePropertyKey = DependencyProperty.RegisterReadOnly("PaneAndTopLevelItemState", typeof(string), typeof(NavigationViewItemPresenter), new FrameworkPropertyMetadata());

        public static readonly DependencyProperty PaneAndTopLevelItemStateProperty = PaneAndTopLevelItemStatePropertyKey.DependencyProperty;

        public string PaneAndTopLevelItemState
        {
            get
            {
                return (String)GetValue(PaneAndTopLevelItemStateProperty);
            }
            internal set
            {
                SetValue(PaneAndTopLevelItemStatePropertyKey, value);
            }
        }

        #endregion

        #region NavigationViewIconPositionState Property

        public static readonly DependencyPropertyKey NavigationViewIconPositionStatePropertyKey = DependencyProperty.RegisterReadOnly("NavigationViewIconPositionState", typeof(string), typeof(NavigationViewItemPresenter), new FrameworkPropertyMetadata());

        public static readonly DependencyProperty NavigationViewIconPositionStateProperty = NavigationViewIconPositionStatePropertyKey.DependencyProperty;

        public string NavigationViewIconPositionState
        {
            get
            {
                return (String)GetValue(NavigationViewIconPositionStateProperty);
            }
            internal set
            {
                SetValue(NavigationViewIconPositionStatePropertyKey, value);
            }
        }

        #endregion

        #region Methods

        static NavigationViewItemPresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationViewItemPresenter), new FrameworkPropertyMetadata(typeof(NavigationViewItemPresenter)));
        }

        public NavigationViewItemPresenter()
        {
            TemplateSettings = new NavigationViewItemPresenterTemplateSettings();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Retrieve pointers to stable controls 
            m_helper.Init(this);

            if (GetTemplateChild(c_contentGrid) is Grid contentGrid)
            {
                m_contentGrid = contentGrid;
            }

            if (GetNavigationViewItem() is NavigationViewItem navigationViewItem)
            {
                if (GetTemplateChild(c_expandCollapseChevron) is Grid expandCollapseChevron)
                {
                    m_expandCollapseChevron = expandCollapseChevron;
                    m_expandCollapseChevron.PreviewMouseUp += navigationViewItem.OnExpandCollapseChevronTapped;
                }

                navigationViewItem.UpdateVisualStateNoTransition();
                navigationViewItem.UpdateIsClosedCompact();

                // We probably switched displaymode, so restore width now, otherwise the next time we will restore is when the CompactPaneLength changes
                if (navigationViewItem.GetNavigationView() is NavigationView navigationView)
                {
                    if (navigationView.PaneDisplayMode != NavigationViewPaneDisplayMode.Top)
                    {
                        UpdateCompactPaneLength(m_compactPaneLengthValue, true);
                    }
                }
            }

            if (VisualTreeHelper.GetChildrenCount(this) > 0 && VisualTreeHelper.GetChild(this, 0) is FrameworkElement childRoot)
            {
                m_chevronExpandedStoryboard = childRoot.Resources[c_expandCollapseRotateExpandedStoryboard] as Storyboard;
                m_chevronCollapsedStoryboard = childRoot.Resources[c_expandCollapseRotateCollapsedStoryboard] as Storyboard;
            }

            UpdateMargin();
        }

        public UIElement GetSelectionIndicator()
        {
            return m_helper.GetSelectionIndicator();
        }

        public void UpdateContentLeftIndentation(double leftIndentation)
        {
            m_leftIndentation = leftIndentation;

            UpdateMargin();
        }

        public void RotateExpandCollapseChevron(bool isExpanded)
        {
            if (isExpanded)
            {
                if (m_chevronExpandedStoryboard != null)
                {
                    m_chevronExpandedStoryboard.Begin();
                }
            }
            else
            {
                if (m_chevronCollapsedStoryboard != null)
                {
                    m_chevronCollapsedStoryboard.Begin();
                }
            }
        }

        public void UpdateCompactPaneLength(double compactPaneLength, bool shouldUpdate)
        {
            m_compactPaneLengthValue = compactPaneLength;

            if (shouldUpdate)
            {
                TemplateSettings.IconWidth = compactPaneLength;
                TemplateSettings.SmallerIconWidth = compactPaneLength - 8;
            }
        }

        public void UpdateClosedCompactVisualState(bool isTopLevelItem, bool isClosedCompact)
        {
            // We increased the ContentPresenter margin to align it visually with the expand/collapse chevron. This updated margin is even applied when the
            // NavigationView is in a visual state where no expand/collapse chevrons are shown, leading to more content being cut off than necessary.
            // This is the case for top-level items when the NavigationView is in a compact mode and the NavigationView pane is closed. To keep the original
            // cutoff visual experience intact, we restore  the original ContentPresenter margin for such top-level items only (children shown in a flyout
            // will use the updated margin).
            string stateName = isClosedCompact && isTopLevelItem ? "ClosedCompactAndTopLevelItem" : "NotClosedCompactAndTopLevelItem";

            VisualStateManager.GoToState(this, stateName, false /*useTransitions*/);

            PaneAndTopLevelItemState = stateName;
        }

        private NavigationViewItem GetNavigationViewItem()
        {
            NavigationViewItem navigationViewItem = null;

            DependencyObject obj = this;

            if (SharedHelpers.GetAncestorOfType<NavigationViewItem>(VisualTreeHelper.GetParent(obj)) is NavigationViewItem item)
            {
                navigationViewItem = item;
            }

            return navigationViewItem;
        }

        private void UpdateMargin()
        {
            if (m_contentGrid != null)
            {
                Thickness oldGridMargin = m_contentGrid.Margin;
                m_contentGrid.Margin = new Thickness(m_leftIndentation, oldGridMargin.Top, oldGridMargin.Right, oldGridMargin.Bottom);
            }
        }

        private const string c_contentGrid = "PresenterContentRootGrid";
        private const string c_expandCollapseChevron = "ExpandCollapseChevron";
        private const string c_expandCollapseRotateExpandedStoryboard = "ExpandCollapseRotateExpandedStoryboard";
        private const string c_expandCollapseRotateCollapsedStoryboard = "ExpandCollapseRotateCollapsedStoryboard";

        private const string c_iconBoxColumnDefinitionName = "IconColumn";

        private double m_compactPaneLengthValue = 40;
        NavigationViewItemHelper<NavigationViewItemPresenter> m_helper = new NavigationViewItemHelper<NavigationViewItemPresenter>();
        private Grid m_contentGrid;
        private Grid m_expandCollapseChevron;
        private double m_leftIndentation = 0;
        private Storyboard m_chevronExpandedStoryboard;
        private Storyboard m_chevronCollapsedStoryboard;

        #endregion
    }
}
