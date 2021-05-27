using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using EasyWPFUI.Controls;

namespace EasyWPFUI.Automation.Peers
{
    public class AutoSuggestBoxAutomationPeer : FrameworkElementAutomationPeer
    {
        public AutoSuggestBoxAutomationPeer(AutoSuggestBox owner) : base(owner)
        {

        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            return base.GetPattern(patternInterface);
        }

        protected override string GetClassNameCore()
        {
            return nameof(AutoSuggestBox);
        }

        protected override string GetNameCore()
        {
            string name = base.GetNameCore();

            return name;
        }
    }
}
