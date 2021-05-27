// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using EasyWPFUI.Controls;

namespace EasyWPFUI.Automation.Peers
{
    public class TeachingTipAutomationPeer : FrameworkElementAutomationPeer
    {
        private TeachingTip teachingTip;

        public TeachingTipAutomationPeer(TeachingTip owner) : base(owner)
        {
            teachingTip = (TeachingTip)Owner;
        }

        protected override string GetClassNameCore()
        {
            return nameof(TeachingTip);
        }
    }
}
