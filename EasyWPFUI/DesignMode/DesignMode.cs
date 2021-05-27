using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;

namespace EasyWPFUI
{
    public static class DesignMode
    {
        private static DependencyObject obj;

        public static bool IsInDesignMode
        {
            get
            {
                if(obj == null)
                {
                    obj = new DependencyObject();
                }

                return DesignerProperties.GetIsInDesignMode(obj);
            }
        }

    }
}
