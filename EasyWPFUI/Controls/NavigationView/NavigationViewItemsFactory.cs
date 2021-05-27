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
    public class NavigationViewItemsFactory : ElementFactory
    {
        public NavigationViewItemsFactory()
        {

        }

        internal void UserElementFactory(object newValue)
        {
            m_itemTemplateWrapper = newValue as IElementFactoryShim;

            if (m_itemTemplateWrapper == null)
            {
                // ItemTemplate set does not implement IElementFactoryShim. We also 
                // want to support DataTemplate and DataTemplateSelectors automagically.
                if (newValue is DataTemplate dataTemplate)
                {
                    m_itemTemplateWrapper = new ItemTemplateWrapper(dataTemplate);
                }
                else if (newValue is DataTemplateSelector selector)
                {
                    m_itemTemplateWrapper = new ItemTemplateWrapper(selector);
                }
            }

            navigationViewItemPool = new List<NavigationViewItem>();
        }

        internal void SettingsItem(NavigationViewItemBase settingsItem)
        {
            m_settingsItem = settingsItem;
        }

        #region IElementFactoryOverrides

        // Retrieve the element that will be displayed for a specific data item.
        // If the resolved element is not derived from NavigationViewItemBase, wrap in a NavigationViewItem before returning.
        protected override UIElement GetElementCore(ElementFactoryGetArgs args)
        {
            object newContent;

            // Do not template SettingsItem
            if (m_settingsItem == args.Data)
            {
                newContent = args.Data;
            }
            else if (m_itemTemplateWrapper != null)
            {
                return m_itemTemplateWrapper.GetElement(args);
            }
            else
            {
                newContent = args.Data;
            }

            // Element is already of expected type, just return it
            if (newContent is NavigationViewItemBase newItem)
            {
                return newItem;
            }

            NavigationViewItem nviImpl;
            if (navigationViewItemPool.Count > 0)
            {
                nviImpl = navigationViewItemPool.First();
                navigationViewItemPool.RemoveAt(navigationViewItemPool.Count - 1);
            }
            else
            {
                nviImpl = new NavigationViewItem();
            }

            nviImpl.CreatedByNavigationViewItemsFactory = true;

            // If a user provided item template exists, just pass the template and data down to the ContentPresenter of the NavigationViewItem
            if (m_itemTemplateWrapper != null && m_itemTemplateWrapper is ItemTemplateWrapper itemTemplateWrapper)
            {
                // Recycle newContent
                ElementFactoryRecycleArgs tempArgs = new ElementFactoryRecycleArgs();
                tempArgs.Element = newContent as UIElement;
                itemTemplateWrapper.RecycleElement(tempArgs);


                nviImpl.Content = args.Data;
                nviImpl.ContentTemplate = itemTemplateWrapper.Template;
                nviImpl.ContentTemplateSelector = itemTemplateWrapper.TemplateSelector;
                return nviImpl;
            }

            nviImpl.Content = newContent;
            return nviImpl;
        }

        protected override void RecycleElementCore(ElementFactoryRecycleArgs args)
        {
            if(args.Element is UIElement element)
            {
                if(element is NavigationViewItem nviImpl && nviImpl.CreatedByNavigationViewItemsFactory)
                {
                    nviImpl.CreatedByNavigationViewItemsFactory = false;
                    UnlinkElementFromParent(args);
                    args.Element = null;

                    // Retain the NVI that we created for future re-use
                    navigationViewItemPool.Add(nviImpl);

                    // Retrieve the proper element that requires recycling for a user defined item template
                    // and update the args correspondingly
                    if (m_itemTemplateWrapper != null)
                    {
                        // TODO: Retrieve the element and add to the args
                    }
                }

                // Do not recycle SettingsItem
                bool isSettingsItem = m_settingsItem != null && m_settingsItem == args.Element;

                if (m_itemTemplateWrapper != null && !isSettingsItem)
                {
                    m_itemTemplateWrapper.RecycleElement(args);
                }
                else
                {
                    UnlinkElementFromParent(args);
                }
            }
        }

        #endregion

        private void UnlinkElementFromParent(ElementFactoryRecycleArgs args)
        {
            // We want to unlink the containers from the parent repeater
            // in case we are required to move it to a different repeater.
            if (args.Parent is Panel panel)
            {
                UIElementCollection children = panel.Children;
                int childIndex = 0;
                if (children.IndexOf(args.Element, out childIndex))
                {
                    children.RemoveAt(childIndex);
                }
            }
        }


        private IElementFactoryShim m_itemTemplateWrapper = null;
        private NavigationViewItemBase m_settingsItem = null;
        private List<NavigationViewItem> navigationViewItemPool;
    }
}
