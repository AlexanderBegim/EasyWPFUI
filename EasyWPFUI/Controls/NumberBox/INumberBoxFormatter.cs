using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyWPFUI.Controls.Primitives
{
    public interface INumberBoxFormatter
    {
        string Format(double value);

        double? Parse(string text);
    }
}
