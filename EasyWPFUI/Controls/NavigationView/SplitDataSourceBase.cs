// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EasyWPFUI.Controls
{
    // The same copy of .Net Collections like C# ObservableCollection<string> data is splitted into multiple Vectors.
    // For example, the raw data is:  Homes Apps Music | Microsoft Development
    // raw Data SplitDataSource is splitted into 3 ObservableVector which is owned by SplitVector:
    //  A: Home 
    //  B: Apps Music Microsoft Development
    //  C: |
    // A flag vector is used to indicate which Vector the item belongs to and the flag vector is the same length with raw data.
    // so raw data: Homes Apps Music | Microsoft Development
    // flag vector:   A    B    B    C   B         B
    //  We never Add/Delete A,B and C Vector directly, but change the flag.
    //  If flag for Homes is changed from A to B, it asks A to remove it by indexInRawData first, then insert the new data to B vector with indexInRawData
    // SplitVector itself maintained the mapping between indexInRawData and indexInSplitVector.
    public class SplitVector<T, SplitVectorID>
    {
        public SplitVector(SplitVectorID id, Func<T, int> indexOfFunction)
        {
            m_vectorID = id;
            m_indexFunctionFromDataSource = indexOfFunction;

            m_vector = new ObservableCollection<T>();
        }

        public SplitVectorID GetVectorIDForItem()
        {
            return m_vectorID;
        }

        public IList GetVector()
        {
            return m_vector;
        }

        public void OnRawDataRemove(int indexInOriginalVector, SplitVectorID vectorID)
        {
            if (Equals(m_vectorID, vectorID))
            {
                RemoveAt(indexInOriginalVector);
            }

            for (int i = 0; i < m_indexesInOriginalVector.Count; i++)
            {
                int v = m_indexesInOriginalVector[i];

                if (v > indexInOriginalVector)
                {
                    m_indexesInOriginalVector[i]--;
                }
            };
        }

        public void OnRawDataInsert(int preferIndex, int indexInOriginalVector, T value, SplitVectorID vectorID)
        {
            for (int i = 0; i < m_indexesInOriginalVector.Count; i++)
            {
                int v = m_indexesInOriginalVector[i];

                if (v > indexInOriginalVector)
                {
                    m_indexesInOriginalVector[i]++;
                }
            }

            if (Equals(m_vectorID, vectorID))
            {
                InsertAt(preferIndex, indexInOriginalVector, value);
            }
        }

        public void InsertAt(int preferIndex, int indexInOriginalVector, T value)
        {
            m_vector.Insert(preferIndex, value);
            m_indexesInOriginalVector.Insert(preferIndex, indexInOriginalVector);
        }

        public void Replace(int indexInOriginalVector, T value)
        {
            var index = IndexFromIndexInOriginalVector(indexInOriginalVector);
            var vector = m_vector;
            vector.RemoveAt(index);
            vector.Insert(index, value);
        }

        public void Clear()
        {
            m_vector.Clear();
            m_indexesInOriginalVector.Clear();
        }

        public void RemoveAt(int indexInOriginalVector)
        {
            var index = IndexFromIndexInOriginalVector(indexInOriginalVector);

            m_vector.RemoveAt(index);
            m_indexesInOriginalVector.RemoveAt(index);
        }

        public int IndexOf(T value)
        {
            int indexInOriginalVector = m_indexFunctionFromDataSource(value);
            return IndexFromIndexInOriginalVector(indexInOriginalVector);
        }

        public int IndexToIndexInOriginalVector(int index)
        {
            return m_indexesInOriginalVector[index];
        }

        public int IndexFromIndexInOriginalVector(int indexInOriginalVector)
        {
            var pos = m_indexesInOriginalVector.IndexOf(indexInOriginalVector);
            if (pos != -1)
            {
                return pos;
            }
            return -1;
        }

        public int Size()
        {
            return m_indexesInOriginalVector.Count;
        }

        private SplitVectorID m_vectorID;
        private Collection<T> m_vector;
        private List<int> m_indexesInOriginalVector = new List<int>();
        private Func<T, int> m_indexFunctionFromDataSource;
    }

    public class SplitDataSourceBase<T, SplitVectorID, AttachedDataType> where SplitVectorID : Enum
    {
        static int SplitVectorSize = Enum.GetNames(typeof(SplitVectorID)).Length;

        public SplitVectorID GetVectorIDForItem(int index)
        {
            return m_flags[index];
        }

        public AttachedDataType AttachedData(int index)
        {
            return m_attachedData[index];
        }

        public void AttachedData(int index, AttachedDataType attachedData)
        {
            m_attachedData[index] = attachedData;
        }

        public void ResetAttachedData()
        {
            ResetAttachedData(DefaultAttachedData());
        }

        public void ResetAttachedData(AttachedDataType attachedData)
        {
            for (int i = 0; i < RawDataSize(); i++)
            {
                m_attachedData[i] = attachedData;
            }
        }

        public SplitVector<T, SplitVectorID> GetVectorForItem(int index)
        {
            if (index >= 0 && index < RawDataSize())
            {
                return m_splitVectors[Convert.ToInt32(m_flags[index])];
            }
            return null;
        }

        public void MoveItemsToVector(SplitVectorID newVectorID)
        {
            MoveItemsToVector(0, RawDataSize(), newVectorID);
        }

        public void MoveItemsToVector(int start, int end, SplitVectorID newVectorID)
        {
            for (int i = start; i < end; i++)
            {
                MoveItemToVector(i, newVectorID);
            }
        }

        public void MoveItemToVector(int index, SplitVectorID newVectorID)
        {
            if (!Equals(m_flags[index], newVectorID))
            {
                // remove from the old vector
                if (GetVectorForItem(index) is SplitVector<T, SplitVectorID> splitVector)
                {
                    splitVector.RemoveAt(index);
                }

                // change flag
                m_flags[index] = newVectorID;

                // insert item to vector which matches with the newVectorID
                if (m_splitVectors[Convert.ToInt32(newVectorID)] is SplitVector<T, SplitVectorID> toVector)
                {
                    int pos = GetPreferIndex(index, newVectorID);

                    var value = GetAt(index);
                    toVector.InsertAt(pos, index, value);
                }
            }
        }

        public virtual int IndexOf(T value)
        {
            return 0;
        }

        public virtual T GetAt(int index)
        {
            return default;
        }

        public virtual int Size()
        {
            return 0;
        }

        public virtual SplitVectorID DefaultVectorIDOnInsert()
        {
            return default;
        }

        public virtual AttachedDataType DefaultAttachedData()
        {
            return default;
        }

        public int IndexOfImpl(T value, SplitVectorID vectorID)
        {
            int indexInOriginalVector = IndexOf(value);
            int index = -1;
            if (indexInOriginalVector != -1)
            {
                var vector = GetVectorForItem(indexInOriginalVector);
                if (vector != null && Equals(vector.GetVectorIDForItem(), vectorID))
                {
                    index = vector.IndexFromIndexInOriginalVector(indexInOriginalVector);
                }
            }

            return index;
        }

        public void InitializeSplitVectors(List<SplitVector<T, SplitVectorID>> vectors)
        {
            foreach (SplitVector<T, SplitVectorID> vector in vectors)
            {
                m_splitVectors[Convert.ToInt32(vector.GetVectorIDForItem())] = vector;
            }
        }

        public SplitVector<T, SplitVectorID> GetVector(SplitVectorID vectorID)
        {
            return m_splitVectors[Convert.ToInt32(vectorID)];
        }

        public void OnClear()
        {
            // Clear all vectors
            foreach (var vector in m_splitVectors)
            {
                if (vector != null)
                {
                    vector.Clear();
                }
            }

            m_flags.Clear();
            m_attachedData.Clear();
        }

        public void OnRemoveAt(int startIndex, int count)
        {
            for (int i = startIndex + count - 1; i >= startIndex; i--)
            {
                OnRemoveAt(i);
            }
        }

        public void OnInsertAt(int startIndex, int count)
        {
            for (int i = startIndex; i < startIndex + count; i++)
            {
                OnInsertAt(i);
            }
        }

        public int RawDataSize()
        {
            return m_flags.Count;
        }

        public void SyncAndInitVectorFlagsWithID(SplitVectorID defaultID, AttachedDataType defaultAttachedData)
        {
            // Initialize the flags
            for (int i = 0; i < Size(); i++)
            {
                m_flags.Add(defaultID);
                m_attachedData.Add(defaultAttachedData);
            }
        }

        public void Clear()
        {
            OnClear();
        }

        private void OnRemoveAt(int index)
        {
            var vectorID = m_flags[index];

            // Update mapping on all Vectors and Remove Item on vectorID vector;
            foreach (var vector in m_splitVectors)
            {
                if (vector != null)
                {
                    vector.OnRawDataRemove(index, vectorID);
                }
            }

            m_flags.RemoveAt(index);
            m_attachedData.RemoveAt(index);
        }

        private void OnReplace(int index)
        {
            if (GetVectorForItem(index) is SplitVector<T, SplitVectorID> splitVector)
            {
                var value = GetAt(index);
                splitVector.Replace(index, value);
            }
        }

        private void OnInsertAt(int index)
        {
            var vectorID = DefaultVectorIDOnInsert();
            var defaultAttachedData = DefaultAttachedData();
            var preferIndex = GetPreferIndex(index, vectorID);
            var data = GetAt(index);

            // Update mapping on all Vectors and Insert Item on vectorID vector;
            foreach (var vector in m_splitVectors)
            {
                if (vector != null)
                {
                    vector.OnRawDataInsert(preferIndex, index, data, vectorID);
                }
            }

            m_flags.Insert(index, vectorID);
            m_attachedData.Insert(index, defaultAttachedData);
        }

        private int GetPreferIndex(int index, SplitVectorID vectorID)
        {
            return RangeCount(0, index, vectorID);
        }

        private int RangeCount(int start, int end, SplitVectorID vectorID)
        {
            int count = 0;
            for (int i = start; i < end; i++)
            {
                if (Equals(m_flags[i], vectorID))
                {
                    count++;
                }
            }
            return count;
        }

        // length is the same as data source, and used to identify which SplitVector it belongs to.
        private List<SplitVectorID> m_flags = new List<SplitVectorID>();
        private List<AttachedDataType> m_attachedData = new List<AttachedDataType>();
        private SplitVector<T, SplitVectorID>[] m_splitVectors = new SplitVector<T, SplitVectorID>[SplitVectorSize];
    }
}
