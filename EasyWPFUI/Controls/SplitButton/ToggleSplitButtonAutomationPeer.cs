using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using EasyWPFUI.Controls;

namespace EasyWPFUI.Automation.Peers
{
    public class ToggleSplitButtonAutomationPeer :FrameworkElementAutomationPeer, IExpandCollapseProvider, IToggleProvider
    {
        public ToggleSplitButtonAutomationPeer(ToggleSplitButton owner) : base(owner)
        {

        }

        // IAutomationPeerOverrides
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.ExpandCollapse || patternInterface == PatternInterface.Invoke)
            {
                return this;
            }

            return base.GetPattern(patternInterface);
        }

        protected override string GetClassNameCore()
        {
            return nameof(ToggleSplitButton);
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.SplitButton;
        }

        private ToggleSplitButton GetImpl()
        {
            ToggleSplitButton impl = null;

            if (Owner is ToggleSplitButton splitButton)
            {
                impl = splitButton;
            }

            return impl;
        }

        // IExpandCollapseProvider
        public ExpandCollapseState ExpandCollapseState
        {
            get
            {
                ExpandCollapseState currentState = ExpandCollapseState.Collapsed;

                ToggleSplitButton splitButton = GetImpl();

                if (splitButton != null)
                {
                    if (splitButton.IsFlyoutOpen())
                    {
                        currentState = ExpandCollapseState.Expanded;
                    }
                }

                return currentState;
            }
        }

        public void Expand()
        {
            GetImpl()?.OpenFlyout();
        }

        public void Collapse()
        {
            GetImpl()?.CloseFlyout();
        }

        public ToggleState ToggleState
        {
            get
            {
                ToggleState state = ToggleState.Off;

                ToggleSplitButton toggleSplitButton = GetImpl();

                if(toggleSplitButton != null)
                {
                    if(toggleSplitButton.IsChecked)
                    {
                        state = ToggleState.On;
                    }
                }

                return state;
            }
        }

        public void Toggle()
        {
            GetImpl()?.Toggle();
        }
    }
}
