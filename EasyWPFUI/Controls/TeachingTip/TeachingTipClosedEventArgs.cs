using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyWPFUI.Controls
{
    public class TeachingTipClosedEventArgs : EventArgs
    {
        public TeachingTipCloseReason Reason { get; internal set; }
    }
}
