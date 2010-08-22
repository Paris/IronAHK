using System;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        // TODO: organise ListView.cs

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static int LV_Add(string options, string[] fields)
        {
            var list = DefaultListView;

            if (list == null)
                return 0;

            var item = new ListViewItem();
            item.SubItems.AddRange(fields);
            var vis = false;

            foreach (var opt in ParseOptions(options.ToLowerInvariant()))
            {
                var on = opt[0] != '-';
                var mode = opt.Substring(!on || opt[0] == '+' ? 1 : 0);

                switch (mode)
                {
                    case Keyword_Checked: item.Checked = on; break;
                    case Keyword_Focus: item.Focused = on; break;
                    case Keyword_Select: item.Selected = on; break;
                    case Keyword_Vis: vis = on; break;

                    default:
                        {
                            int n;

                            if (mode.StartsWith(Keyword_Icon) && int.TryParse(mode.Substring(Keyword_Icon.Length), out n))
                                item.ImageIndex = n;
                        }
                        break;
                }
            }

            list.Items.Add(item);

            if (vis)
                item.EnsureVisible();

            return item.Index;
        }

        /// <summary>
        /// Removes one or all ListView rows.
        /// </summary>
        /// <param name="row">The row number to remove. Leave blank to remove every row.</param>
        /// <returns><c>true</c> if one or more rows were deleted, <c>false</c> otherwise.</returns>
        public static bool LV_Delete(int row = -1)
        {
            var list = DefaultListView;

            if (list == null)
                return false;

            if (row == -1)
            {
                foreach (ListViewItem item in list.Items)
                    item.Remove();

                return true;
            }

            row--;

            if (row > 0 && row < list.Items.Count)
            {
                list.Items.RemoveAt(row);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Deletes a ListView column.
        /// </summary>
        /// <param name="column">The column index to remove.</param>
        /// <returns><c>true</c> if the specified column was removed, <c>false</c> otherwise.</returns>
        public static bool LV_DeleteCol(int column)
        {
            var list = DefaultListView;

            if (list == null)
                return false;

            column--;

            if (column > 0 && column < list.Columns.Count)
            {
                list.Columns.RemoveAt(column);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the number of columns or rows in a ListView.
        /// </summary>
        /// <param name="type">
        /// <list type="bullet">
        /// <item><term>(blank)</term>: <description>all rows</description></item>
        /// <item><term>Selected</term>: <description>selected rows</description></item>
        /// <item><term>Column</term>: <description>columns</description></item>
        /// </list>
        /// </param>
        /// <returns>The number of columns or rows as specified by <paramref name="type"/>.</returns>
        public static int LV_GetCount(string type = null)
        {
            var list = DefaultListView;

            if (list == null)
                return 0;

            if (string.IsNullOrEmpty(type))
                return list.Items.Count;

            type = string.IsNullOrEmpty(type) ? string.Empty : type.ToLowerInvariant();

            if (type == Keyword_Column || type.Length == 1 && type[0] == Keyword_Column[0] || type.Length == 3 && Keyword_Column.StartsWith(type))
                return list.Columns.Count;

            if (type == Keyword_Selected || type.Length == 1 && type[0] == Keyword_Selected[0])
            {
                var selected = 0;

                foreach (ListViewItem item in list.Items)
                    if (item.Selected)
                        selected++;

                return selected;
            }

            return 0;
        }

        /// <summary>
        /// Returns the row number of the next selected, checked, or focused row.
        /// </summary>
        /// <param name="index">The starting index. Leave blank to search from the first row.</param>
        /// <param name="type">
        /// <list type="bullet">
        /// <item><term>(blank)</term>: <description>a selected row</description></item>
        /// <item><term>Checked</term>: <description>a checked row</description></item>
        /// <item><term>Focused</term>: <description>a focused row</description></item>
        /// </list>
        /// </param>
        /// <returns>The next row number matching the specified criteria.</returns>
        public static int LV_GetNext(int index, string type = null)
        {
            var list = DefaultListView;

            if (list == null)
                return 0;

            type = string.IsNullOrEmpty(type) ? string.Empty : type.ToLowerInvariant();

            for (int i = Math.Max(0, index); i < list.Items.Count; i++)
            {
                var item = list.Items[i];

                if (string.IsNullOrEmpty(type))
                    if (item.Selected)
                        return i;

                if (type == Keyword_Checked || type.Length == 1 && type[0] == Keyword_Checked[0])
                    if (item.Checked)
                        return i;

                if (type == Keyword_Focused || type.Length == 1 && type[0] == Keyword_Focused[0])
                    if (item.Focused)
                        return i;
            }

            return 0;
        }

        /// <summary>
        /// Retrieves the text at the specified <paramref name="row"/> and <paramref name="column"/>.
        /// </summary>
        /// <param name="result">The variable in which to store the retrieved text.</param>
        /// <param name="row">The row index. Leave blank to return the <paramref name="column"/> header.</param>
        /// <param name="column">The column index.</param>
        /// <returns><c>true</c> if the specified <paramref name="row"/> and <paramref name="column"/> was found, <c>false</c> otherwise.</returns>
        public static bool LV_GetText(out string result, int row = 1, int column = 1)
        {
            var list = DefaultListView;
            result = string.Empty;

            if (list == null)
                return false;

            row--;
            column--;

            if (row == 0)
            {
                if (column > 0 && column < list.Columns.Count)
                {
                    result = list.Columns[column].Text;
                    return true;
                }
            }

            if (row > 0 && row < list.Items.Count)
            {
                var item = list.Items[row];

                if (column > 0 && column < item.SubItems.Count)
                {
                    result = item.SubItems[column].Text;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="options"></param>
        /// <param name="columns"></param>
        public static void LV_Insert(int row, string options, string[] columns)
        {
            var list = DefaultListView;

            if (list == null)
                return;

            throw new NotImplementedException(); // TODO: LV_Insert
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        /// <param name="options"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static int LV_InsertCol(int column, string options, string title)
        {
            var list = DefaultListView;

            if (list == null)
                return 0;

            throw new NotImplementedException(); // TODO: LV_InsertCol
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="options"></param>
        /// <param name="column"></param>
        public static void LV_Modify(int row, string options, string[] column)
        {
            var list = DefaultListView;

            if (list == null)
                return;

            throw new NotImplementedException(); // TODO: LV_Modify
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        /// <param name="options"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static int LV_ModifyCol(int column, string options, string title)
        {
            var list = DefaultListView;

            if (list == null)
                return 0;

            throw new NotImplementedException(); // TODO: LV_ModifyCol
        }

        /// <summary>
        /// Sets the ImageList whose icons will be displayed by the rows of the ListView.
        /// </summary>
        /// <param name="id">The ImageList ID. See <see cref="IL_Create"/>.</param>
        /// <param name="type">
        /// The type of image list:
        /// <list type="bullet">
        /// <item><term>0 (default)</term>: <description>large</description></item>
        /// <item><term>1</term>: <description>small</description></item>
        /// <item><term>2</term>: <description>state</description></item>
        /// </list>
        /// </param>
        public static void LV_SetImageList(long id, int type = 0)
        {
            var list = DefaultListView;

            if (list == null)
                return;

            if (!imageLists.ContainsKey(id))
                return;

            var img = imageLists[id];

            switch (type)
            {
                case 0: list.LargeImageList = img; break;
                case 1: list.SmallImageList = img; break;
                case 2: list.StateImageList = img; break;
            }
        }
    }
}
