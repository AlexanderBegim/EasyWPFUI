using System;

namespace EasyWPFUI.Controls
{
    public class NavigationViewItemExpandingEventArgs : EventArgs
    {
        public object ExpandingItem
        {
            get
            {
                if(m_expandingItem != null)
                {
                    return m_expandingItem;
                }

                if(m_navigationView != null)
                {
                    m_expandingItem = m_navigationView.MenuItemFromContainer(ExpandingItemContainer);
                    return m_expandingItem;
                }

                return null;
            }
        }

        public NavigationViewItemBase ExpandingItemContainer { get; internal set; }

        internal NavigationViewItemExpandingEventArgs(NavigationView navigationView)
        {
            m_navigationView = navigationView;
        }

        private NavigationView m_navigationView;
        private object m_expandingItem;
    }
}
