// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EasyWPFUI.Controls
{
    public class RecyclingElementFactory : ElementFactory
    {
        public event EventHandler<SelectTemplateEventArgs> SelectTemplateKey;

        public RecyclePool RecyclePool
        {
            get
            {
                return m_recyclePool;
            }
            set
            {
                m_recyclePool = value;
            }
        }

        public Dictionary<string, DataTemplate> Templates
        {
            get
            {
                return m_templates;
            }
            set
            {
                m_templates = value;
            }
        }


        public RecyclingElementFactory()
        {
            m_templates = new Dictionary<string, DataTemplate>();
        }

        #region IRecyclingElementFactoryOverrides

        protected virtual string OnSelectTemplateKeyCore(object dataContext, UIElement owner)
        {
            if (m_args == null)
            {
                m_args = new SelectTemplateEventArgs();
            }

            SelectTemplateEventArgs args = m_args;
            args.TemplateKey = string.Empty;
            args.DataContext = dataContext;
            args.Owner = owner;

            SelectTemplateKey?.Invoke(this, args);

            string templateKey = args.TemplateKey;
            if (string.IsNullOrEmpty(templateKey))
            {
                throw new Exception("Please provide a valid template identifier in the handler for the SelectTemplateKey event.");
            }

            return templateKey;
        }

        #endregion

        #region IElementFactoryOverrides

        protected override UIElement GetElementCore(ElementFactoryGetArgs args)
        {
            if (m_templates != null || m_templates.Count == 0)
            {
                throw new Exception("Templates property cannot be null or empty.");
            }

            UIElement winrtOwner = args.Parent;
            string templateKey =
                m_templates.Count == 1 ?
                m_templates.First().Key :
                OnSelectTemplateKeyCore(args.Data, winrtOwner);

            if (string.IsNullOrEmpty(templateKey))
            {
                // Note: We could allow null/whitespace, which would work as long as
                // the recycle pool is not shared. in order to make this work in all cases
                // currently we validate that a valid template key is provided.
                throw new Exception("Template key cannot be empty or null.");
            }

            // Get an element from the Recycle Pool or create one
            var element = m_recyclePool.TryGetElement(templateKey, winrtOwner) as FrameworkElement;

            if (element == null)
            {
                // No need to call HasKey if there is only one template.
                if (m_templates.Count > 1 && !m_templates.ContainsKey(templateKey))
                {
                    string message = $"No templates of key {templateKey} were found in the templates collection.";
                    throw new Exception(message);
                }

                DataTemplate dataTemplate = m_templates[templateKey];
                element = dataTemplate.LoadContent() as FrameworkElement;

                // Associate ReuseKey with element
                RecyclePool.SetReuseKey(element, templateKey);
            }

            return element;
        }

        protected override void RecycleElementCore(ElementFactoryRecycleArgs args)
        {
            UIElement element = args.Element;
            string key = RecyclePool.GetReuseKey(element);
            m_recyclePool.PutElement(element, key, args.Parent);
        }

        #endregion

        private RecyclePool m_recyclePool;
        private Dictionary<string, DataTemplate> m_templates;
        private SelectTemplateEventArgs m_args;
    }
}
