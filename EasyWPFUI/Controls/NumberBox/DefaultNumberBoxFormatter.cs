// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyWPFUI.Controls.Primitives
{
    public class DefaultNumberBoxFormatter : INumberBoxFormatter
    {
        public string Format(double value)
        {
            return value.ToString();
        }

        public double? Parse(string text)
        {
            double result;
            if (double.TryParse(text, out result))
            {
                return result;
            }

            return null;
        }
    }
}
