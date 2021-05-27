// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyWPFUI.Controls
{
    public class SelectionTreeHelper
    {
        public struct TreeWalkNodeInfo
        {
            public TreeWalkNodeInfo(SelectionNode node, IndexPath indexPath, SelectionNode parent)
            {
                Node = node;
                Path = indexPath;
                ParentNode = parent;
            }

            public TreeWalkNodeInfo(SelectionNode node,IndexPath indexPath)
            {
                Node = node;
                Path = indexPath;
                ParentNode = null;
            }

            public SelectionNode Node { get; set; }
            public IndexPath Path { get; set; }
            public SelectionNode ParentNode { get; set; }
        }


        public static void TraverseIndexPath(SelectionNode root, IndexPath path, bool realizeChildren, Action<SelectionNode, IndexPath, int /*depth*/, int /*childIndex*/> nodeAction)
        {
            SelectionNode node = root;
            for (int depth = 0; depth < path.GetSize(); depth++)
            {
                int childIndex = path.GetAt(depth);
                nodeAction(node, path, depth, childIndex);

                if (depth < path.GetSize() - 1)
                {
                    node = node.GetAt(childIndex, realizeChildren);
                }
            }
        }

        public static void Traverse(SelectionNode root, bool realizeChildren, Action<TreeWalkNodeInfo> nodeAction)
        {
            List<TreeWalkNodeInfo> pendingNodes = new List<TreeWalkNodeInfo>();
            IndexPath current = new IndexPath(null);
            pendingNodes.Add(new TreeWalkNodeInfo(root, current));

            while (pendingNodes.Count > 0)
            {
                TreeWalkNodeInfo nextNode = pendingNodes.Last();
                pendingNodes.Remove(nextNode);
                int count = realizeChildren ? nextNode.Node.DataCount : nextNode.Node.ChildrenNodeCount;
                for (int i = count - 1; i >= 0; i--)
                {
                    SelectionNode child = nextNode.Node.GetAt(i, realizeChildren);
                    IndexPath childPath = nextNode.Path.CloneWithChildIndex(i);
                    if (child != null)
                    {
                        pendingNodes.Add(new TreeWalkNodeInfo(child, childPath, nextNode.Node));
                    }
                }

                // Queue the children first and then perform the action. This way
                // the action can remove the children in the action if necessary
                nodeAction(nextNode);
            }
        }

        public static void TraverseRangeRealizeChildren(SelectionNode root, IndexPath start, IndexPath end, Action<TreeWalkNodeInfo> nodeAction)
        {
            // MUX_ASSERT(start.CompareTo(end) == -1);

            List<TreeWalkNodeInfo> pendingNodes = new List<TreeWalkNodeInfo>();
            IndexPath current = start;

            // Build up the stack to account for the depth first walk up to the 
            // start index path.
            TraverseIndexPath(
                root,
                start,
                true, /* realizeChildren */
                (node, path, depth, childIndex) =>
                {
                    IndexPath currentPath = StartPath(path, depth);
                    bool isStartPath = IsSubSet(start, currentPath);
                    bool isEndPath = IsSubSet(end, currentPath);

                    int startIndex = depth < start.GetSize() && isStartPath ? Math.Max(0, start.GetAt(depth)) : 0;
                    int endIndex = depth < end.GetSize() && isEndPath ? Math.Min(node.DataCount - 1, end.GetAt(depth)) : node.DataCount - 1;

                    for (int i = endIndex; i >= startIndex; i--)
                    {
                        SelectionNode child = node.GetAt(i, true /* realizeChild */);
                        if (child != null)
                        {
                            IndexPath childPath = currentPath.CloneWithChildIndex(i);
                            pendingNodes.Add(new TreeWalkNodeInfo(child, childPath, node));
                        }
                    }
                });

            // From the start index path, do a depth first walk as long as the
            // current path is less than the end path.
            while (pendingNodes.Count > 0)
            {
                TreeWalkNodeInfo info = pendingNodes.Last();
                pendingNodes.Remove(info);
                int depth = info.Path.GetSize();
                bool isStartPath = IsSubSet(start, info.Path);
                bool isEndPath = IsSubSet(end, info.Path);
                int startIndex = depth < start.GetSize() && isStartPath ? start.GetAt(depth) : 0;
                int endIndex = depth < end.GetSize() && isEndPath ? end.GetAt(depth) : info.Node.DataCount - 1;
                for (int i = endIndex; i >= startIndex; i--)
                {
                    SelectionNode child = info.Node.GetAt(i, true /* realizeChild */);
                    if (child != null)
                    {
                        IndexPath childPath = info.Path.CloneWithChildIndex(i);
                        pendingNodes.Add(new TreeWalkNodeInfo(child, childPath, info.Node));
                    }
                }

                nodeAction(info);

                if (info.Path.CompareTo(end) == 0)
                {
                    // We reached the end index path. stop iterating.
                    break;
                }
            }
        }

        private static bool IsSubSet(IndexPath path, IndexPath subset)
        {
            int subsetSize = subset.GetSize();
            if (path.GetSize() < subsetSize)
            {
                return false;
            }

            for (int i = 0; i < subsetSize; i++)
            {
                if (path.GetAt(i) != subset.GetAt(i))
                {
                    return false;
                }
            }

            return true;
        }

        private static IndexPath StartPath(IndexPath path, int length)
        {
            List<int> subPath = new List<int>();

            for (int i = 0; i < length; i++)
            {
                subPath.Add(path.GetAt(i));
            }

            return new IndexPath(subPath);
        }
    }
}
