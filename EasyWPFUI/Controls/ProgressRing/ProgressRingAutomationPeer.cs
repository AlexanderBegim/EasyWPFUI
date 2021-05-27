// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using EasyWPFUI.Controls;

namespace EasyWPFUI.Automation.Peers
{
    public class ProgressRingAutomationPeer : FrameworkElementAutomationPeer
    {
        public ProgressRingAutomationPeer(ProgressRing owner) : base(owner) { }

        protected override string GetClassNameCore()
        {
            return "ProgressRing";
        }

        protected override string GetNameCore()
        {
            string name = base.GetNameCore();

            return name;
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.ProgressBar;
        }
    }
}
