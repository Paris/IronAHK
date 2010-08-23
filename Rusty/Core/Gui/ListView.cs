using System;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// Adds a new row to the end of the list
        /// </summary>
        /// <param name="options">See <see cref="LV_Modify"/>.</param>
        /// <param name="fields">Text for each column.</param>
        /// <returns>The 1-based index of the new row.</returns>
        public static int LV_Add(string options, string[] fields = null)
        {
            var list = DefaultListView;

            if (list == null)
                return 0;

            var item = new ListViewItem();
            list.Items.Add(item);
            LV_Modify(item.Index + 1, options, fields);

            return item.Index;
        }

        /// <summary>
        /// Removes one or all rows.
        /// </summary>
        /// <param name="row">The 1-based row index. Leave blank to remove every row.</param>
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
        /// Deletes a column.
        /// </summary>
        /// <param name="column">The 1-based column index.</param>
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
        /// Returns the number of columns or rows.
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
                var n = i + 1;

                if (string.IsNullOrEmpty(type))
                    if (item.Selected)
                        return n;

                if (type == Keyword_Checked || type.Length == 1 && type[0] == Keyword_Checked[0])
                    if (item.Checked)
                        return n;

                if (type == Keyword_Focused || type.Length == 1 && type[0] == Keyword_Focused[0])
                    if (item.Focused)
                        return n;
            }

            return 0;
        }

        /// <summary>
        /// Retrieves the text at the specified <paramref name="row"/> and <paramref name="column"/>.
        /// </summary>
        /// <param name="result">The variable in which to store the retrieved text.</param>
        /// <param name="row">The 1-based row index. Leave blank to return the <paramref name="column"/> header.</param>
        /// <param name="column">The 1-based column index.</param>
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
        /// Inserts a row at the specified position.
        /// </summary>
        /// <param name="row">The 1-based row index.</param>
        /// <param name="options">See <see cref="LV_Modify"/>.</param>
        /// <param name="fields">Text for each column.</param>
        public static void LV_Insert(int row, string options, string[] fields = null)
        {
            var list = DefaultListView;

            if (list == null)
                return;

            var item = new ListViewItem();

            row = Math.Max(row - 1, 0);

            if (row > list.Items.Count)
                list.Items.Add(item);
            else
                list.Items.Insert(row, item);

            LV_Modify(item.Index + 1, options, fields);
        }

        /// <summary>
        /// Creates a new column at the specified position.
        /// </summary>
        /// <param name="column">The 1-based row index.</param>
        /// <param name="options">See <see cref="LV_ModifyCol"/>.</param>
        /// <param name="title">The column title.</param>
        /// <returns>The index of the created column.</returns>
        public static int LV_InsertCol(int column = 0, string options = null, string title = null)
        {
            var list = DefaultListView;

            if (list == null)
                return 0;

            title = title ?? string.Empty;
            column--;

            if (column < 0 || column > list.Columns.Count)
                column = list.Columns.Add(title).Index;
            else
                list.Columns.Insert(column, title);

            column++;

            LV_ModifyCol(column, options, null);

            return column;
        }

        /// <summary>
        /// Modifies the attributes and/or text of a row.
        /// </summary>
        /// <param name="row">The 1-based row index.</param>
        /// <param name="options"></param>
        /// <param name="fields">New text for each column. Leave blank to use the existing contents.</param>
        /// <returns><c>true</c> if attributes and/or text were successfully applied, <c>false</c> otherwise.</returns>
        public static bool LV_Modify(int row = 0, string options = null, string[] fields = null)
        {
            var list = DefaultListView;

            if (list == null)
                return false;

            if (row == 0)
            {
                var pass = true;

                foreach (ListViewItem sub in list.Items)
                    if (!LV_Modify(sub.Index + 1, options, fields))
                        pass = false;

                return pass;
            }

            row--;

            if (row < 0 || row > list.Items.Count)
                return false;

            var item = list.Items[row];

            if (fields != null)
            {
                item.SubItems.Clear();
                item.SubItems.AddRange(fields);
            }

            foreach (var opt in ParseOptions(options.ToLowerInvariant()))
            {
                var on = opt[0] != '-';
                var mode = opt.Substring(!on || opt[0] == '+' ? 1 : 0);

                switch (mode)
                {
                    case Keyword_Checked: item.Checked = on; break;
                    case Keyword_Focus: item.Focused = on; break;
                    case Keyword_Select: item.Selected = on; break;
                    case Keyword_Vis: item.EnsureVisible(); break;

                    default:
                        {
                            int n;

                            if (mode.StartsWith(Keyword_Icon) && int.TryParse(mode.Substring(Keyword_Icon.Length), out n))
                                item.ImageIndex = n;
                        }
                        break;
                }
            }

            return true;
        }

        /// <summary>
        /// Modifies the attributes and/or text of the specified column and its header.
        /// </summary>
        /// <param name="column">The 1-based column index.</param>
        /// <param name="options"></param>
        /// <param name="title">The column title. Leave blank to keep the existing contents.</param>
        /// <returns><c>true</c> if attributes and/or text were successfully applied, <c>false</c> otherwise.</returns>
        /// <remarks>Leave <paramref name="options"/> and <paramref name="title"/> blank to auto-size the column to fit its contents width.
        /// If <paramref name="column"/> is also unspecified every column will be adjusted this way.</remarks>
        public static bool LV_ModifyCol(int column = -1, string options = null, string title = null)
        {
            var list = DefaultListView;

            if (list == null)
                return false;

            if (column == -1 && options == null && title == null)
            {
                var pass = true;
                for (var i = 0; i < list.Columns.Count; i++)
                    if (!LV_ModifyCol(list.Columns[i].Index + 1, null, null))
                        pass = false;
                return pass;
            }

            column--;

            if (column < 0 || column > list.Columns.Count)
                return false;

            var col = list.Columns[column];

            if (options == null && title == null)
            {
                col.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                return true;
            }

            if (title != null)
                col.Text = title;

            if (string.IsNullOrEmpty(options))
                return true;

            foreach (var opt in ParseOptions(options))
            {
                bool on = opt[0] != '-';
                string mode = opt.Substring(!on || opt[0] == '+' ? 1 : 0).ToLowerInvariant();
                
                switch (mode)
                {
                    // TODO: LV_ModifyCol options
                    case Keyword_Auto: col.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent); break;
                    case Keyword_AutoHdr: col.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize); break;
                    case Keyword_IconRight: break;
                    case Keyword_Float: break;
                    case Keyword_Integer: break;
                    case Keyword_Text: break;
                    case Keyword_Center: col.TextAlign = HorizontalAlignment.Center; break;
                    case Keyword_Left: col.TextAlign = HorizontalAlignment.Left; break;
                    case Keyword_Right: col.TextAlign = HorizontalAlignment.Right; break;
                    case Keyword_Case: break;
                    case Keyword_CaseLocale: break;
                    case Keyword_Desc: break;
                    case Keyword_Locale: break;
                    case Keyword_NoSort: break;
                    case Keyword_Sort: break;
                    case Keyword_SortDesc: break;
                    case Keyword_Unicode: break;

                    default:
                        int n;
                        if (mode.StartsWith(Keyword_Icon, StringComparison.OrdinalIgnoreCase) && int.TryParse(mode.Substring(Keyword_Icon.Length), out n))
                            col.ImageIndex = n;
                        else if (int.TryParse(mode, out n))
                            col.Width = n;
                        break;
                }
            }

            return true;
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
