using System;

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
        /// Returns the total number of items in the control. This function is always instantaneous because the control keeps track of the count.
        /// </summary>
        /// <returns></returns>
        public static int TV_GetCount()
        {
            var tree = DefaultTreeView;

            if (tree == null)
                return 0;

            throw new NotImplementedException(); // TODO: TV_GetCount
        }

        /// <summary>
        /// This has the following modes:
        /// </summary>
        /// <param name="ItemID"></param>
        /// <param name="Mode"></param>
        /// <returns></returns>
        public static int TV_GetNext(int ItemID, string Mode)
        {
            var tree = DefaultTreeView;

            if (tree == null)
                return 0;

            throw new NotImplementedException(); // TODO: TV_GetNext
        }

        /// <summary>
        /// Returns the specified item's parent as an item ID. Items at the top level have no parent and thus return 0.
        /// </summary>
        /// <param name="ItemID"></param>
        /// <returns></returns>
        public static int TV_GetParent(int ItemID)
        {
            var tree = DefaultTreeView;

            if (tree == null)
                return 0;

            throw new NotImplementedException(); // TODO: TV_GetParent
        }

        /// <summary>
        /// Returns the ID number of the sibling above the specified item (or 0 if none).
        /// </summary>
        /// <param name="ItemID"></param>
        /// <returns></returns>
        public static int TV_GetPrev(int ItemID)
        {
            var tree = DefaultTreeView;

            if (tree == null)
                return 0;

            throw new NotImplementedException(); // TODO: TV_GetPrev
        }

        /// <summary>
        /// Returns the selected item's ID number.
        /// </summary>
        /// <returns></returns>
        public static int TV_GetSelection()
        {
            var tree = DefaultTreeView;

            if (tree == null)
                return 0;

            throw new NotImplementedException(); // TODO: TV_GetSelection
        }

        /// <summary>
        /// Retrieves the text/name of the specified ItemID and stores it in OutputVar. If the text is longer than 8191, only the first 8191 characters are retrieved. Upon success, the function returns the item's own ID. Upon failure, it returns 0 (and OutputVar is also made blank).
        /// </summary>
        /// <param name="OutputVar"></param>
        /// <param name="ItemID"></param>
        /// <returns></returns>
        public static int TV_GetText(out string OutputVar, int ItemID)
        {
            var tree = DefaultTreeView;
            OutputVar = null;

            if (tree == null)
                return 0;

            throw new NotImplementedException(); // TODO: TV_GetText
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
