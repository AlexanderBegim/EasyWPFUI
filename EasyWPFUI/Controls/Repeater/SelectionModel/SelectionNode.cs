// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyWPFUI.Controls
{
    public enum SelectionState
    {
        Selected,
        NotSelected,
        PartiallySelected
    }

    // SelectionNode in the internal tree data structure that we keep track of for selection in 
    // a nested scenario. This would map to one ItemsSourceView/Collection. This node reacts
    // to collection changes and keeps the selected indices up to date.
    // This can either be a leaf node or a non leaf node.
    public class SelectionNode
    {
        public SelectionNode(SelectionModel manager, SelectionNode parent)
        {
            m_manager = manager;
            m_parent = parent;

            m_selectedIndicesCached = new List<int>();
            m_selected = new List<IndexRange>();
            m_childrenNodes = new List<SelectionNode>();
        }

        ~SelectionNode()
        {
            UnhookCollectionChangedHandler();
        }


        public object Source
        {
            get
            {
                return m_source;
            }
            set
            {
                if (m_source != value)
                {
                    ClearSelection();
                    UnhookCollectionChangedHandler();

                    m_source = value;

                    // Setup ItemsSourceView
                    ItemsSourceView newDataSource = value as ItemsSourceView;
                    if (value != null && newDataSource == null)
                    {
                        newDataSource = new InspectingDataSource(value);
                        //newDataSource = new ItemsSourceView();
                    }

                    m_dataSource = newDataSource;

                    HookupCollectionChangedHandler();
                    OnSelectionChanged();
                }
            }
        }

        public ItemsSourceView ItemsSourceView
        {
            get
            {
                return m_dataSource;
            }
        }

        public int DataCount
        {
            get
            {
                return m_dataSource == null ? 0 : m_dataSource.Count;
            }
        }

        public int ChildrenNodeCount
        {
            get
            {
                return m_childrenNodes.Count;
            }
        }

        public int RealizedChildrenNodeCount
        {
            get
            {
                return m_realizedChildrenNodeCount;
            }
        }

        public int AnchorIndex
        {
            get
            {
                return m_anchorIndex;
            }
            set
            {
                m_anchorIndex = value;
            }
        }

        public IndexPath IndexPath
        {
            get
            {
                List<int> path = new List<int>();
                SelectionNode parent = m_parent;
                SelectionNode child = this;
                while (parent != null)
                {
                    List<SelectionNode> childNodes = parent.m_childrenNodes;
                    int index = childNodes.IndexOf(child); // std::find_if(childNodes.cbegin(), childNodes.cend(), [&child](const auto&item) { return item.get() == child; });
                    // assert(index >= 0);
                    // we are walking up to the parent, so the path will be backwards
                    path.Insert(0, index);
                    child = parent;
                    parent = parent.m_parent;
                }

                return new IndexPath(path);
            }
        }

        // For a genuine tree view, we dont know which node is leaf until we 
        // actually walk to it, so currently the tree builds up to the leaf. I don't 
        // create a bunch of leaf node instances - instead i use the same instance m_leafNode to avoid 
        // an explosion of node objects. However, I'm still creating the m_childrenNodes 
        // collection unfortunately.
        public SelectionNode GetAt(int index, bool realizeChild)
        {
            SelectionNode child = null;

            if (realizeChild)
            {
                if (m_childrenNodes.Count == 0)
                {
                    if (m_dataSource != null)
                    {
                        for (int i = 0; i < m_dataSource.Count; i++)
                        {
                            m_childrenNodes.Add(null);
                        }
                    }
                }

                // MUX_ASSERT(0 <= index && index <= static_cast<int>(m_childrenNodes.size()));

                if (m_childrenNodes[index] == null)
                {
                    object childData = m_dataSource.GetAt(index);

                    if (childData != null)
                    {
                        IndexPath childDataIndexPath = IndexPath.CloneWithChildIndex(index);
                        object resolvedChild = m_manager.ResolvePath(childData, childDataIndexPath);

                        if (resolvedChild != null)
                        {
                            child = new SelectionNode(m_manager, this /* parent */);
                            child.Source = resolvedChild;
                        }
                        else
                        {
                            child = m_manager.SharedLeafNode();
                        }
                    }
                    else
                    {
                        child = m_manager.SharedLeafNode();
                    }

                    m_childrenNodes[index] = child;
                    m_realizedChildrenNodeCount++;
                }
                else
                {
                    child = m_childrenNodes[index];
                }
            }
            else
            {
                if(m_childrenNodes.Count > 0)
                {
                    // MUX_ASSERT(0 <= index && index <= static_cast<int>(m_childrenNodes.size()));
                    child = m_childrenNodes[index];
                }
            }

            return child;
        }


        public int SelectedCount
        {
            get
            {
                return m_selectedCount;
            }
        }

        // True  -> Selected
        // False -> Not Selected
        // Null  -> Some descendents are selected and some are not
        public bool? IsSelectedWithPartial()
        {
            bool? isSelected = false;
            if (m_parent != null)
            {
                List<SelectionNode> parentsChildren = m_parent.m_childrenNodes;
                int it = parentsChildren.IndexOf(this);
                if (it >= 0)
                {
                    isSelected = m_parent.IsSelectedWithPartial(it);
                }
            }

            return isSelected;
        }

        // True  -> Selected
        // False -> Not Selected
        // Null  -> Some descendents are selected and some are not
        public bool? IsSelectedWithPartial(int index)
        {
            SelectionState selectionState = SelectionState.NotSelected;

            // MUX_ASSERT(index >= 0);

            if (m_childrenNodes.Count == 0 || // no nodes realized
                m_childrenNodes.Count <= index || // target node is not realized 
                m_childrenNodes[index] == null || // target node is not realized
                m_childrenNodes[index] == m_manager.SharedLeafNode())  // target node is a leaf node.
            {
                // Ask parent if the target node is selected.
                selectionState = IsSelected(index) ? SelectionState.Selected : SelectionState.NotSelected;
            }
            else
            {
                // targetNode is the node representing the index. This node is the parent. 
                // targetNode is a non-leaf node, containing one or many children nodes. Evaluate 
                // based on children of targetNode.
                SelectionNode targetNode = m_childrenNodes[index];
                selectionState = targetNode.EvaluateIsSelectedBasedOnChildrenNodes();
            }

            return ConvertToNullableBool(selectionState);
        }

        public bool IsSelected(int index)
        {
            bool isSelected = false;
            foreach (IndexRange range in m_selected)
            {
                if (range.Contains(index))
                {
                    isSelected = true;
                    break;
                }
            }

            return isSelected;
        }

        public int SelectedIndex()
        {
            return SelectedCount > 0 ? SelectedIndices()[0] : -1;
        }

        public void SelectedIndex(int value)
        {
            if (IsValidIndex(value) && (SelectedCount != 1 || !IsSelected(value)))
            {
                ClearSelection();

                if (value != -1)
                {
                    Select(value, true);
                }
            }
        }

        public List<int> SelectedIndices()
        {
            if (!m_selectedIndicesCacheIsValid)
            {
                m_selectedIndicesCacheIsValid = true;
                foreach (IndexRange range in m_selected)
                {
                    for (int index = range.Begin; index <= range.End; index++)
                    {
                        // Avoid duplicates
                        if (m_selectedIndicesCached.Contains(index) == false)
                        {
                            m_selectedIndicesCached.Add(index);
                        }
                    }
                }

                // Sort the list for easy consumption
                m_selectedIndicesCached.Sort();
            }

            return m_selectedIndicesCached;
        }

        public bool Select(int index, bool select)
        {
            return Select(index, select, true /* raiseOnSelectionChanged */);
        }

        public bool ToggleSelect(int index)
        {
            return Select(index, !IsSelected(index));
        }

        public void SelectAll()
        {
            if (m_dataSource != null)
            {
                int  size = m_dataSource.Count;
                if (size > 0)
                {
                    SelectRange(new IndexRange(0, size - 1), true /* select */);
                }
            }
        }

        public void Clear()
        {
            ClearSelection();
        }

        public bool SelectRange(IndexRange range, bool select)
        {
            if (IsValidIndex(range.Begin) && IsValidIndex(range.End))
            {
                if (select)
                {
                    AddRange(range, true /* raiseOnSelectionChanged */);
                }
                else
                {
                    RemoveRange(range, true /* raiseOnSelectionChanged */);
                }

                return true;
            }

            return false;
        }

        public SelectionState EvaluateIsSelectedBasedOnChildrenNodes()
        {
            SelectionState selectionState = SelectionState.NotSelected;
            int realizedChildrenNodeCount = RealizedChildrenNodeCount;
            int selectedCount = SelectedCount;

            if (realizedChildrenNodeCount != 0 || selectedCount != 0)
            {
                // There are realized children or some selected leaves.
                int dataCount = DataCount;
                if (realizedChildrenNodeCount == 0 && selectedCount > 0)
                {
                    // All nodes are leaves under it - we didn't create children nodes as an optimization.
                    // See if all/some or none of the leaves are selected.
                    selectionState = dataCount != selectedCount ?
                        SelectionState.PartiallySelected :
                        dataCount == selectedCount ? SelectionState.Selected : SelectionState.NotSelected;
                }
                else
                {
                    // There are child nodes, walk them individually and evaluate based on each child
                    // being selected/not selected or partially selected.
                    selectedCount = 0;
                    int notSelectedCount = 0;
                    for (int i = 0; i < ChildrenNodeCount; i++)
                    {
                        if (GetAt(i, false /* realizeChild */) is SelectionNode child)
                        {
                            // child is realized, ask it.
                            bool? isChildSelected = IsSelectedWithPartial(i);
                            if (isChildSelected == null)
                            {
                                selectionState = SelectionState.PartiallySelected;
                                break;
                            }
                            else if (isChildSelected.Value)
                            {
                                selectedCount++;
                            }
                            else
                            {
                                notSelectedCount++;
                            }
                        }
                        else
                        {
                            // not realized.
                            if (IsSelected(i))
                            {
                                selectedCount++;
                            }
                            else
                            {
                                notSelectedCount++;
                            }
                        }

                        if (selectedCount > 0 && notSelectedCount > 0)
                        {
                            selectionState = SelectionState.PartiallySelected;
                            break;
                        }
                    }

                    if (selectionState != SelectionState.PartiallySelected)
                    {
                        if (selectedCount != 0 && selectedCount != dataCount)
                        {
                            selectionState = SelectionState.PartiallySelected;
                        }
                        else
                        {
                            selectionState = selectedCount == dataCount ? SelectionState.Selected : SelectionState.NotSelected;
                        }
                    }
                }
            }

            return selectionState;
        }

        public static bool? ConvertToNullableBool(SelectionState isSelected)
        {
            bool? result = null; // PartialySelected
            if (isSelected == SelectionState.Selected)
            {
                result = true;
            }
            else if (isSelected == SelectionState.NotSelected)
            {
                result = false;
            }

            return result;
        }


        private void HookupCollectionChangedHandler()
        {
            if(m_dataSource != null)
            {
                m_dataSource.CollectionChanged += OnSourceListChanged;
            }
        }

        private void UnhookCollectionChangedHandler()
        {
            if (m_dataSource != null)
            {
                m_dataSource.CollectionChanged -= OnSourceListChanged;
            }
        }

        private bool IsValidIndex(int index)
        {
            return (ItemsSourceView == null || (index >= 0 && index < ItemsSourceView.Count));
        }

        private void AddRange(IndexRange addRange, bool raiseOnSelectionChanged)
        {
            // TODO: Check for duplicates (Task 14107720)
            // TODO: Optimize by merging adjacent ranges (Task 14107720)

            int oldCount = SelectedCount;

            for (int i = addRange.Begin; i <= addRange.End; i++)
            {
                if (!IsSelected(i))
                {
                    m_selectedCount++;
                }
            }

            if (oldCount != m_selectedCount)
            {
                m_selected.Add(addRange);

                if (raiseOnSelectionChanged)
                {
                    OnSelectionChanged();
                }
            }
        }

        private void RemoveRange(IndexRange removeRange, bool raiseOnSelectionChanged)
        {
            int oldCount = m_selectedCount;

            // TODO: Prevent overlap of Ranges in _selected (Task 14107720)
            for (int i = removeRange.Begin; i <= removeRange.End; i++)
            {
                if (IsSelected(i))
                {
                    m_selectedCount--;
                }
            }

            if (oldCount != m_selectedCount)
            {
                // Build up a both a list of Ranges to remove and ranges to add
                List<IndexRange> toRemove = new List<IndexRange>();
                List<IndexRange> toAdd = new List<IndexRange>();

                foreach (IndexRange range in m_selected)
                {
                    // If this range intersects the remove range, we have to do something
                    if (removeRange.Intersects(range))
                    {
                        IndexRange before = new IndexRange(-1, -1);
                        IndexRange cut = new IndexRange(-1, -1);
                        IndexRange after = new IndexRange(-1, -1);

                        // Intersection with the beginning of the range
                        //  Anything to the left of the point (exclusive) stays
                        //  Anything to the right of the point (inclusive) gets clipped
                        if (range.Contains(removeRange.Begin - 1))
                        {
                            range.Split(removeRange.Begin - 1, ref before, ref cut);
                            toAdd.Add(before);
                        }

                        // Intersection with the end of the range
                        //  Anything to the left of the point (inclusive) gets clipped
                        //  Anything to the right of the point (exclusive) stays
                        if (range.Contains(removeRange.End))
                        {
                            if (range.Split(removeRange.End, ref cut, ref after))
                            {
                                toAdd.Add(after);
                            }
                        }

                        // Remove this Range from the collection
                        // New ranges will be added for any remaining subsections
                        toRemove.Add(range);
                    }
                }


                bool change = ((toRemove.Count > 0) || (toAdd.Count > 0));

                if (change)
                {
                    // Remove tagged ranges
                    foreach (IndexRange remove in toRemove)
                    {
                        m_selected.Remove(remove);
                    }

                    // Add new ranges
                    foreach (IndexRange add in toAdd)
                    {
                        m_selected.Add(add);
                    }

                    if (raiseOnSelectionChanged)
                    {
                        OnSelectionChanged();
                    }
                }
            }
        }

        private void ClearSelection()
        {
            // Deselect all items
            if (m_selected.Count > 0)
            {
                m_selected.Clear();
                OnSelectionChanged();
            }

            m_selectedCount = 0;
            AnchorIndex = -1;

            // This will throw away all the children SelectionNodes
            // causing them to be unhooked from their data source. This
            // essentially cleans up the tree.
            m_childrenNodes.Clear();
        }

        private bool Select(int index, bool select, bool raiseOnSelectionChanged)
        {
            if (IsValidIndex(index))
            {
                // Ignore duplicate selection calls
                if (IsSelected(index) == select)
                {
                    return true;
                }

                IndexRange range = new IndexRange(index, index);

                if (select)
                {
                    AddRange(range, raiseOnSelectionChanged);
                }
                else
                {
                    RemoveRange(range, raiseOnSelectionChanged);
                }

                return true;
            }

            return false;
        }

        private void OnSourceListChanged(object dataSource, NotifyCollectionChangedEventArgs args)
        {
            bool selectionInvalidated = false;
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        selectionInvalidated = OnItemsAdded(args.NewStartingIndex, args.NewItems.Count);
                        break;
                    }

                case NotifyCollectionChangedAction.Remove:
                    {
                        selectionInvalidated = OnItemsRemoved(args.OldStartingIndex, args.OldItems.Count);
                        break;
                    }

                case NotifyCollectionChangedAction.Reset:
                    {
                        ClearSelection();
                        selectionInvalidated = true;
                        break;
                    }

                case NotifyCollectionChangedAction.Replace:
                    {
                        selectionInvalidated = OnItemsRemoved(args.OldStartingIndex, args.OldItems.Count);
                        selectionInvalidated |= OnItemsAdded(args.NewStartingIndex, args.NewItems.Count);
                        break;
                    }
            }

            if (selectionInvalidated)
            {
                OnSelectionChanged();
                m_manager.OnSelectionInvalidatedDueToCollectionChange();
            }
        }

        private bool OnItemsAdded(int index, int count)
        {
            bool selectionInvalidated = false;
            // Update ranges for leaf items
            List<IndexRange> toAdd = new List<IndexRange>();

            for (int i = 0; i < m_selected.Count; i++)
            {
                IndexRange range = m_selected[i];

                // The range is after the inserted items, need to shift the range right
                if (range.End >= index)
                {
                    int begin = range.Begin;
                    // If the index left of newIndex is inside the range,
                    // Split the range and remember the left piece to add later
                    if (range.Contains(index - 1))
                    {
                        IndexRange before = new IndexRange(-1, -1);
                        IndexRange after = new IndexRange(-1, -1);
                        range.Split(index - 1, ref before, ref after);
                        toAdd.Add(before);
                        begin = index;
                    }

                    // Shift the range to the right
                    m_selected[i] = new IndexRange(begin + count, range.End + count);
                    selectionInvalidated = true;
                }
            }

            if (toAdd.Count > 0)
            {
                // Add the left sides of the split ranges
                foreach (IndexRange add in toAdd)
                {
                    m_selected.Add(add);
                }
            }

            // Update for non-leaf if we are tracking non-leaf nodes
            if (m_childrenNodes.Count > 0)
            {
                selectionInvalidated = true;
                for (int i = 0; i < count; i++)
                {
                    m_childrenNodes.Insert(index, null);
                }
            }

            //Adjust the anchor
            if (AnchorIndex >= index)
            {
                AnchorIndex = AnchorIndex + count;
            }

            // Check if adding a node invalidated an ancestors
            // selection state. For example if parent was selected before
            // adding a new item makes the parent partially selected now.
            if (!selectionInvalidated)
            {
                SelectionNode parent = m_parent;
                while (parent != null)
                {
                    bool? isSelected = parent.IsSelectedWithPartial();
                    // If a parent is selected, then it will become partially selected.
                    // If it is not selected or partially selected - there is no change.
                    if (isSelected != null && isSelected.Value)
                    {
                        selectionInvalidated = true;
                        break;
                    }

                    parent = parent.m_parent;
                }
            }

            return selectionInvalidated;
        }

        private bool OnItemsRemoved(int index, int count)
        {
            bool selectionInvalidated = false;
            // Remove the items from the selection for leaf
            if (ItemsSourceView.Count > 0)
            {
                bool isSelected = false;
                for (int i = index; i <= index + count - 1; i++)
                {
                    if (IsSelected(i))
                    {
                        isSelected = true;
                        break;
                    }
                }

                if (isSelected)
                {
                    RemoveRange(new IndexRange(index, index + count - 1), false /* raiseOnSelectionChanged */);
                    selectionInvalidated = true;
                }

                for (int i = 0; i < m_selected.Count; i++)
                {
                    IndexRange range = m_selected[i];

                    // The range is after the removed items, need to shift the range left
                    if (range.End > index)
                    {
                        // MUX_ASSERT(!range.Contains(index));

                        // Shift the range to the left
                        m_selected[i] = new IndexRange(range.Begin - count, range.End - count);
                        selectionInvalidated = true;
                    }
                }

                // Update for non-leaf if we are tracking non-leaf nodes
                if (m_childrenNodes.Count > 0)
                {
                    selectionInvalidated = true;
                    for (int i = 0; i < count; i++)
                    {
                        if (m_childrenNodes[index] != null)
                        {
                            m_realizedChildrenNodeCount--;
                        }
                        m_childrenNodes.RemoveAt(index);
                    }
                }

                //Adjust the anchor
                if (AnchorIndex >= index)
                {
                    AnchorIndex = AnchorIndex - count;
                }
            }
            else
            {
                // No more items in the list, clear
                ClearSelection();
                m_realizedChildrenNodeCount = 0;
                selectionInvalidated = true;
            }

            // Check if removing a node invalidated an ancestors
            // selection state. For example if parent was partially selected before
            // removing an item, it could be selected now.
            if (!selectionInvalidated)
            {
                SelectionNode parent = m_parent;
                while (parent != null)
                {
                    bool? isSelected = parent.IsSelectedWithPartial();
                    // If a parent is partially selected, then it will become selected.
                    // If it is selected or not selected - there is no change.
                    if (isSelected == null)
                    {
                        selectionInvalidated = true;
                        break;
                    }

                    parent = parent.m_parent;
                }
            }

            return selectionInvalidated;
        }

        private void OnSelectionChanged()
        {
            m_selectedIndicesCacheIsValid = false;
            m_selectedIndicesCached.Clear();
        }

        private SelectionModel m_manager;

        // Note that a node can contain children who are leaf as well as 
        // chlidren containing leaf entries.

        // For inner nodes (any node whose children are data sources)
        private List<SelectionNode> m_childrenNodes;
        // Don't take a ref.
        private SelectionNode m_parent = null;

        // For parents of leaf nodes (any node whose children are not data sources)
        List<IndexRange> m_selected;

        private object m_source;
        private ItemsSourceView m_dataSource;
        
        // ItemsSourceView::CollectionChanged_revoker m_itemsSourceViewChanged { };

        private int m_selectedCount = 0;
        private List<int> m_selectedIndicesCached;
        private bool m_selectedIndicesCacheIsValid = false;
        private int m_anchorIndex = -1;
        private int m_realizedChildrenNodeCount = 0;
    }
}
