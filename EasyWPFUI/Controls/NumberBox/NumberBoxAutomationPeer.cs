// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using EasyWPFUI.Controls;
using EasyWPFUI.Common;

namespace EasyWPFUI.Automation.Peers
{
    public class NumberBoxAutomationPeer : FrameworkElementAutomationPeer, IRangeValueProvider
    {
        public NumberBoxAutomationPeer(NumberBox owner) : base(owner) { }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.RangeValue)
            {
                return this;
            }

            return base.GetPattern(patternInterface);
        }

        protected override string GetClassNameCore()
        {
            return nameof(NumberBox);
        }

        protected override string GetNameCore()
        {
            string name = base.GetNameCore();

            if (string.IsNullOrEmpty(name))
            {
                NumberBox numberBox = Owner as NumberBox;

                if (numberBox != null && numberBox.Header != null)
                {
                    name = numberBox.Header is string ? numberBox.Header.ToString() : string.Empty;
                }
            }

            return name;
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Spinner;
        }

        private NumberBox GetImpl()
        {
            NumberBox impl = Owner as NumberBox;

            return impl;
        }

        public double Value
        {
            get
            {
                return GetImpl().Value;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public double Maximum
        {
            get
            {
                return GetImpl().Maximum;
            }
        }

        public double Minimum
        {
            get
            {
                return GetImpl().Minimum;
            }
        }

        public double LargeChange
        {
            get
            {
                return GetImpl().LargeChange;
            }
        }

        public double SmallChange
        {
            get
            {
                return GetImpl().SmallChange;
            }
        }

        public void SetValue(double value)
        {
            GetImpl().Value = value;
        }

        public void RaiseValueChangedEvent(double oldValue, double newValue)
        {
            RaisePropertyChangedEvent(RangeValuePatternIdentifiers.ValueProperty, oldValue, newValue);
        }
    }
}
