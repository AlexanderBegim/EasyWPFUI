using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyWPFUI
{
    public delegate void TypedEventHandler<TSender, TResult>(TSender sender, TResult args);
}
