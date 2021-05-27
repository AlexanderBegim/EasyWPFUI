using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;

namespace EasyWPFUI.Controls
{
    public class NavigationViewItemBase : ContentControl
    {
        internal event TypedEventHandler<DependencyObject, DependencyProperty> IsSelectedChanged;

        #region IsSelected Property

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(NavigationView), new FrameworkPropertyMetadata(OnIsSelectedPropertyChanged));

        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }
            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }

        private static void OnIsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NavigationViewItemBase navigationViewItemBase = (NavigationViewItemBase)d;

            navigationViewItemBase.OnNavigationViewItemBaseIsSelectedChanged();
        }

        #endregion

        #region Position Property

        public NavigationViewRepeaterPosition Position
        {
            get
            {
                return m_position;
            }
            set
            {
                if (m_position != value)
                {
                    m_position = value;
                    OnNavigationViewItemBasePositionChanged();
                }
            }
        }

        #endregion

        #region Depth Property

        public int Depth
        {
            get
            {
                return m_depth;
            }
            set
            {
                if (m_depth != value)
                {
                    m_depth = value;
                    OnNavigationViewItemBaseDepthChanged();
                }
            }
        }

        #endregion

        #region IsTopLevelItem Property

        public bool IsTopLevelItem { get; set; }

        #endregion

        #region CreatedByNavigationViewItemsFactory Property

        public bool CreatedByNavigationViewItemsFactory { get; set; }

        #endregion


        #region Methods

        static NavigationViewItemBase()
        {

        }

        internal NavigationViewItemBase()
        {

        }

        public NavigationView GetNavigationView()
        {
            if (m_navigationView != null && m_navigationView.TryGetTarget(out NavigationView target))
            {
                return target;
            }

            return null;
        }

        public SplitView GetSplitView()
        {
            SplitView splitView = null;

            if (GetNavigationView() is NavigationView navigationView)
            {
                splitView = navigationView.GetSplitView();
            }

            return splitView;
        }

        public void SetNavigationViewParent(NavigationView navigationView)
        {
            m_navigationView = new WeakReference<NavigationView>(navigationView);
        }

        protected virtual void OnNavigationViewItemBasePositionChanged()
        {

        }

        protected virtual void OnNavigationViewItemBaseDepthChanged()
        {

        }

        protected virtual void OnNavigationViewItemBaseIsSelectedChanged()
        {

        }

        #endregion

        protected WeakReference<NavigationView> m_navigationView = null;
        // TODO: Constant is a temporary measure. Potentially expose using TemplateSettings.
        protected const int c_itemIndentation = 25;
        private NavigationViewRepeaterPosition m_position = NavigationViewRepeaterPosition.LeftNav;
        private int m_depth = 0;
    }
}
