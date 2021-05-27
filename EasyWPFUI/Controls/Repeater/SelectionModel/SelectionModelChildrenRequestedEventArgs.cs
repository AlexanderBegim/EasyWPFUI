// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyWPFUI.Controls
{
    public class SelectionModelChildrenRequestedEventArgs : EventArgs
    {
        internal SelectionModelChildrenRequestedEventArgs(object source, IndexPath sourceIndexPath, bool throwOnAccess)
        {
            Initialize(source, sourceIndexPath, throwOnAccess);
        }

        public object Source
        {
            get
            {
                if(m_throwOnAccess)
                {
                    throw new Exception("Source can only be accesed in the ChildrenRequested event handler.");
                }

                return m_source;
            }
        }

        public IndexPath SourceIndex
        {
            get
            {
                if(m_throwOnAccess)
                {
                    throw new Exception("SourceIndex can only be accesed in the ChildrenRequested event handler.");
                }

                return m_sourceIndexPath;
            }
        }

        public object Children { get; set; }

        public void Initialize(object source, IndexPath sourceIndexPath, bool throwOnAccess)
        {
            m_source = source;
            m_sourceIndexPath = sourceIndexPath;
            m_throwOnAccess = throwOnAccess;
        }


        private object m_source;
        private IndexPath m_sourceIndexPath;
        // This flag allows for the re-use of a SelectionModelChildrenRequestedEventArgs object.
        // We do not want someone to cache the args object and access its properties later on, so we use this flag to only allow property access in the ChildrenRequested event handler.
        bool m_throwOnAccess;
    }
}
