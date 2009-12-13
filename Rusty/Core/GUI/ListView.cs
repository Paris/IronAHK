using System;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// Adds a new row to the bottom of the list. The parameters Field1 and beyond are the columns of the new row, which can be text or numeric (including numeric expression results). To make any field blank, specify "" or the equivalent. If there are too few fields to fill all the columns, the columns at the end are left blank. If there are too many fields, the fields at the end are completely ignored.
        /// </summary>
        /// <param name="Options"></param>
        /// <param name="FieldN"></param>
        /// <returns></returns>
        public static int LV_Add(string Options, string[] FieldN)
        {
            try
            {
                ListViewItem row = new ListViewItem();
                foreach (string field in FieldN)
                    row.SubItems.Add(field);
                LV_RowOptions(ref row, Options);

                Settings.GUI.ListView.Items.Add(row);
                return row.Index + 1;
            }
            catch (Exception) { return 0; }
        }

        /// <summary>
        /// If the parameter is omitted, all rows in the ListView are deleted. Otherwise, only the specified RowNumber is deleted. It returns 1 upon success and 0 upon failure.
        /// </summary>
        /// <param name="RowNumber"></param>
        /// <returns></returns>
        public static bool LV_Delete(int RowNumber)
        {
            try
            {
                Settings.GUI.ListView.Items[RowNumber].Remove();
                return true;
            }
            catch (Exception) { return false; }
        }

        /// <summary>
        /// Deletes the specified column and all of the contents beneath it. It returns 1 upon success and 0 upon failure. Once a column is deleted, the column numbers of any that lie to its right are reduced by 1. Consequently, calling LV_DeleteCol(2) twice would delete the second and third columns. On operating systems older than Windows XP, attempting to delete the original first column might might fail and return 0.
        /// </summary>
        /// <param name="ColumnNumber"></param>
        /// <returns></returns>
        public static bool LV_DeleteCol(int ColumnNumber)
        {
            try
            {
                Settings.GUI.ListView.Columns.RemoveAt(ColumnNumber);
                return true;
            }
            catch (Exception) { return false; }
        }

        /// <summary>
        /// When the parameter is omitted, the function returns the total number of rows in the control. When the parameter is "S" or "Selected", the count includes only the selected/highlighted rows. When the parameter is "Col" or "Column", the function returns the number of columns in the control. This function is always instantaneous because the control keeps track of these counts.
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static int LV_GetCount(string Type)
        {
            Type = Type.ToLower();
            if (Type == Keywords.Selected || (Type.Length == 1 && Type[0] == Keywords.Selected[0]))
                return Settings.GUI.ListView.SelectedItems.Count;
            else if (Type == Keywords.Column || (Type.Length == 1 && Type[0] == Keywords.Column[0]))
                return Settings.GUI.ListView.Columns.Count;
            else throw new ArgumentOutOfRangeException(); // graceful error instead?
        }

        /// <summary>
        /// Returns the row number of the next selected, checked, or focused row. If none is found, zero is returned. If StartingRowNumber is omitted or less than 1, the search begins at the top of the list. Otherwise, the search begins at the row after StartingRowNumber. If the second parameter is omitted, the function searches for the next selected/highlighted row. Otherwise, specify "C" or "Checked" to find the next checked row; or "F" or "Focused" to find the focused row (there is never more than one focused row in the entire list, and sometimes there is none at all).
        /// </summary>
        /// <param name="StartingRowNumber"></param>
        /// <param name="Mode"></param>
        /// <returns></returns>
        public static int LV_GetNext(int StartingRowNumber, string Mode)
        {
            ListView lv = Settings.GUI.ListView;
            var opts = ParseKeys(Mode);
            bool check = opts.Contains("c") || opts.Contains("checked");
            bool focus = opts.Contains("f") || opts.Contains("focused");

            for (int i = StartingRowNumber; i < lv.Items.Count; i++)
            {
                ListViewItem item = lv.Items[i];
                if ((check && item.Checked) && (focus && item.Focused))
                    return i;
            }

            return 0;
        }

        /// <summary>
        /// Retrieves the text at the specified RowNumber and ColumnNumber and stores it in OutputVar. If ColumnNumber is omitted, it defaults to 1 (the text in the first column). If RowNumber is 0, the column header text is retrieved. If the text is longer than 8191, only the first 8191 characters are retrieved. The function returns 1 upon success and 0 upon failure. Upon failure, OutputVar is also made blank.
        /// </summary>
        /// <param name="OutputVar"></param>
        /// <param name="RowNumber"></param>
        /// <param name="ColumnNumber"></param>
        /// <returns></returns>
        public static bool LV_GetText(out string OutputVar, int RowNumber, int ColumnNumber)
        {
            try
            {
                ListView lv = Settings.GUI.ListView;
                OutputVar = RowNumber < 1 ? lv.Columns[ColumnNumber].Text : lv.Items[RowNumber].SubItems[ColumnNumber - 1].Text;
                return true;
            }
            catch (Exception)
            {
                OutputVar = null;
                return false;
            }
        }

        /// <summary>
        /// Behaves identically to LV_Add() except for its different first parameter, which specifies the row number for the newly inserted row. Any rows at or beneath RowNumber are shifted downward to make room for the new row. If RowNumber is greater than the number of rows in the list (even as high as 2147483647), the new row is added to the end of the list. For Options, see row options.
        /// </summary>
        /// <param name="RowNumber"></param>
        /// <param name="Options"></param>
        /// <param name="ColN"></param>
        public static void LV_Insert(string RowNumber, string Options, string[] ColN)
        {

        }

        /// <summary>
        /// Creates a new column, inserting it as the specified ColumnNumber (shifting any other columns to the right to make room). The first column is 1 (not 0). If ColumnNumber is larger than the number of columns currently in the control, the new column is added to the end of the list. The newly inserted column starts off with empty contents beneath it unless it is the first column, in which case it inherits the old first column's contents and the old first column acquires blank contents. The new column's attributes -- such as whether or not it uses integer sorting -- always start off at their defaults unless changed via Options. This function returns the new column's position number (or 0 upon failure). The maximum number of columns in a ListView is 200.
        /// </summary>
        /// <param name="ColumnNumber"></param>
        /// <param name="Options"></param>
        /// <param name="ColumnTitle"></param>
        /// <returns></returns>
        public static int LV_InsertCol(int ColumnNumber, string Options, string ColumnTitle)
        {
            ListView lv = Settings.GUI.ListView;
            ColumnHeader col = new ColumnHeader();
            LV_ColOptions(ref col, Options);
            lv.Columns.Insert(ColumnNumber - 1, col);
            return ColumnNumber;
        }

        /// <summary>
        /// Modifies the attributes and/or text of a row, and returns 1 upon success and 0 upon failure. If RowNumber is 0, all rows in the control are modified (in this case the function returns 1 on complete success and 0 if any part of the operation failed). When only the first two parameters are present, only the row's attributes and not its text are changed. Similarly, if there are too few parameters to cover all the columns, the columns at the end are not changed. The ColN option may be used to update specific columns without affecting the others. For other options, see row options.
        /// </summary>
        /// <param name="RowNumber"></param>
        /// <param name="Options"></param>
        /// <param name="NewCol1"></param>
        /// <param name="NewCol2"></param>
        /// <param name="Mode"></param>
        public static void LV_Modify(string RowNumber, string Options, string NewCol1, string NewCol2, string Mode)
        {

        }

        /// <summary>
        /// Modifies the attributes and/or text of the specified column and its header. The first column is number 1 (not 0). If all parameters are omitted, the width of every column is adjusted to fit the contents of the rows. If only the first parameter is present, only the specified column is auto-sized. Auto-sizing has no effect when not in Report (Details) view. This function returns 1 upon success and 0 upon failure.
        /// </summary>
        /// <param name="ColumnNumber"></param>
        /// <param name="Options"></param>
        /// <param name="ColumnTitle"></param>
        /// <returns></returns>
        public static int LV_ModifyCol(int ColumnNumber, string Options, string ColumnTitle)
        {
            try
            {
                ListView lv = Settings.GUI.ListView;
                if (ColumnTitle.Length != 0)
                    lv.Columns[ColumnNumber].Text = ColumnTitle;
                ColumnHeader col = lv.Columns[ColumnNumber];
                LV_ColOptions(ref col, Options);
                return 1;
            }
            catch (Exception) { return 0; }
        }

        /// <summary>
        /// <para>This function is normally called prior to adding any rows to the ListView. It sets the ImageList whose icons will be displayed by the ListView's rows (and optionally, its columns). ImageListID is the number returned from a previous call to IL_Create(). If the second parameter is omitted, the type of icons in the ImageList is detected automatically as large or small. Otherwise, specify 0 for large icons, 1 for small icons, and 2 for state icons (state icons are not yet directly supported, but they could be used via SendMessage).</para>
        /// <para>A ListView may have up to two ImageLists: small-icon and/or large-icon. This is useful when the script allows the user to switch to and from the large-icon view. To add more than one ImageList to a ListView, call LV_SetImageList() a second time, specifying the ImageListID of the second list. A ListView with both a large-icon and small-icon ImageList should ensure that both lists contain the icons in the same order. This is because the same ID number is used to reference both the large and small versions of a particular icon.</para>
        /// <para>Although it is traditional for all viewing modes except Icon and Tile to show small icons, this can be overridden by passing a large-icon list to LV_SetImageList and specifying 1 (small-icon) for the second parameter. This also increases the height of each row in the ListView to fit the large icon.</para>
        /// <para>If successful, LV_SetImageList() returns the ImageListID that was previously associated with the ListView (or 0 if none). Any such detached ImageList should normally be destroyed via IL_Destroy(ImageListID).</para>
        /// </summary>
        /// <param name="ImageListID"></param>
        /// <param name="Mode"></param>
        public static void LV_SetImageList(int ImageListID, string Mode)
        {

        }
    }
}