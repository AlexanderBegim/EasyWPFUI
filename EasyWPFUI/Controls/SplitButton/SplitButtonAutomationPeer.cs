// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
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

namespace EasyWPFUI.Automation.Peers
{
    public class SplitButtonAutomationPeer : FrameworkElementAutomationPeer, IExpandCollapseProvider, IInvokeProvider
    {
        public SplitButtonAutomationPeer(SplitButton owner) : base(owner)
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
            return nameof(SplitButton);
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.SplitButton;
        }

        private SplitButton GetImpl()
        {
            SplitButton impl = null;

            if (Owner is SplitButton splitButton)
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

                SplitButton splitButton = GetImpl();

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

        public void Collapse()
        {
            GetImpl()?.CloseFlyout();
        }

        public void Expand()
        {
            GetImpl()?.OpenFlyout();
        }

        // IInvokeProvider
        public void Invoke()
        {
            GetImpl()?.OnClickPrimary(Owner, null);
        }
    }
}
