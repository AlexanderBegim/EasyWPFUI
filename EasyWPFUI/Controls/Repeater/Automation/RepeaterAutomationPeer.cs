// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using System.Windows;

namespace EasyWPFUI.Controls
{
    public class RepeaterAutomationPeer : FrameworkElementAutomationPeer
    {
        public RepeaterAutomationPeer(ItemsRepeater owner) : base(owner)
        {

        }

        protected override List<AutomationPeer> GetChildrenCore()
        {
            ItemsRepeater repeater = Owner as ItemsRepeater;
            List<AutomationPeer> childrenPeers = base.GetChildrenCore();

            if (childrenPeers == null)
                return null;

            int peerCount = childrenPeers.Count;

            List<Tuple<int /* index */, AutomationPeer>> realizedPeers = new List<Tuple<int, AutomationPeer>>();
            realizedPeers.Capacity = peerCount;

            // Filter out unrealized peers.
            {
                for (int i = 0; i < peerCount; ++i)
                {
                    AutomationPeer childPeer = childrenPeers[i];
                    if (GetElement(childPeer, repeater) is UIElement childElement)
                    {
                        VirtualizationInfo virtInfo = ItemsRepeater.GetVirtualizationInfo(childElement);
                        if (virtInfo.IsRealized)
                        {
                            realizedPeers.Add(Tuple.Create(virtInfo.Index, childPeer));
                        }
                    }
                }
            }

            // Sort peers by index.
            realizedPeers.Sort(); //std::sort(realizedPeers.begin(), realizedPeers.end(), [](const auto&lhs, const auto&rhs) { return lhs.first < rhs.first; });

            // Select peers.
            {
                List<AutomationPeer> peers = new List<AutomationPeer>(realizedPeers.Count /* capacity */);
                foreach (Tuple<int, AutomationPeer> entry in realizedPeers)
                {
                    peers.Append(entry.Item2);
                }
                return peers;
            }
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Group;
        }

        private UIElement GetElement(AutomationPeer peer, ItemsRepeater repeater)
        {
            DependencyObject childElement = ((FrameworkElementAutomationPeer)peer).Owner as DependencyObject;

            DependencyObject parent = CachedVisualTreeHelpers.GetParent(childElement);
            // Child peer could have given a descendant of the repeater's child. We
            // want to get to the immediate child.
            while (parent != null && (parent as ItemsRepeater) != repeater)
            {
                childElement = parent;
                parent = CachedVisualTreeHelpers.GetParent(childElement);
            }

            return childElement as UIElement;
        }

    }
}
