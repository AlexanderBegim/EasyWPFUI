using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using EasyWPFUI.Controls;
using EasyWPFUI.Common;

namespace EasyWPFUI.Automation.Peers
{
    public class NavigationViewItemAutomationPeer : FrameworkElementAutomationPeer, IInvokeProvider, ISelectionItemProvider, IExpandCollapseProvider
    {
        public bool IsSelected
        {
            get
            {
                if (Owner is NavigationViewItem nvi)
                {
                    return nvi.IsSelected;
                }

                return false;
            }
        }

        public IRawElementProviderSimple SelectionContainer
        {
            get
            {
                if (GetParentNavigationView() is NavigationView navView && FrameworkElementAutomationPeer.CreatePeerForElement(navView) is AutomationPeer peer)
                {
                    return ProviderFromPeer(peer);
                }

                return null;
            }
        }

        public ExpandCollapseState ExpandCollapseState
        {
            get
            {
                ExpandCollapseState state = ExpandCollapseState.LeafNode;
                if (Owner is NavigationViewItem navigationViewItem)
                {
                    state = navigationViewItem.IsExpanded ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;
                }

                return state;
            }
        }

        public NavigationViewItemAutomationPeer(NavigationViewItem owner) : base(owner)
        {

        }

        public void RaiseExpandCollapseAutomationEvent(ExpandCollapseState newState)
        {
            if (AutomationPeer.ListenerExists(AutomationEvents.PropertyChanged))
            {
                ExpandCollapseState oldState = (newState == ExpandCollapseState.Expanded) ?
                    ExpandCollapseState.Collapsed :
                    ExpandCollapseState.Expanded;

                // box_value(oldState) doesn't work here, use ReferenceWithABIRuntimeClassName to make Narrator can unbox it.
                RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldState, newState);
            }
        }

        protected override string GetNameCore()
        {
            string returnString = base.GetNameCore();

            // If a name hasn't been provided by AutomationProperties.Name in markup:
            if (string.IsNullOrEmpty(returnString) && Owner is NavigationViewItem lvi)
            {
                returnString = lvi.Content is string ? lvi.Content.ToString() : string.Empty;
            }

            if (string.IsNullOrEmpty(returnString))
            {
                // NB: It'll be up to the app to determine the automation label for
                // when they're using a PlaceholderValue vs. Value.
                returnString = Properties.Resources.Strings.Resources.NavigationViewItemDefaultControlName;
            }

            return returnString;
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.SelectionItem ||
                // Only provide expand collapse pattern if we have children!
                (patternInterface == PatternInterface.ExpandCollapse && HasChildren()))
            {
                return this;
            }

            return base.GetPattern(patternInterface);
        }

        protected override string GetClassNameCore()
        {
            return nameof(NavigationViewItem);
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            // To be compliant with MAS 4.1.2, in DisplayMode 'Top',
            //  a NavigationViewItem should report itsself as TabItem
            if (IsOnTopNavigation())
            {
                return AutomationControlType.TabItem;
            }
            else
            {
                // TODO: Should this be ListItem in minimal mode and
                // TreeItem otherwise.
                return AutomationControlType.ListItem;
            }
        }

        public void Invoke()
        {
            if (GetParentNavigationView() is NavigationView navView && Owner is NavigationViewItem navigationViewItem)
            {
                if (navigationViewItem == navView.SettingsItem)
                {
                    navView.OnSettingsInvoked();
                }
                else
                {
                    navView.OnNavigationViewItemInvoked(navigationViewItem);
                }
            }
        }

        public void AddToSelection()
        {
            ChangeSelection(true);
        }

        public void RemoveFromSelection()
        {
            ChangeSelection(false);
        }

        public void Select()
        {
            ChangeSelection(true);
        }

        public void Expand()
        {
            if (GetParentNavigationView() is NavigationView navView && Owner is NavigationViewItem navigationViewItem)
            {
                navView.Expand(navigationViewItem);
                RaiseExpandCollapseAutomationEvent(ExpandCollapseState.Expanded);
            }
        }

        public void Collapse()
        {
            if (GetParentNavigationView() is NavigationView navView && Owner is NavigationViewItem navigationViewItem)
            {
                navView.Collapse(navigationViewItem);
                RaiseExpandCollapseAutomationEvent(ExpandCollapseState.Collapsed);
            }
        }

        private void ChangeSelection(bool isSelected)
        {
            if (Owner is NavigationViewItem nvi)
            {
                nvi.IsSelected = isSelected;
            }
        }

        private NavigationView GetParentNavigationView()
        {
            if (Owner is NavigationViewItemBase navigationViewItem)
            {
                return navigationViewItem.GetNavigationView();
            }

            return null;
        }

        private bool HasChildren()
        {
            if (Owner is NavigationViewItem navigationViewItem)
            {
                return navigationViewItem.HasChildren();
            }
            return false;
        }

        private bool IsOnTopNavigation()
        {
            NavigationViewRepeaterPosition position = GetNavigationViewRepeaterPosition();
            return position != NavigationViewRepeaterPosition.LeftNav && position != NavigationViewRepeaterPosition.LeftFooter;
        }

        private bool IsOnFooterNavigation()
        {
            NavigationViewRepeaterPosition position = GetNavigationViewRepeaterPosition();
            return position == NavigationViewRepeaterPosition.LeftFooter || position == NavigationViewRepeaterPosition.TopFooter;
        }

        private NavigationViewRepeaterPosition GetNavigationViewRepeaterPosition()
        {
            if (Owner is NavigationViewItemBase navigationViewItem)
            {
                return navigationViewItem.Position;
            }

            return NavigationViewRepeaterPosition.LeftNav;
        }

        private ItemsRepeater GetParentItemsRepeater()
        {
            if (GetParentNavigationView() is NavigationView navView)
            {
                if (Owner is NavigationViewItemBase navigationViewItem)
                {
                    return navView.GetParentItemsRepeaterForContainer(navigationViewItem);
                }
            }
            return null;
        }
    }
}
