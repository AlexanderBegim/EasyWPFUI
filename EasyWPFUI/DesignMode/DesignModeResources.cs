using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;

namespace EasyWPFUI
{
    public class DesignModeResources : ResourceDictionary, ISupportInitialize
    {
        public DesignModeResources()
        {

        }

        public new void BeginInit()
        {
            base.BeginInit();
        }

        public new void EndInit()
        {
            base.EndInit();

            if (!DesignMode.IsInDesignMode)
            {
                Clear();
                MergedDictionaries.Clear();
            }
        }
    }
}
