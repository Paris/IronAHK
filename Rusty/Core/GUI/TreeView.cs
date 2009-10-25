using System;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// Adds a new item to the TreeView and returns its unique Item ID number (or 0 upon failure). Name is the displayed text of the item, which can be text or numeric (including numeric expression results). ParentItemID is the ID number of the new item's parent (omit it or specify 0 to add the item at the top level). When adding a large number of items, performance can be improved by using GuiControl, -Redraw, MyTreeView before adding the items, and GuiControl, +Redraw, MyTreeView afterward.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ParentItemID"></param>
        /// <param name="Options"></param>
        /// <returns></returns>
        public static int TV_Add(string Name, int ParentItemID, string Options)
        {
            TreeNode child = new TreeNode(Name);

            TreeNode.FromHandle(Settings.GUI.TreeView, new IntPtr(ParentItemID)).
                Nodes.Add(child);
            return child.Handle.ToInt32();
        }

        /// <summary>
        /// If ItemID is omitted, all items in the TreeView are deleted. Otherwise, only the specified ItemID is deleted. It returns 1 upon success and 0 upon failure.
        /// </summary>
        /// <param name="ItemID"></param>
        /// <returns></returns>
        public static bool TV_Delete(int ItemID)
        {
            try
            {
                TreeView tv = Settings.GUI.TreeView;
                if (ItemID == 0)
                    foreach (TreeNode item in tv.Nodes)
                        item.Remove();
                TreeNode.FromHandle(tv, (IntPtr)ItemID).Remove();
                return true;
            }
            catch (Exception) { return false; }
        }

        /// <summary>
        /// Returns the ID number of the specified item's first/top child (or 0 if none).
        /// </summary>
        /// <param name="ParentItemID"></param>
        /// <returns></returns>
        public static int TV_GetChild(int ParentItemID)
        {
            try
            {
                TreeView tv = Settings.GUI.TreeView;
                return TreeNode.FromHandle(tv, (IntPtr)ParentItemID).FirstNode.Handle.ToInt32();
            }
            catch (Exception) { return 0; }
        }

        /// <summary>
        /// Returns the total number of items in the control. This function is always instantaneous because the control keeps track of the count.
        /// </summary>
        /// <returns></returns>
        public static int TV_GetCount()
        {
            try
            {
                TreeView tv = Settings.GUI.TreeView;
                return tv.Nodes.Count;
            }
            catch (Exception) { return 0; }
        }

        /// <summary>
        /// This has the following modes:
        /// </summary>
        /// <param name="ItemID"></param>
        /// <param name="Checked_Full"></param>
        /// <returns></returns>
        public static int TV_GetNext(int? ItemID, string Checked_Full)
        {
            try
            {
                TreeView tv = Settings.GUI.TreeView;
                IntPtr handle = IntPtr.Zero;
                if (ItemID == null)
                    handle = tv.Nodes[0].Handle;
                else
                {
                    TreeNode item = TreeNode.FromHandle(tv, (IntPtr)ItemID);
                    string mode = " " + Checked_Full.ToLower();
                    bool full = mode.Contains(" f");
                    if (mode.Contains(" c"))
                        while (!item.Checked)
                            item = full ? tv.Nodes[item.Index + 1] : item.NextNode;
                    else item = full ? tv.Nodes[item.Index + 1] : item.NextNode;
                    handle = item.Handle;
                }

                return handle.ToInt32();
            }
            catch (Exception) { return 0; }
        }

        //public static int TV_Get(int ItemID, string Mode) // ?!?!
        //{
        //    try
        //    {
        //        TreeView tv = Settings.GUI.TreeView;
        //        TreeNode item = TreeNode.FromHandle(tv, (IntPtr)ItemID);
        //        string mode = " " + Mode.ToLower();
        //        if ((mode.Contains(" e") && !item.IsExpanded) ||
        //            (mode.Contains(" c") && !item.Checked) ||
        //            (mode.Contains(" b") && !item.NodeFont.Bold))
        //            return 0;
        //        else return item.Handle.ToInt32();
        //    }
        //    catch (Exception) { return 0; }
        //}

        /// <summary>
        /// Returns the specified item's parent as an item ID. Items at the top level have no parent and thus return 0.
        /// </summary>
        /// <param name="ItemID"></param>
        /// <returns></returns>
        public static int TV_GetParent(int ItemID)
        {
            try
            {
                TreeView tv = Settings.GUI.TreeView;
                return TreeNode.FromHandle(tv, (IntPtr)ItemID).Parent.Handle.ToInt32();
            }
            catch (Exception) { return 0; }
        }

        /// <summary>
        /// Returns the ID number of the sibling above the specified item (or 0 if none).
        /// </summary>
        /// <param name="ItemID"></param>
        /// <returns></returns>
        public static int TV_GetPrev(int ItemID)
        {
            try
            {
                TreeView tv = Settings.GUI.TreeView;
                return TreeNode.FromHandle(tv, (IntPtr)ItemID).PrevNode.Handle.ToInt32();
            }
            catch (Exception) { return 0; }
        }

        /// <summary>
        /// Returns the selected item's ID number.
        /// </summary>
        /// <returns></returns>
        public static int TV_GetSelection()
        {
            try
            {
                TreeView tv = Settings.GUI.TreeView;
                return tv.SelectedNode.Handle.ToInt32();
            }
            catch (Exception) { return 0; }
        }

        /// <summary>
        /// Retrieves the text/name of the specified ItemID and stores it in OutputVar. If the text is longer than 8191, only the first 8191 characters are retrieved. Upon success, the function returns the item's own ID. Upon failure, it returns 0 (and OutputVar is also made blank).
        /// </summary>
        /// <param name="OutputVar"></param>
        /// <param name="ItemID"></param>
        /// <returns></returns>
        public static int TV_GetText(out string OutputVar, int ItemID)
        {
            OutputVar = string.Empty;
            try
            {
                TreeView tv = Settings.GUI.TreeView;
                TreeNode item = TreeNode.FromHandle(tv, (IntPtr)ItemID);
                OutputVar = item.Text;
                return item.Handle.ToInt32();
            }
            catch (Exception) { return 0; }
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
            TreeNode node = TreeNode.FromHandle(Settings.GUI.TreeView, new IntPtr(ItemID));

            if (NewName.Length != 0)
                node.Name = NewName;

            Formats.TV_NodeOptions(ref node, Options);

            return node.Handle.ToInt32();
        }
    }
}