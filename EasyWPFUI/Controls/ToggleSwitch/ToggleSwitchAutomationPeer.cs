using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using EasyWPFUI.Controls;
using EasyWPFUI.Common;

namespace EasyWPFUI.Automation.Peers
{
    public class ToggleSwitchAutomationPeer : FrameworkElementAutomationPeer
    {
        public ToggleSwitchAutomationPeer(ToggleSwitch owner) : base(owner) { }

        protected override string GetClassNameCore()
        {
            return nameof(ToggleSwitch);
        }

        protected override string GetNameCore()
        {
            string name = base.GetNameCore();

            ToggleSwitch toggleSwitch = Owner as ToggleSwitch;

            if (toggleSwitch != null && toggleSwitch.Header != null)
            {
                name = toggleSwitch.Header is string ? toggleSwitch.Header.ToString() : string.Empty;
            }

            return name;
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.CheckBox;
        }
    }
}
