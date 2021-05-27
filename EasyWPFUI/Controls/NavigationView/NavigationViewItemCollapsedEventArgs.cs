using System;

namespace EasyWPFUI.Controls
{
    public sealed class NavigationViewItemCollapsedEventArgs : EventArgs
    {
        public object CollapsedItem
        {
            get
            {
                if(m_collapsedItem != null)
                {
                    return m_collapsedItem;
                }

                if(m_navigationView != null)
                {
                    m_collapsedItem = m_navigationView.MenuItemFromContainer(CollapsedItemContainer);
                    return m_collapsedItem;
                }

                return null;
            }
        }

        public NavigationViewItemBase CollapsedItemContainer { get; internal set; }


        internal NavigationViewItemCollapsedEventArgs(NavigationView navigationView)
        {
            m_navigationView = navigationView;
        }


        private NavigationView m_navigationView;
        private object m_collapsedItem;
    }
}
