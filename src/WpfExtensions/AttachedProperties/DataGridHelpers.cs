namespace WpfExtensions.AttachedProperties
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    public static class DataGridHelpers
    {
        public static readonly DependencyProperty UseCustomSortProperty = DependencyProperty.RegisterAttached("UseCustomSort", typeof(bool), typeof(DataGridHelpers), new PropertyMetadata(default(bool), OnUseCustomSortChanged));
        public static readonly DependencyProperty CustomSorterProperty = DependencyProperty.RegisterAttached("CustomSorter", typeof(IComparer), typeof(DataGridHelpers), new PropertyMetadata(default(IComparer)));
        public static readonly DependencyProperty CustomSorterTypeProperty = DependencyProperty.RegisterAttached("CustomSorterType", typeof(Type), typeof(DataGridHelpers), new PropertyMetadata(default(Type), CustomSorterTypePropertyChangedCallback));

        #region Getters and Setters

        public static IComparer GetCustomSorter(DependencyObject element)
        {
            return (IComparer)element.GetValue(CustomSorterProperty);
        }

        public static void SetCustomSorter(DependencyObject element, IComparer value)
        {
            element.SetValue(CustomSorterProperty, value);
        }

        public static bool GetUseCustomSort(DependencyObject element)
        {
            return (bool)element.GetValue(UseCustomSortProperty);
        }

        public static void SetUseCustomSort(DependencyObject element, bool value)
        {
            element.SetValue(UseCustomSortProperty, value);
        }

        public static Type GetCustomSorterType(DependencyObject element)
        {
            return (Type)element.GetValue(CustomSorterTypeProperty);
        }

        public static void SetCustomSorterType(DependencyObject element, Type value)
        {
            element.SetValue(CustomSorterTypeProperty, value);
        }

        #endregion

        /// <summary>
        /// The callback for the <see cref="UseCustomSortProperty"/>.
        /// </summary>
        /// <param name="d">
        /// The dependency object.
        /// </param>
        /// <param name="e">
        /// The event args.
        /// </param>
        private static void OnUseCustomSortChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = d as DataGrid;
            if (dataGrid == null)
            {
                return;
            }

            bool allowSorting = (bool)e.NewValue;

            dataGrid.Sorting -= HandleCustomSorting;

            if (allowSorting)
            {
                dataGrid.Sorting += HandleCustomSorting;
            }
        }

        /// <summary>The callback for the <see cref="CustomSorterTypeProperty"/>.</summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The event args.</param>
        private static void CustomSorterTypePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Type newValue = (Type)e.NewValue;

            var sorter = (IComparer)Activator.CreateInstance(newValue);
            SetCustomSorter(d, sorter);
        }

        private static void HandleCustomSorting(object sender, DataGridSortingEventArgs e)
        {
            var dataGrid = (DataGrid)sender;

            if (!GetUseCustomSort(dataGrid))
            {
                return;
            }

            if (string.IsNullOrEmpty(e.Column.SortMemberPath))
            {
                return;
            }

            var listCollectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
            IComparer sorter = GetCustomSorter(e.Column);

            if (sorter == null)
            {
                return;
            }

            listCollectionView.CustomSort = new ColumnComparer(sorter, e.Column);

            e.Handled = true;
        }

        private class ColumnComparer : IComparer
        {
            private readonly DataGridColumn column;
            private readonly Func<object, object, int> compareMethod;
            private readonly List<PropertyDescriptor> propertyDescriptors;

            public ColumnComparer(IComparer valueComparer, DataGridColumn column)
            {
                this.column = column;

                this.propertyDescriptors = new List<PropertyDescriptor>();

                // switching from DESC to ACS (or initial null)
                if (column.SortDirection != ListSortDirection.Ascending)
                {
                    column.SortDirection = ListSortDirection.Ascending;
                    this.compareMethod = valueComparer.Compare;
                }
                else
                {
                    column.SortDirection = ListSortDirection.Descending;
                    this.compareMethod = (x, y) => valueComparer.Compare(y, x);
                }
            }

            public int Compare(object x, object y)
            {
                if (x == y || string.IsNullOrEmpty(this.column.SortMemberPath))
                {
                    return 0;
                }

                if (x == null)
                {
                    return -1;
                }

                if (y == null)
                {
                    return 1;
                }

                if (this.propertyDescriptors.Count == 0)
                {
                    string sortMemberPath = this.column.SortMemberPath;

                    this.PopulatePropertyDescriptorForNestedSortMemberPath(x, sortMemberPath);

                    if (this.propertyDescriptors.Count == 0)
                    {
                        // Try with other item.
                        this.PopulatePropertyDescriptorForNestedSortMemberPath(y, sortMemberPath);
                    }

                    if (this.propertyDescriptors.Count == 0)
                    {
                        // If still null return anything, will try on next iteration.
                        return -1;
                    }
                }

                object xSortValue = this.GetNestedValue(x);
                object ySortValue = this.GetNestedValue(y);

                return this.compareMethod(xSortValue, ySortValue);
            }

            private static PropertyDescriptor GetPropertyDescriptor(object obj, string propertyName)
            {
                return TypeDescriptor.GetProperties(obj.GetType()).Find(propertyName, false);
            }

            private void PopulatePropertyDescriptorForNestedSortMemberPath(object obj, string propertyName)
            {
                string[] split = propertyName.Split('.');

                foreach (string singlePropertyName in split)
                {
                    if (obj == null)
                    {
                        this.propertyDescriptors.Clear();
                        return;
                    }

                    PropertyDescriptor descriptor = GetPropertyDescriptor(obj, singlePropertyName);
                    this.propertyDescriptors.Add(descriptor);

                    obj = descriptor.GetValue(obj);
                }
            }

            private object GetNestedValue(object obj)
            {
                foreach (PropertyDescriptor descriptor in this.propertyDescriptors)
                {
                    if (obj == null)
                    {
                        return null;
                    }

                    obj = descriptor.GetValue(obj);
                }

                return obj;
            }
        }
    }
}
