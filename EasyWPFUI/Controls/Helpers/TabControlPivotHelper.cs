using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasyWPFUI.Controls.Helpers
{
    public static class TabControlPivotHelper
    {
        #region Title Property

        public static readonly DependencyProperty TitleProperty = DependencyProperty.RegisterAttached("Title", typeof(object), typeof(TabControlPivotHelper), new FrameworkPropertyMetadata(null));

        public static object GetTitle(UIElement ui)
        {
            return ui.GetValue(TitleProperty);
        }

        public static void SetTitle(UIElement ui, object value)
        {
            ui.SetValue(TitleProperty, value);
        }

        #endregion

        #region TitleTemplate Property

        public static readonly DependencyProperty TitleTemplateProperty = DependencyProperty.RegisterAttached("TitleTemplate", typeof(DataTemplate), typeof(TabControlPivotHelper), new FrameworkPropertyMetadata(null));

        public static DataTemplate GetTitleTemplate(UIElement ui)
        {
            return (DataTemplate)ui.GetValue(TitleTemplateProperty);
        }

        public static void SetTitleTemplate(UIElement ui, DataTemplate value)
        {
            ui.SetValue(TitleTemplateProperty, value);
        }

        #endregion


        #region LeftHeader Property

        public static readonly DependencyProperty LeftHeaderProperty = DependencyProperty.RegisterAttached("LeftHeader", typeof(object), typeof(TabControlPivotHelper), new FrameworkPropertyMetadata(null));

        public static object GetLeftHeader(UIElement ui)
        {
            return ui.GetValue(LeftHeaderProperty);
        }

        public static void SetLeftHeader(UIElement ui, object value)
        {
            ui.SetValue(LeftHeaderProperty, value);
        }

        #endregion

        #region LeftHeaderTemplate Property

        public static readonly DependencyProperty LeftHeaderTemplateProperty = DependencyProperty.RegisterAttached("LeftHeaderTemplate", typeof(DataTemplate), typeof(TabControlPivotHelper), new FrameworkPropertyMetadata(null));

        public static DataTemplate GetLeftHeaderTemplate(UIElement ui)
        {
            return (DataTemplate)ui.GetValue(LeftHeaderTemplateProperty);
        }

        public static void SetLeftHeaderTemplate(UIElement ui, DataTemplate value)
        {
            ui.SetValue(LeftHeaderTemplateProperty, value);
        }

        #endregion


        #region RightHeader Property

        public static readonly DependencyProperty RightHeaderProperty = DependencyProperty.RegisterAttached("RightHeader", typeof(object), typeof(TabControlPivotHelper), new FrameworkPropertyMetadata(null));

        public static object GetRightHeader(UIElement ui)
        {
            return ui.GetValue(RightHeaderProperty);
        }

        public static void SetRightHeader(UIElement ui, object value)
        {
            ui.SetValue(RightHeaderProperty, value);
        }

        #endregion

        #region LeftHeaderTemplate Property

        public static readonly DependencyProperty RightHeaderTemplateProperty = DependencyProperty.RegisterAttached("RightHeaderTemplate", typeof(DataTemplate), typeof(TabControlPivotHelper), new FrameworkPropertyMetadata(null));

        public static DataTemplate GetRightHeaderTemplate(UIElement ui)
        {
            return (DataTemplate)ui.GetValue(RightHeaderTemplateProperty);
        }

        public static void SetRightHeaderTemplate(UIElement ui, DataTemplate value)
        {
            ui.SetValue(RightHeaderTemplateProperty, value);
        }

        #endregion
    }
}
