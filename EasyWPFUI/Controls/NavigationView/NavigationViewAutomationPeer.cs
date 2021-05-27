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
    public class NavigationViewAutomationPeer : FrameworkElementAutomationPeer, ISelectionProvider
    {
        public NavigationViewAutomationPeer(NavigationView owner) : base(owner)
        {

        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            return base.GetPattern(patternInterface);
        }

        public bool CanSelectMultiple => throw new NotImplementedException();

        public bool IsSelectionRequired => throw new NotImplementedException();

        public IRawElementProviderSimple[] GetSelection()
        {
            throw new NotImplementedException();
        }

        public void RaiseSelectionChangedEvent(object oldSelection, object newSelecttion)
        {

        }
    }
}
