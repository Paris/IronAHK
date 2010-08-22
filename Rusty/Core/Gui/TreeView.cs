using System.Drawing;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// Adds a new node to the TreeView.
        /// </summary>
        /// <param name="text">The node text.</param>
        /// <param name="parent">The optional parent node ID.</param>
        /// <param name="options">See <see cref="TV_Modify"/>.</param>
        /// <returns>The ID of the created node.</returns>
        public static long TV_Add(string text = null, long parent = 0, string options = null)
        {
            var tree = DefaultTreeView;

            if (tree == null)
                return 0;

            text = text ?? string.Empty;
            TreeNode node;

            if (parent == 0)
                node = tree.Nodes.Add(text);
            else
            {
                var top = TV_FindNode(tree, parent);
                node = top == null ? tree.Nodes.Add(text) : top.Nodes.Add(text);
            }

            var id = node.Handle.ToInt64();
            node.Name = id.ToString();
            TV_NodeOptions(node, options);
            return id;
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
        /// Modifies the attributes and/or text of a node.
        /// </summary>
        /// <param name="id">The node ID.</param>
        /// <param name="options">
        /// <list type="bullet">
        /// <item><term><c>bold</c></term>: <description>displays <paramref name="text"/> in bold</description></item>
        /// <item><term><c>check</c></term>: <description>shows a checkmark beside the node</description></item>
        /// <item><term><c>select</c></term>: <description>selects the node</description></item>
        /// <item><term><c>vis</c></term>: <description>ensures the node is visible</description></item>
        /// </list>
        /// </param>
        /// <param name="text">The new node text. Leave blank to keep the current text.</param>
        /// <returns>The ID of the modified node.</returns>
        public static long TV_Modify(long id, string options, string text = null)
        {
            var tree = DefaultTreeView;

            if (tree == null)
                return 0;

            var node = TV_FindNode(tree, id);

            if (node == null)
                return 0;

            if (text != null)
                node.Text = text;

            TV_NodeOptions(node, options);

            return node.Handle.ToInt64();
        }
    }
}
