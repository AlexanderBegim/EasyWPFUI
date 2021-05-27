using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace EasyWPFUI.Controls.Helpers
{
    public class DataGridHelper
    {
        #region DataGridHelperProperty

        public static readonly DependencyProperty DataGridHelperProperty = DependencyProperty.RegisterAttached("DataGridHelper", typeof(DataGridHelper), typeof(DataGridHelper), new FrameworkPropertyMetadata(null));

        #endregion

        #region IsHelperEnabled Property

        public static readonly DependencyProperty IsHelperEnabledProperty = DependencyProperty.RegisterAttached("IsHelperEnabled", typeof(bool), typeof(DataGridHelper), new FrameworkPropertyMetadata(false, OnIsHelperEnabledPropertyChanged));

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
            DataGrid dataGrid = d as DataGrid;

            if (dataGrid == null)
                return;

            if((bool)e.NewValue)
            {
                DataGridHelper helper = new DataGridHelper(dataGrid);
                helper.Attach();

                dataGrid.SetValue(DataGridHelperProperty, helper);
            }
            else
            {
                DataGridHelper helper = dataGrid.GetValue(DataGridHelperProperty) as DataGridHelper;

                if (helper == null)
                    return;

                helper.Detach();

                dataGrid.ClearValue(DataGridHelperProperty);
            }
        }

        #endregion

        #region SelectionUnit Property

        public static DependencyProperty SelectionUnitProperty = DependencyProperty.RegisterAttached("SelectionUnit", typeof(DataGridSelectionUnit), typeof(DataGridHelper), new FrameworkPropertyMetadata(OnSelectionUnitPropertyChanged));

        public static DataGridSelectionUnit GetSelectionUnit(UIElement ui)
        {
            return (DataGridSelectionUnit)ui.GetValue(SelectionUnitProperty);
        }

        public static void SetSelectionUnit(UIElement ui, DataGridSelectionUnit value)
        {
            ui.SetValue(SelectionUnitProperty, value);
        }

        private static void OnSelectionUnitPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region TextColumnStyle Property

        public static readonly DependencyProperty TextColumnStyleProperty = DependencyProperty.RegisterAttached("TextColumnStyle", typeof(Style), typeof(DataGridHelper));

        public static Style GetTextColumnStyle(UIElement ui)
        {
            return (Style)ui.GetValue(TextColumnStyleProperty);
        }

        public static void SetTextColumnStyle(UIElement ui, Style value)
        {
            ui.SetValue(TextColumnStyleProperty, value);
        }

        #endregion

        #region TextColumnEditingStyle Property

        public static readonly DependencyProperty TextColumnEditingStyleProperty = DependencyProperty.RegisterAttached("TextColumnEditingStyle", typeof(Style), typeof(DataGridHelper));

        public static Style GetTextColumnEditingStyle(UIElement ui)
        {
            return (Style)ui.GetValue(TextColumnEditingStyleProperty);
        }

        public static void SetTextColumnEditingStyle(UIElement ui, Style value)
        {
            ui.SetValue(TextColumnEditingStyleProperty, value);
        }

        #endregion


        #region CheckBoxColumnStyle Property

        public static readonly DependencyProperty CheckBoxColumnStyleProperty = DependencyProperty.RegisterAttached("CheckBoxColumnStyle", typeof(Style), typeof(DataGridHelper));

        public static Style GetCheckBoxColumnStyle(UIElement ui)
        {
            return (Style)ui.GetValue(CheckBoxColumnStyleProperty);
        }

        public static void SetCheckBoxColumnStyle(UIElement ui, Style value)
        {
            ui.SetValue(CheckBoxColumnStyleProperty, value);
        }

        #endregion

        #region CheckBoxColumnEditingStyle Property

        public static readonly DependencyProperty CheckBoxColumnEditingStyleProperty = DependencyProperty.RegisterAttached("CheckBoxColumnEditingStyle", typeof(Style), typeof(DataGridHelper));

        public static Style GetCheckBoxColumnEditingStyle(UIElement ui)
        {
            return (Style)ui.GetValue(CheckBoxColumnEditingStyleProperty);
        }

        public static void SetCheckBoxColumnEditingStyle(UIElement ui, Style value)
        {
            ui.SetValue(CheckBoxColumnEditingStyleProperty, value);
        }

        #endregion


        #region ComboBoxColumnStyle Property

        public static readonly DependencyProperty ComboBoxColumnStyleProperty = DependencyProperty.RegisterAttached("ComboBoxColumnStyle", typeof(Style), typeof(DataGridHelper));

        public static Style GetComboBoxColumnStyle(UIElement ui)
        {
            return (Style)ui.GetValue(ComboBoxColumnStyleProperty);
        }

        public static void SetComboBoxColumnStyle(UIElement ui, Style value)
        {
            ui.SetValue(ComboBoxColumnStyleProperty, value);
        }

        #endregion

        #region ComboBoxColumnEditingStyle Property

        public static readonly DependencyProperty ComboBoxColumnEditingStyleProperty = DependencyProperty.RegisterAttached("ComboBoxColumnEditingStyle", typeof(Style), typeof(DataGridHelper));

        public static Style GetComboBoxColumnEditingStyle(UIElement ui)
        {
            return (Style)ui.GetValue(ComboBoxColumnEditingStyleProperty);
        }

        public static void SetComboBoxColumnEditingStyle(UIElement ui, Style value)
        {
            ui.SetValue(ComboBoxColumnEditingStyleProperty, value);
        }

        #endregion


        #region HyperlinkColumnStyle Property

        public static readonly DependencyProperty HyperlinkColumnStyleProperty = DependencyProperty.RegisterAttached("HyperlinkColumnStyle", typeof(Style), typeof(DataGridHelper));

        public static Style GetHyperlinkColumnStyle(UIElement ui)
        {
            return (Style)ui.GetValue(HyperlinkColumnStyleProperty);
        }

        public static void SetHyperlinkColumnStyle(UIElement ui, Style value)
        {
            ui.SetValue(HyperlinkColumnStyleProperty, value);
        }

        #endregion

        #region HyperlinkColumnEditingStyle Property

        public static readonly DependencyProperty HyperlinkColumnEditingStyleProperty = DependencyProperty.RegisterAttached("HyperlinkColumnEditingStyle", typeof(Style), typeof(DataGridHelper));

        public static Style GetHyperlinkColumnEditingStyle(UIElement ui)
        {
            return (Style)ui.GetValue(HyperlinkColumnEditingStyleProperty);
        }

        public static void SetHyperlinkColumnEditingStyle(UIElement ui, Style value)
        {
            ui.SetValue(HyperlinkColumnEditingStyleProperty, value);
        }

        #endregion


        #region TemplateColumnStyle Property

        public static readonly DependencyProperty TemplateColumnStyleProperty = DependencyProperty.RegisterAttached("TemplateColumnStyle", typeof(Style), typeof(DataGridHelper));

        public static Style GetTemplateColumnStyle(UIElement ui)
        {
            return (Style)ui.GetValue(TemplateColumnStyleProperty);
        }

        public static void SetTemplateColumnStyle(UIElement ui, Style value)
        {
            ui.SetValue(TemplateColumnStyleProperty, value);
        }

        #endregion

        #region TemplateColumnEditingStyle Property

        public static readonly DependencyProperty TemplateColumnEditingStyleProperty = DependencyProperty.RegisterAttached("TemplateColumnEditingStyle", typeof(Style), typeof(DataGridHelper));

        public static Style GetTemplateColumnEditingStyle(UIElement ui)
        {
            return (Style)ui.GetValue(TemplateColumnEditingStyleProperty);
        }

        public static void SetTemplateColumnEditingStyle(UIElement ui, Style value)
        {
            ui.SetValue(TemplateColumnEditingStyleProperty, value);
        }

        #endregion


        #region Fields

        private DataGrid dataGrid;

        #endregion

        #region Methods

        public DataGridHelper(DataGrid dataGrid)
        {
            this.dataGrid = dataGrid;
        }

        public void Attach()
        {
            dataGrid.Columns.CollectionChanged += OnColumnsCollectionChanged;

            SetBindingColumnsStyles(dataGrid.Columns);
        }

        public void Detach()
        {
            dataGrid.Columns.CollectionChanged -= OnColumnsCollectionChanged;

            ClearBindingColumnsStyles(dataGrid.Columns);
        }

        private void OnColumnsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SetBindingColumnsStyles(e.NewItems);
        }

        private void SetBindingColumnsStyles(IList columns)
        {
            foreach (DataGridColumn column in columns)
            {
                if (column is DataGridTextColumn)
                {
                    BindingOperations.SetBinding(column, DataGridBoundColumn.ElementStyleProperty, new Binding() { Source = dataGrid, Path = new PropertyPath(TextColumnStyleProperty) });
                    BindingOperations.SetBinding(column, DataGridBoundColumn.EditingElementStyleProperty, new Binding() { Source = dataGrid, Path = new PropertyPath(TextColumnEditingStyleProperty) });
                }
                else if (column is DataGridCheckBoxColumn)
                {
                    BindingOperations.SetBinding(column, DataGridBoundColumn.ElementStyleProperty, new Binding() { Source = dataGrid, Path = new PropertyPath(CheckBoxColumnStyleProperty) });
                    BindingOperations.SetBinding(column, DataGridBoundColumn.EditingElementStyleProperty, new Binding() { Source = dataGrid, Path = new PropertyPath(CheckBoxColumnEditingStyleProperty) });
                }
                else if (column is DataGridComboBoxColumn)
                {
                    BindingOperations.SetBinding(column, DataGridBoundColumn.ElementStyleProperty, new Binding() { Source = dataGrid, Path = new PropertyPath(ComboBoxColumnStyleProperty) });
                    BindingOperations.SetBinding(column, DataGridBoundColumn.EditingElementStyleProperty, new Binding() { Source = dataGrid, Path = new PropertyPath(ComboBoxColumnEditingStyleProperty) });
                }
                else if (column is DataGridHyperlinkColumn)
                {
                    BindingOperations.SetBinding(column, DataGridBoundColumn.ElementStyleProperty, new Binding() { Source = dataGrid, Path = new PropertyPath(HyperlinkColumnStyleProperty) });
                    BindingOperations.SetBinding(column, DataGridBoundColumn.EditingElementStyleProperty, new Binding() { Source = dataGrid, Path = new PropertyPath(HyperlinkColumnEditingStyleProperty) });
                }
                else if (column is DataGridTemplateColumn)
                {
                    BindingOperations.SetBinding(column, DataGridBoundColumn.ElementStyleProperty, new Binding() { Source = dataGrid, Path = new PropertyPath(TemplateColumnStyleProperty) });
                    BindingOperations.SetBinding(column, DataGridBoundColumn.EditingElementStyleProperty, new Binding() { Source = dataGrid, Path = new PropertyPath(TemplateColumnEditingStyleProperty) });
                }
            }
        }

        private void ClearBindingColumnsStyles(IList columns)
        {
            foreach (DataGridColumn column in columns)
            {
                if (column is DataGridTextColumn)
                {
                    BindingOperations.ClearBinding(dataGrid, TextColumnStyleProperty);
                    BindingOperations.ClearBinding(dataGrid, TextColumnEditingStyleProperty);
                }
                else if (column is DataGridCheckBoxColumn)
                {
                    BindingOperations.ClearBinding(dataGrid, CheckBoxColumnStyleProperty);
                    BindingOperations.ClearBinding(dataGrid, CheckBoxColumnEditingStyleProperty);
                }
                else if (column is DataGridComboBoxColumn)
                {
                    BindingOperations.ClearBinding(dataGrid, ComboBoxColumnStyleProperty);
                    BindingOperations.ClearBinding(dataGrid, ComboBoxColumnEditingStyleProperty);
                }
                else if (column is DataGridHyperlinkColumn)
                {
                    BindingOperations.ClearBinding(dataGrid, HyperlinkColumnStyleProperty);
                    BindingOperations.ClearBinding(dataGrid, HyperlinkColumnEditingStyleProperty);
                }
                else if (column is DataGridTemplateColumn)
                {
                    BindingOperations.ClearBinding(dataGrid, TemplateColumnStyleProperty);
                    BindingOperations.ClearBinding(dataGrid, TemplateColumnEditingStyleProperty);
                }
            }
        }

        #endregion
    }
}
