// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyWPFUI.Controls
{
    partial class RepeaterTestHooks
    {
        static void EnsureHooks()
        {
            if (s_testHooks == null)
            {
                s_testHooks = new RepeaterTestHooks();
            }
        }

        public static event EventHandler<object> BuildTreeCompleted
        {
            add
            {
                EnsureHooks();
                s_testHooks.m_buildTreeCompleted += value;
            }
            remove
            {
                if (s_testHooks != null)
                {
                    s_testHooks.m_buildTreeCompleted -= value;
                }
            }
        }

        static void NotifyBuildTreeCompleted()
        {
            if (s_testHooks != null)
            {
                s_testHooks.NotifyBuildTreeCompletedImpl();
            }
        }

        private static RepeaterTestHooks s_testHooks;
    }
}
