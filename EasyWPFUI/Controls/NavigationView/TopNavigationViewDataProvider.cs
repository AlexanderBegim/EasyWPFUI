// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using SplitDataSourceT = EasyWPFUI.Controls.SplitDataSourceBase<object, EasyWPFUI.Controls.NavigationViewSplitVectorID, double>;
using SplitVectorT = EasyWPFUI.Controls.SplitVector<object, EasyWPFUI.Controls.NavigationViewSplitVectorID>;

namespace EasyWPFUI.Controls
{
    public enum NavigationViewSplitVectorID
    {
        NotInitialized = 0,
        PrimaryList = 1,
        OverflowList = 2,
        SkippedList = 3,
        Size = 4
    }

    public class TopNavigationViewDataProvider : SplitDataSourceT
    {
        public TopNavigationViewDataProvider()
        {
            Func<object, int> lambda = (object value) =>
            {
                return IndexOf(value);
            };

            SplitVectorT primaryVector = new SplitVectorT(NavigationViewSplitVectorID.PrimaryList, lambda);
            SplitVectorT overflowVector = new SplitVectorT(NavigationViewSplitVectorID.OverflowList, lambda);

            InitializeSplitVectors(new List<SplitVectorT> { primaryVector, overflowVector });
        }

        public IList GetPrimaryItems()
        {
            return GetVector(NavigationViewSplitVectorID.PrimaryList).GetVector();
        }

        public IList GetOverflowItems()
        {
            return GetVector(NavigationViewSplitVectorID.OverflowList).GetVector();
        }

        // The raw data is from MenuItems or MenuItemsSource
        public void SetDataSource(object rawData)
        {
            if (ShouldChangeDataSource(rawData)) // avoid to create multiple of datasource for the same raw data
            {
                ItemsSourceView dataSource = null;
                if (rawData != null)
                {
                    dataSource = new InspectingDataSource(rawData);
                }
                ChangeDataSource(dataSource);
                m_rawDataSource = rawData;
                if (dataSource != null)
                {
                    MoveAllItemsToPrimaryList();
                }
            }
        }

        public bool ShouldChangeDataSource(object rawData)
        {
            return rawData != m_rawDataSource;
        }

        public void OnRawDataChanged(Action<NotifyCollectionChangedEventArgs> dataChangeCallback)
        {
            m_dataChangeCallback = dataChangeCallback;
        }

        // override SplitDataSourceBase
        public override int IndexOf(object value)
        {
            if (m_dataSource is ItemsSourceView dataSource)
            {
                return dataSource.IndexOf(value);
            }
            return -1;
        }

        public override object GetAt(int index)
        {
            if (m_dataSource is ItemsSourceView dataSource)
            {
                return dataSource.GetAt(index);
            }
            return null;
        }

        public override int Size()
        {
            if (m_dataSource is ItemsSourceView dataSource)
            {
                return dataSource.Count;
            }
            return 0;
        }

        public override NavigationViewSplitVectorID DefaultVectorIDOnInsert()
        {
            return NavigationViewSplitVectorID.NotInitialized;
        }

        public override double DefaultAttachedData()
        {
            return double.MinValue;
        }


        public void MoveAllItemsToPrimaryList()
        {
            for (int i = 0; i < Size(); i++)
            {
                MoveItemToVector(i, NavigationViewSplitVectorID.PrimaryList);
            }
        }

        public List<int> ConvertPrimaryIndexToIndex(List<int> indexesInPrimary)
        {
            List<int> indexes = new List<int>();
            if (indexesInPrimary.Count > 0)
            {
                SplitVectorT vector = GetVector(NavigationViewSplitVectorID.PrimaryList);
                if (vector != null)
                {
                    // transform PrimaryList index to OrignalVector index
                    indexes = indexesInPrimary.Select(index =>
                    {
                        return vector.IndexToIndexInOriginalVector(index);
                    }).ToList();
                }
            }
            return indexes;
        }

        public int ConvertOriginalIndexToIndex(int originalIndex)
        {
            SplitVectorT vector = GetVector(IsItemInPrimaryList(originalIndex) ? NavigationViewSplitVectorID.PrimaryList : NavigationViewSplitVectorID.OverflowList);
            return vector.IndexFromIndexInOriginalVector(originalIndex);
        }

        public void MoveItemsOutOfPrimaryList(List<int> indexes)
        {
            MoveItemsToList(indexes, NavigationViewSplitVectorID.OverflowList);
        }

        public void MoveItemsToPrimaryList(List<int> indexes)
        {
            MoveItemsToList(indexes, NavigationViewSplitVectorID.PrimaryList);
        }

        public void MoveItemsToList(List<int> indexes, NavigationViewSplitVectorID vectorID)
        {
            foreach (int index in indexes)
            {
                MoveItemToVector(index, vectorID);
            }
        }


        public int IndexOf(object value, NavigationViewSplitVectorID vectorID)
        {
            return IndexOfImpl(value, vectorID);
        }

        public int GetPrimaryListSize()
        {
            return GetPrimaryItems().Count;
        }

        public int GetNavigationViewItemCountInPrimaryList()
        {
            int count = 0;
            for (int i = 0; i < Size(); i++)
            {
                if (IsItemInPrimaryList(i) && IsContainerNavigationViewItem(i))
                {
                    count++;
                }
            }
            return count;
        }

        public int GetNavigationViewItemCountInTopNav()
        {
            int count = 0;
            for (int i = 0; i < Size(); i++)
            {
                if (IsContainerNavigationViewItem(i))
                {
                    count++;
                }
            }
            return count;
        }

        public void UpdateWidthForPrimaryItem(int indexInPrimary, double width)
        {
            SplitVectorT vector = GetVector(NavigationViewSplitVectorID.PrimaryList);
            if (vector != null)
            {
                int index = vector.IndexToIndexInOriginalVector(indexInPrimary);
                SetWidthForItem(index, width);
            }
        }

        public double WidthRequiredToRecoveryAllItemsToPrimary()
        {
            double width = 0;
            for (int i = 0; i < Size(); i++)
            {
                if (!IsItemInPrimaryList(i))
                {
                    width += GetWidthForItem(i);
                }
            }
            width -= m_overflowButtonCachedWidth;
            return Math.Max(0, width);
        }

        public double CalculateWidthForItems(List<int> items)
        {
            double width = 0;
            foreach (int index in items)
            {
                width += GetWidthForItem(index);
            }
            return width;
        }

        public double GetWidthForItem(int index)
        {
            double width = AttachedData(index);
            if (!IsValidWidth(width))
            {
                width = 0;
            }
            return width;
        }

        public void InvalidWidthCache()
        {
            ResetAttachedData(-1.0);
        }

        public double OverflowButtonWidth()
        {
            return m_overflowButtonCachedWidth;
        }

        public void OverflowButtonWidth(double width)
        {
            m_overflowButtonCachedWidth = width;
        }

        public bool IsItemInPrimaryList(int index)
        {
            return GetVectorIDForItem(index) == NavigationViewSplitVectorID.PrimaryList;
        }

        public bool HasInvalidWidth(List<int> items)
        {
            bool hasInvalidWidth = false;
            foreach (int index in items)
            {
                if (!IsValidWidthForItem(index))
                {
                    hasInvalidWidth = true;
                    break;
                }
            }
            return hasInvalidWidth;
        }

        public bool IsValidWidthForItem(int index)
        {
            double width = AttachedData(index);
            return IsValidWidth(width);
        }

        // If value is not in the raw data set or can't be move to primarylist, then return false
        public bool IsItemSelectableInPrimaryList(object value)
        {
            int index = IndexOf(value);
            return (index != -1);
        }

        protected void OnDataSourceChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        OnInsertAt(args.NewStartingIndex, args.NewItems.Count);
                        break;
                    }

                case NotifyCollectionChangedAction.Remove:
                    {
                        OnRemoveAt(args.OldStartingIndex, args.OldItems.Count);
                        break;
                    }

                case NotifyCollectionChangedAction.Reset:
                    {
                        OnClear();
                        break;
                    }

                case NotifyCollectionChangedAction.Replace:
                    {
                        OnRemoveAt(args.OldStartingIndex, args.OldItems.Count);
                        OnInsertAt(args.NewStartingIndex, args.NewItems.Count);
                        break;
                    }
            }
            if (m_dataChangeCallback != null)
            {
                m_dataChangeCallback(args);
            }
        }


        private bool IsValidWidth(double width)
        {
            return (width >= 0) && (width < double.MaxValue);
        }

        private void SetWidthForItem(int index, double width)
        {
            if (IsValidWidth(width))
            {
                AttachedData(index, width);
            }
        }

        private void ChangeDataSource(ItemsSourceView newValue)
        {
            ItemsSourceView oldValue = m_dataSource;
            if (oldValue != newValue)
            {
                // update to the new datasource.

                if (oldValue != null)
                {
                    oldValue.CollectionChanged -= OnDataSourceChanged;
                }

                Clear();

                m_dataSource = newValue;
                SyncAndInitVectorFlagsWithID(NavigationViewSplitVectorID.NotInitialized, DefaultAttachedData());

                if (newValue != null)
                {
                    newValue.CollectionChanged += OnDataSourceChanged;
                }
            }

            // Move all to primary list
            MoveItemsToVector(NavigationViewSplitVectorID.NotInitialized);
        }

        private bool IsContainerNavigationViewItem(int index)
        {
            bool isContainerNavigationViewItem = true;

            object item = GetAt(index);
            if (item != null && (item is NavigationViewItemHeader || item is NavigationViewItemSeparator))
            {
                isContainerNavigationViewItem = false;
            }
            return isContainerNavigationViewItem;
        }

        private bool IsContainerNavigationViewHeader(int index)
        {
            bool isContainerNavigationViewHeader = false;

            object item = GetAt(index);
            if (item != null && item is NavigationViewItemHeader)
            {
                isContainerNavigationViewHeader = true;
            }
            return isContainerNavigationViewHeader;
        }


        private ItemsSourceView m_dataSource;
        // If the raw datasource is the same, we don't need to create new winrt::ItemsSourceView object.
        private object m_rawDataSource;
        // Event tokens
        //winrt::event_token m_dataSourceChanged { };
        private Action<NotifyCollectionChangedEventArgs> m_dataChangeCallback;

        private double m_overflowButtonCachedWidth;
    }
}
