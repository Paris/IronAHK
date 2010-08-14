﻿using System;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        // TODO: organise TreeView.cs

        /// <summary>
        /// Adds a new item to the TreeView and returns its unique Item ID number (or 0 upon failure). Name is the displayed text of the item, which can be text or numeric (including numeric expression results). ParentItemID is the ID number of the new item's parent (omit it or specify 0 to add the item at the top level). When adding a large number of items, performance can be improved by using GuiControl, -Redraw, MyTreeView before adding the items, and GuiControl, +Redraw, MyTreeView afterward.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ParentItemID"></param>
        /// <param name="Options"></param>
        /// <returns></returns>
        public static int TV_Add(string Name, int ParentItemID, string Options)
        {
            var tree = DefaultTreeView;

            if (tree == null)
                return 0;

            throw new NotImplementedException(); // TODO: TV_Add
        }

        /// <summary>
        /// Removes all or the specified node.
        /// </summary>
        /// <param name="id">The node ID. Leave blank to remove all nodes.</param>
        /// <returns><c>true</c> if an item was removed, <c>false</c> otherwise.</returns>
        public static bool TV_Delete(long id = 0)
        {
            var tree = DefaultTreeView;

            if (tree == null)
                return false;

            if (id == 0)
            {
                tree.Nodes.Clear();
                return true;
            }

            var node = TV_FindNode(tree, id);

            if (node != null)
            {
                node.Remove();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the ID of the specified node's fist child.
        /// </summary>
        /// <param name="id">The parent node ID.</param>
        /// <returns>The ID of the first child or <c>0</c> if none.</returns>
        public static long TV_GetChild(long id)
        {
            var tree = DefaultTreeView;

            if (tree == null)
                return 0;

            var node = TV_FindNode(tree, id);

            if (node == null)
                return 0;

            return node.Nodes.Count == 0 ? 0 : node.FirstNode.Handle.ToInt64();
        }

        /// <summary>
        /// Returns the total number of nodes.
        /// </summary>
        /// <returns>The number of nodes.</returns>
        public static int TV_GetCount()
        {
            var tree = DefaultTreeView;

            if (tree == null)
                return 0;

            return tree.Nodes.Count;
        }

        /// <summary>
        /// Returns the next node with the specified criteria.
        /// </summary>
        /// <param name="id">The starting node ID. Leave blank to search from the first node.</param>
        /// <param name="mode">
        /// <list type="bullet">
        /// <item><term><c>Full</c></term>: <description>the next node irrespective of its relationship to the starting node</description></item>
        /// <item><term><c>Checked</c></term>: <description>the next checked node, implies <c>Full</c></description></item>
        /// <item><term>(blank)</term>: <description>the next sibling node</description></item>
        /// </list>
        /// </param>
        /// <returns>The ID of the first match.</returns>
        public static long TV_GetNext(int id = 0, string mode = null)
        {
            var tree = DefaultTreeView;

            if (tree == null)
                return 0;

        none:
            if (string.IsNullOrEmpty(mode))
            {
                if (id == 0)
                    return tree.Nodes.Count == 0 ? 0 : tree.Nodes[0].Handle.ToInt64();

                var node = TV_FindNode(tree, id);
                return node == null || node.NextNode == null ? 0 : node.NextNode.Handle.ToInt64();
            }
            
            var check = OptionContains(mode, Keyword_Check, Keyword_Checked, Keyword_Checked[0].ToString());
            var full = check || OptionContains(mode, Keyword_Full, Keyword_Full[0].ToString());

            if (!full)
            {
                mode = null;
                goto none;
            }

            for (var i = id == 0 ? 0 : TV_FindNode(tree, id).Index; i < tree.Nodes.Count; i++)
            {
                if (check && !tree.Nodes[i].Checked)
                    continue;

                return tree.Nodes[i].Handle.ToInt64();
            }

            return 0;
        }

        /// <summary>
        /// Returns the ID of the specified node's fist parent.
        /// </summary>
        /// <param name="id">The child node ID.</param>
        /// <returns>The ID of the parent or <c>0</c> if none.</returns>
        public static long TV_GetParent(long id)
        {
            var tree = DefaultTreeView;

            if (tree == null)
                return 0;

            var node = TV_FindNode(tree, id);

            return node == null || node.Parent == null || !(node.Parent is TreeNode) ? 0 : node.Parent.Handle.ToInt64();
        }

        /// <summary>
        /// Returns the ID of the sibling above the specified node.
        /// </summary>
        /// <param name="id">The node ID.</param>
        /// <returns>The ID of the previous node.</returns>
        public static long TV_GetPrev(long id)
        {
            var tree = DefaultTreeView;

            if (tree == null)
                return 0;

            var node = TV_FindNode(tree, id);

            return node.PrevNode == null ? 0 : node.PrevNode.Handle.ToInt64();
        }

        /// <summary>
        /// Returns the selected nodes's ID.
        /// </summary>
        /// <returns>The ID of the currently selected node.</returns>
        public static long TV_GetSelection()
        {
            var tree = DefaultTreeView;

            if (tree == null)
                return 0;

            return tree.SelectedNode == null ? 0 : tree.SelectedNode.Handle.ToInt64();
        }

        /// <summary>
        /// Retrieves the text of the specified node.
        /// </summary>
        /// <param name="result">The variable to store the node text.</param>
        /// <param name="id">The node ID.</param>
        /// <returns>The <paramref name="id"/> if found, <c>0</c> otherwise.</returns>
        public static long TV_GetText(out string result, long id)
        {
            var tree = DefaultTreeView;
            result = null;

            if (tree == null)
                return 0;

            var node = TV_FindNode(tree, id);

            if (node == null)
            {
                result = string.Empty;
                return 0;
            }

            result = node.Text;
            return node.Handle.ToInt64();
        }

        /// <summary>
        /// Modifies the attributes and/or name of an item. It returns the item's own ID upon success or 0 upon failure (or partial failure). When only the first parameter is present, the specified item is selected. When NewName is omitted, the current name is left unchanged. For Options, see the list above.
        /// </summary>
        /// <param name="ItemID"></param>
        /// <param name="Options"></param>
        /// <param name="NewName"></param>
        /// <returns></returns>
        public static int TV_Modify(int ItemID, string Options, string NewName)
        {
            var tree = DefaultTreeView;

            if (tree == null)
                return 0;

            throw new NotImplementedException(); // TODO: TV_Modify
        }
    }
}
