// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Resources;

namespace ExampleApplication.Data
{
    public class DataGridDataSource
    {
        private static ObservableCollection<DataGridDataItem> _items;
        private static List<string> _mountains;
        private static CollectionViewSource groupedItems;
        private string _cachedSortedColumn = string.Empty;

        // Loading data
        public async Task<IEnumerable<DataGridDataItem>> GetDataAsync()
        {
            Uri sampleUri = new Uri("pack://application:,,,/ExampleApplication;component/Data/mtns.csv");

            StreamResourceInfo resource = Application.GetResourceStream(sampleUri);

            _items = new ObservableCollection<DataGridDataItem>();

            using (StreamReader sr = new StreamReader(resource.Stream))
            {
                while (!sr.EndOfStream)
                {
                    string line = await sr.ReadLineAsync();
                    string[] values = line.Split(',');

                    _items.Add(
                        new DataGridDataItem()
                        {
                            Rank = uint.Parse(values[0]),
                            Mountain = values[1],
                            Height_m = uint.Parse(values[2]),
                            Range = values[3],
                            Coordinates = values[4],
                            Prominence = uint.Parse(values[5]),
                            Parent_mountain = values[6],
                            First_ascent = uint.Parse(values[7]),
                            Ascents = values[8]
                        });
                }
            }

            return _items;
        }

        // Load mountains into separate collection for use in combobox column
        public async Task<IEnumerable<string>> GetMountains()
        {
            if (_items == null || !_items.Any())
            {
                await GetDataAsync();
            }

            _mountains = _items?.OrderBy(x => x.Mountain).Select(x => x.Mountain).Distinct().ToList();

            return _mountains;
        }

        // Sorting implementation using LINQ
        public string CachedSortedColumn
        {
            get
            {
                return _cachedSortedColumn;
            }

            set
            {
                _cachedSortedColumn = value;
            }
        }

        public ObservableCollection<DataGridDataItem> SortData(string sortBy, bool ascending)
        {
            _cachedSortedColumn = sortBy;
            switch (sortBy)
            {
                case "Rank":
                    if (ascending)
                    {
                        return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                          orderby item.Rank ascending
                                                                          select item);
                    }
                    else
                    {
                        return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                          orderby item.Rank descending
                                                                          select item);
                    }

                case "Parent_mountain":
                    if (ascending)
                    {
                        return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                          orderby item.Parent_mountain ascending
                                                                          select item);
                    }
                    else
                    {
                        return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                          orderby item.Parent_mountain descending
                                                                          select item);
                    }

                case "Mountain":
                    if (ascending)
                    {
                        return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                          orderby item.Mountain ascending
                                                                          select item);
                    }
                    else
                    {
                        return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                          orderby item.Mountain descending
                                                                          select item);
                    }

                case "Height_m":
                    if (ascending)
                    {
                        return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                          orderby item.Height_m ascending
                                                                          select item);
                    }
                    else
                    {
                        return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                          orderby item.Height_m descending
                                                                          select item);
                    }

                case "Range":
                    if (ascending)
                    {
                        return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                          orderby item.Range ascending
                                                                          select item);
                    }
                    else
                    {
                        return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                          orderby item.Range descending
                                                                          select item);
                    }
            }

            return _items;
        }

        // Grouping implementation using LINQ
        public CollectionViewSource GroupData()
        {
            ObservableCollection<GroupInfoCollection<DataGridDataItem>> groups = new ObservableCollection<GroupInfoCollection<DataGridDataItem>>();
            var query = from item in _items
                        orderby item
                        group item by item.Range into g
                        select new { GroupName = g.Key, Items = g };
            foreach (var g in query)
            {
                GroupInfoCollection<DataGridDataItem> info = new GroupInfoCollection<DataGridDataItem>();
                info.Key = g.GroupName;
                foreach (var item in g.Items)
                {
                    info.Add(item);
                }

                groups.Add(info);
            }

            groupedItems = new CollectionViewSource();
            //groupedItems.IsSourceGrouped = true;
            groupedItems.Source = groups;

            return groupedItems;
        }

        public class GroupInfoCollection<T> : ObservableCollection<T>
        {
            public object Key { get; set; }

            public new IEnumerator<T> GetEnumerator()
            {
                return (IEnumerator<T>)base.GetEnumerator();
            }
        }

        // Filtering implementation using LINQ
        public enum FilterOptions
        {
            All = -1,
            Rank_Low = 0,
            Rank_High = 1,
            Height_Low = 2,
            Height_High = 3
        }

        public ObservableCollection<DataGridDataItem> FilterData(FilterOptions filterBy)
        {
            switch (filterBy)
            {
                case FilterOptions.All:
                    return new ObservableCollection<DataGridDataItem>(_items);

                case FilterOptions.Rank_Low:
                    return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                      where item.Rank < 50
                                                                      select item);

                case FilterOptions.Rank_High:
                    return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                      where item.Rank > 50
                                                                      select item);

                case FilterOptions.Height_High:
                    return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                      where item.Height_m > 8000
                                                                      select item);

                case FilterOptions.Height_Low:
                    return new ObservableCollection<DataGridDataItem>(from item in _items
                                                                      where item.Height_m < 8000
                                                                      select item);
            }

            return _items;
        }
    }

    public class DataGridDataItem : INotifyDataErrorInfo, IComparable
    {
        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        private uint _rank;
        private string _mountain;
        private uint _height;
        private string _range;
        private string _parentMountain;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public uint Rank
        {
            get
            {
                return _rank;
            }

            set
            {
                if (_rank != value)
                {
                    _rank = value;
                }
            }
        }

        public string Mountain
        {
            get
            {
                return _mountain;
            }

            set
            {
                if (_mountain != value)
                {
                    _mountain = value;

                    bool isMountainValid = !_errors.Keys.Contains("Mountain");
                    if (_mountain == string.Empty && isMountainValid)
                    {
                        List<string> errors = new List<string>();
                        errors.Add("Mountain name cannot be empty");
                        _errors.Add("Mountain", errors);
                        this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("Mountain"));
                    }
                    else if (_mountain != string.Empty && !isMountainValid)
                    {
                        _errors.Remove("Mountain");
                        this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("Mountain"));
                    }
                }
            }
        }

        public uint Height_m
        {
            get
            {
                return _height;
            }

            set
            {
                if (_height != value)
                {
                    _height = value;
                }
            }
        }

        public string Range
        {
            get
            {
                return _range;
            }

            set
            {
                if (_range != value)
                {
                    _range = value;

                    bool isRangeValid = !_errors.Keys.Contains("Range");
                    if (_range == string.Empty && isRangeValid)
                    {
                        List<string> errors = new List<string>();
                        errors.Add("Range name cannot be empty");
                        _errors.Add("Range", errors);
                        this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("Range"));
                    }
                    else if (_range != string.Empty && !isRangeValid)
                    {
                        _errors.Remove("Range");
                        this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("Range"));
                    }
                }
            }
        }

        public string Parent_mountain
        {
            get
            {
                return _parentMountain;
            }

            set
            {
                if (_parentMountain != value)
                {
                    _parentMountain = value;

                    bool isParentValid = !_errors.Keys.Contains("Parent_mountain");
                    if (_parentMountain == string.Empty && isParentValid)
                    {
                        List<string> errors = new List<string>();
                        errors.Add("Parent_mountain name cannot be empty");
                        _errors.Add("Parent_mountain", errors);
                        this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("Parent_mountain"));
                    }
                    else if (_parentMountain != string.Empty && !isParentValid)
                    {
                        _errors.Remove("Parent_mountain");
                        this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("Parent_mountain"));
                    }
                }
            }
        }

        public string Coordinates { get; set; }

        public uint Prominence { get; set; }

        public uint First_ascent { get; set; }

        public string Ascents { get; set; }

        bool INotifyDataErrorInfo.HasErrors
        {
            get
            {
                return _errors.Keys.Count > 0;
            }
        }

        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
        {
            if (propertyName == null)
            {
                propertyName = string.Empty;
            }

            if (_errors.Keys.Contains(propertyName))
            {
                return _errors[propertyName];
            }
            else
            {
                return null;
            }
        }

        int IComparable.CompareTo(object obj)
        {
            int lnCompare = Range.CompareTo((obj as DataGridDataItem).Range);

            if (lnCompare == 0)
            {
                return Parent_mountain.CompareTo((obj as DataGridDataItem).Parent_mountain);
            }
            else
            {
                return lnCompare;
            }
        }
    }
}
