using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using EasyWPFUI.Extensions;

namespace EasyWPFUI.Controls.Helpers
{
    public class TreeViewItemHelper
    {
        #region IsHelperEnabled Property

        public static readonly DependencyProperty IsHelperEnabledProperty = DependencyProperty.RegisterAttached("IsHelperEnabled", typeof(bool), typeof(TreeViewItemHelper), new FrameworkPropertyMetadata(false, OnIsHelperEnabledPropertyChanged));

        public static bool GetIsHelperEnabled(UIElement ui)
        {
            return (bool)ui.GetValue(IsHelperEnabledProperty);
        }

        public static void SetIsHelperEnabled(UIElement ui, bool value)
        {
            ui.SetValue(IsHelperEnabledProperty, value);
        }

        private static void OnIsHelperEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItem treeViewItem = d as TreeViewItem;

            if (treeViewItem == null)
                return;

            if ((bool)e.NewValue)
            {
                if (treeViewItem.IsLoaded)
                    UpdateIndentation(treeViewItem);

                treeViewItem.Loaded += OnTreeViewItemLoaded;
            }
            else
            {
                treeViewItem.Loaded -= OnTreeViewItemLoaded;
            }
        }

        private static void OnTreeViewItemLoaded(object sender, RoutedEventArgs e)
        {
            TreeViewItem treeViewItem = (TreeViewItem)sender;

            UpdateIndentation(treeViewItem);
        }

        #endregion

        #region Indentation Property

        private static readonly DependencyPropertyKey IndentationPropertyKey = DependencyProperty.RegisterAttachedReadOnly("Indentation", typeof(Thickness), typeof(TreeViewItemHelper), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty IndentationProperty = IndentationPropertyKey.DependencyProperty;

        public static Thickness GetIndentation(UIElement ui)
        {
            return (Thickness)ui.GetValue(IndentationProperty);
        }

        private static void SetIndentation(UIElement ui, Thickness value)
        {
            ui.SetValue(IndentationPropertyKey, value);
        }

        #endregion

        #region Methods

        private static void UpdateIndentation(TreeViewItem item)
        {
            int depth = item.FindAscendants().TakeWhile(i => !(i is TreeView)).OfType<TreeViewItem>().Count();

            Thickness thickness = new Thickness();
            thickness.Left = depth * 16;
            thickness.Top = 0;
            thickness.Right = 0;
            thickness.Bottom = 0;

            SetIndentation(item, thickness);
        }

        #endregion
    }
}
