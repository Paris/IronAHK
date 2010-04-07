using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        // TODO: organise Gui.cs

        /// <summary>
        /// Creates and manages windows and controls.
        /// </summary>
        /// <param name="Command">
        /// <list type="bullet">
        /// <item><term>Add</term>: <description>creates controls.</description></item>
        /// <item><term>Show</term>: <description>display or move the window.</description></item>
        /// <item><term>Submit</term>: <description>saves user input.</description></item>
        /// <item><term>Hide</term>: <description>hides the window.</description></item>
        /// <item><term>Destroy</term>: <description>deletes the window.</description></item>
        /// <item><term>Font</term>: <description>sets the default font style for subsequently created controls.</description></item>
        /// <item><term>Color</term>: <description>sets the color for the window or controls.</description></item>
        /// <item><term>Margin</term>: <description>sets the spacing used between the edges of the window and controls when an absolute position is unspecified.</description></item>
        /// <item><term>Options</term>: <description>sets various options for the appearance and behaviour of the window.</description></item>
        /// <item><term>Menu</term>: <description>associates a menu bar with the window.</description></item>
        /// <item><term>Minimize/Maximize/Restore</term>: <description>performs the indicated operation on the window.</description></item>
        /// <item><term>Flash</term>: <description>blinks the window in the task bar.</description></item>
        /// <item><term>Default</term>: <description>changes the default window on the current thread.</description></item>
        /// </list>
        /// </param>
        /// <param name="Param2"></param>
        /// <param name="Param3"></param>
        /// <param name="Param4"></param>
        public static void Gui(string Command, string Param2, string Param3, string Param4)
        {
            if (guis == null)
                guis = new Dictionary<string, BaseGui.Window>();

            string id = GuiId(ref Command);

            if (!guis.ContainsKey(id))
                guis.Add(id, GuiCreateWindow(id));

            switch (Command.ToLowerInvariant())
            {
                #region Add

                case Keyword_Add:
                    {
                        switch (Param2.ToLowerInvariant())
                        {
                            case Keyword_Text:
                                {
                                    var text = guis[id].CreateText();
                                    text.Contents = Param4;
                                    GuiApplyStyles(text, Param3);
                                    guis[id].Add(text);
                                }
                                break;

                            case Keyword_Edit:
                                {
                                    var edit = guis[id].CreateEdit();
                                    edit.Contents = Param4;
                                    string opts = GuiApplyStyles(edit, Param3);

                                    foreach (string opt in ParseOptions(opts))
                                    {
                                        bool on = opt[0] != '-';
                                        string mode = opt.Substring(!on || opt[0] == '+' ? 1 : 0).ToLowerInvariant();

                                        switch (mode)
                                        {
                                            case Keyword_Limit:
                                                if (!on)
                                                    edit.Limit = int.MaxValue;
                                                break;

                                            case Keyword_Lowercase: edit.Lowercase = on; break;
                                            case Keyword_Multi: edit.Multi = on; break;
                                            case Keyword_Number: edit.Number = on; break;
                                            case Keyword_Password: edit.Password = on; break;
                                            case Keyword_Readonly: edit.ReadOnly = on; break;
                                            case Keyword_Uppercase: edit.Uppercase = on; break;
                                            case Keyword_WantCtrlA: edit.WantCtrlA = on; break;
                                            case Keyword_WantReturn: edit.WantReturn = on; break;
                                            case Keyword_WantTab: edit.WantTab = on; break;
                                            case Keyword_Wrap: edit.Wrap = on; break;

                                            default:
                                                int n;
                                                if (mode.StartsWith(Keyword_Limit) && int.TryParse(mode.Substring(Keyword_Limit.Length), out n))
                                                    edit.Limit = n;
                                                else if (mode[0] == 't' && int.TryParse(mode.Substring(1), out n))
                                                        edit.TabStops = n;
                                                break;
                                        }
                                    }

                                    guis[id].Add(edit);
                                }
                                break;

                            case Keyword_UpDown:
                                {
                                    var updown = guis[id].CreateUpDown();
                                    updown.Contents = Param4;
                                    string opts = GuiApplyStyles(updown, Param3);

                                    foreach (string opt in ParseOptions(opts))
                                    {
                                        bool on = opt[0] != '-';
                                        string mode = opt.Substring(!on || opt[0] == '+' ? 1 : 0).ToLowerInvariant();

                                        switch (mode)
                                        {
                                            case Keyword_Horz: updown.Horizontal = on; break;
                                            case Keyword_Left: updown.Left = on; break;
                                            case Keyword_Wrap: updown.Wrap = on; break;
                                            case "16": updown.Isolated = on; break;
                                            case "0x80": updown.ThousandsSeperator = on; break;

                                            default:
                                                if (mode.StartsWith(Keyword_Range))
                                                {
                                                    string[] range = mode.Substring(Keyword_Range.Length).Split(new[] { '-' }, 2);
                                                    int n;

                                                    if (int.TryParse(range[0], out n))
                                                        updown.RangeMinimum = n;

                                                    if (range.Length > 1 && int.TryParse(range[1], out n))
                                                        updown.RangeMaximum = n;
                                                }
                                                break;
                                        }
                                    }

                                    guis[id].Add(updown);
                                }
                                break;

                            case Keyword_Picture:
                            case Keyword_Pic:
                                {
                                    var pic = guis[id].CreatePicture();
                                    pic.Contents = Param4;
                                    GuiApplyStyles(pic, Param3);
                                    guis[id].Add(pic);
                                }
                                break;

                            case Keyword_Button:
                                {
                                    var button = guis[id].CreateButton();
                                    button.Contents = Param4;
                                    GuiApplyStyles(button, Param3);
                                    guis[id].Add(button);
                                }
                                break;

                            case Keyword_CheckBox:
                                {
                                    var check = guis[id].CreateCheckbox();
                                    check.Contents = Param4;
                                    check.State = CheckState.Unchecked;
                                    string opts = GuiApplyStyles(check, Param3);

                                    foreach (string opt in ParseOptions(opts))
                                    {
                                        switch (opt.ToLowerInvariant())
                                        {
                                            case Keyword_Check3:
                                            case Keyword_CheckedGray:
                                                check.State = CheckState.Indeterminate;
                                                break;

                                            case Keyword_Checked:
                                                check.State = CheckState.Checked;
                                                break;

                                            default:
                                                if (opt.StartsWith(Keyword_Checked, StringComparison.OrdinalIgnoreCase))
                                                {
                                                    string arg = opt.Substring(Keyword_Checked.Length).Trim();
                                                    int n;

                                                    if (int.TryParse(arg, out n))
                                                        check.State = n == -1 ? CheckState.Indeterminate : n == 1 ? CheckState.Checked : CheckState.Unchecked;
                                                }
                                                break;
                                        }
                                    }

                                    guis[id].Add(check);
                                }
                                break;

                            case Keyword_Radio:
                                {
                                    var radio = guis[id].CreateRadio();
                                    radio.Contents = Param4;
                                    radio.Checked = false;
                                    string opts = GuiApplyStyles(radio, Param3);

                                    foreach (string opt in ParseOptions(opts))
                                    {
                                        switch (opt.ToLowerInvariant())
                                        {
                                            case Keyword_Checked:
                                                radio.Checked = true;
                                                break;

                                            default:
                                                if (opt.StartsWith(Keyword_Checked, StringComparison.OrdinalIgnoreCase))
                                                {
                                                    string arg = opt.Substring(Keyword_Checked.Length).Trim();
                                                    int n;

                                                    if (int.TryParse(arg, out n))
                                                        radio.Checked = n == 1;
                                                }
                                                break;
                                        }
                                    }

                                    guis[id].Add(radio);
                                }
                                break;

                            case Keyword_DropDownList:
                            case Keyword_DDL:
                                {
                                    var ddl = guis[id].CreateDropDownList();
                                    ddl.Contents = Param4;
                                    string opts = GuiApplyStyles(ddl, Param3);

                                    foreach (string opt in ParseOptions(opts))
                                    {
                                        bool on = opt[0] != '-';
                                        string mode = opt.Substring(!on || opt[0] == '+' ? 1 : 0).ToLowerInvariant();

                                        switch (mode)
                                        {
                                            case Keyword_Sort: ddl.Sort = on; break;
                                            case Keyword_Uppercase: ddl.Uppercase = on; break;
                                            case Keyword_Lowercase: ddl.Lowercase = on; break;

                                            default:
                                                if (mode.StartsWith(Keyword_Choose, StringComparison.OrdinalIgnoreCase))
                                                {
                                                    mode = mode.Substring(Keyword_Choose.Length);
                                                    int n;

                                                    if (int.TryParse(mode, out n))
                                                        ddl.Choose = n;
                                                }
                                                break;
                                        }
                                    }

                                    guis[id].Add(ddl);
                                }
                                break;

                            case Keyword_ComboBox:
                                {
                                    var combo = guis[id].CreateComboBox();
                                    combo.Contents = Param4;
                                    string opts = GuiApplyStyles(combo, Param3);

                                    foreach (string opt in ParseOptions(opts))
                                    {
                                        bool on = opt[0] != '-';
                                        string mode = opt.Substring(!on || opt[0] == '+' ? 1 : 0).ToLowerInvariant();

                                        switch (mode)
                                        {
                                            case Keyword_Limit: combo.Limit = on; break;
                                            case Keyword_Simple: combo.Simple = on; break;
                                        }
                                    }

                                    guis[id].Add(combo);
                                }
                                break;

                            case Keyword_ListBox:
                                {
                                    var listbox = guis[id].CreateListBox();
                                    listbox.Contents = Param4;
                                    string opts = GuiApplyStyles(listbox, Param3);

                                    foreach (string opt in ParseOptions(opts))
                                    {
                                        bool on = opt[0] != '-';
                                        string mode = opt.Substring(!on || opt[0] == '+' ? 1 : 0).ToLowerInvariant();

                                        switch (mode)
                                        {
                                            case Keyword_Multi:
                                            case "8":
                                                listbox.MultiSelect = on;
                                                break;

                                            case Keyword_Readonly: listbox.ReadOnly = on; break;
                                            case Keyword_Sort: listbox.Sort = on; break;

                                            default:
                                                if (mode.StartsWith(Keyword_Choose, StringComparison.OrdinalIgnoreCase))
                                                {
                                                    mode = mode.Substring(Keyword_Choose.Length);
                                                    int n;

                                                    if (int.TryParse(mode, out n))
                                                        listbox.Choose = n;
                                                }
                                                break;
                                        }
                                    }

                                    guis[id].Add(listbox);
                                }
                                break;

                            case Keyword_ListView:
                                {
                                    var lv = guis[id].CreateListView();
                                    lv.Contents = Param4;
                                    string opts = GuiApplyStyles(lv, Param3);

                                    foreach (string opt in ParseOptions(opts))
                                    {
                                        bool on = opt[0] != '-';
                                        string mode = opt.Substring(!on || opt[0] == '+' ? 1 : 0).ToLowerInvariant();

                                        switch (mode)
                                        {
                                            case Keyword_Checked: lv.Checklist = on; break;
                                            case Keyword_Grid: lv.Grid = on; break;
                                            case Keyword_Hdr: lv.Header = on; break;
                                            case "lv0x10": lv.MovableColumns = on; break;
                                            case "lv0x20": lv.ClickToSelect = on; break;
                                            case Keyword_Multi: lv.MultiSelect = on; break;
                                            case Keyword_NoSortHdr: lv.SortableHeader = !on; break;
                                            case Keyword_Readonly: lv.ReadOnly = on; break;
                                            case Keyword_Sort: lv.Sort = on; break;
                                            case Keyword_SortDesc: lv.SortDesc = on; break;
                                            case Keyword_WantF2: lv.WantF2 = on; break;
                                        }
                                    }

                                    guis[id].Add(lv);
                                }
                                break;

                            case Keyword_TreeView:
                                {
                                    var tree = guis[id].CreateTreeView();
                                    tree.Contents = Param4;
                                    string opts = GuiApplyStyles(tree, Param3);

                                    foreach (string opt in ParseOptions(opts))
                                    {
                                        bool on = opt[0] != '-';
                                        string mode = opt.Substring(!on || opt[0] == '+' ? 1 : 0).ToLowerInvariant();

                                        switch (mode)
                                        {
                                            case Keyword_Buttons: tree.Buttons = on; break;
                                            case Keyword_HScroll: tree.HorizontalScroll = on; break;
                                            case Keyword_Lines: tree.Lines = on; break;
                                            case Keyword_Readonly: tree.ReadOnly = on; break;
                                            case Keyword_WantF2: tree.WantF2 = on; break;

                                            default:
                                                if (mode.StartsWith(Keyword_ImageList))
                                                {
                                                    mode = mode.Substring(Keyword_ImageList.Length);
                                                    int n;

                                                    // UNDONE: TreeView control ImageList
                                                }
                                                break;
                                        }
                                    }

                                    guis[id].Add(tree);
                                }
                                break;

                            case Keyword_Hotkey:
                                {
                                    var hotkey = guis[id].CreateHotkey();
                                    hotkey.Contents = Param4;
                                    string opts = GuiApplyStyles(hotkey, Param3);

                                    foreach (string opt in ParseOptions(opts))
                                    {
                                        if (!opt.StartsWith(Keyword_Limit, StringComparison.OrdinalIgnoreCase))
                                            continue;

                                        int n;
                                        hotkey.Limit = int.TryParse(opt.Substring(Keyword_Limit.Length), out n) ? n : 0;
                                    }

                                    guis[id].Add(hotkey);
                                }
                                break;

                            case Keyword_DateTime:
                                {
                                    var date = guis[id].CreateDateTime();
                                    date.Contents = ToDateTime(Param4).Ticks.ToString();
                                    string opts = GuiApplyStyles(date, Param3);

                                    foreach (string opt in ParseOptions(opts))
                                    {
                                        bool on = opt[0] != '-';
                                        string mode = opt.Substring(!on || opt[0] == '+' ? 1 : 0).ToLowerInvariant();

                                        switch (mode)
                                        {
                                            case "1": date.UpDown = on; break;
                                            case "2": date.Checklist = on; break;
                                            case Keyword_Right: date.Right = on; break;
                                            case Keyword_LongDate: date.LongDate = on; break;
                                            case Keyword_Time: date.Time = on; break;

                                            default:
                                                if (mode.StartsWith(Keyword_Range))
                                                {
                                                    string[] range = mode.Substring(Keyword_Range.Length).Split(new[] { '-' }, 2);

                                                    date.RangeMinimum = ToDateTime(range[0]);

                                                    if (range.Length > 1)
                                                        date.RangeMaximum = ToDateTime(range[1]);
                                                }
                                                else if (mode.StartsWith(Keyword_Choose))
                                                {
                                                    mode = mode.Substring(Keyword_Choose.Length);
                                                    int n;

                                                    if (int.TryParse(mode, out n))
                                                        date.Choose = n;
                                                }
                                                break;
                                        }
                                    }

                                    guis[id].Add(date);
                                }
                                break;

                            case Keyword_MonthCal:
                                {
                                    var cal = guis[id].CreateMonthCal();
                                    cal.Contents = ToDateTime(Param4).Ticks.ToString();
                                    string opts = GuiApplyStyles(cal, Param3);

                                    foreach (string opt in ParseOptions(opts))
                                    {
                                        bool on = opt[0] != '-';
                                        string mode = opt.Substring(!on || opt[0] == '+' ? 1 : 0).ToLowerInvariant();

                                        switch (mode)
                                        {
                                            case "4": cal.DisplayWeek = on; break;
                                            case "8": cal.HightlightToday = !on; break;
                                            case "16": cal.DisplayToday = !on; break;
                                            case Keyword_Multi: cal.MultiSelect = on; break;

                                            default:
                                                if (mode.StartsWith(Keyword_Range, StringComparison.OrdinalIgnoreCase))
                                                {
                                                    string[] range = mode.Substring(Keyword_Range.Length).Split(new[] { '-' }, 2);

                                                    cal.RangeMinimum = ToDateTime(range[0]);

                                                    if (range.Length > 1)
                                                        cal.RangeMaximum = ToDateTime(range[1]);
                                                }
                                                break;
                                        }
                                    }

                                    guis[id].Add(cal);
                                }
                                break;

                            case Keyword_Slider:
                                {
                                    var slider = guis[id].CreateSider();
                                    slider.Contents = Param4;
                                    string opts = GuiApplyStyles(slider, Param3);

                                    foreach (string opt in ParseOptions(opts))
                                    {
                                        bool on = opt[0] != '-';
                                        string mode = opt.Substring(!on || opt[0] == '+' ? 1 : 0).ToLowerInvariant();

                                        switch (mode)
                                        {
                                            case Keyword_Center: slider.Rounded = on; break;
                                            case Keyword_Invert: slider.Invert = on; break;
                                            case Keyword_Left: slider.Left = on; break;
                                            case Keyword_NoTicks: slider.Tick = !on; break;
                                            case Keyword_Thick: slider.Thick = on; break;
                                            case Keyword_Vertical: slider.Vertical = on; break;

                                            default:
                                                int n;
                                                if (mode.StartsWith(Keyword_Line))
                                                {
                                                    mode = mode.Substring(Keyword_Line.Length);

                                                    if (int.TryParse(mode, out n))
                                                        slider.Line = n;
                                                }
                                                else if (mode.StartsWith(Keyword_Page))
                                                {
                                                    mode = mode.Substring(Keyword_Page.Length);

                                                    if (int.TryParse(mode, out n))
                                                        slider.Page = n;
                                                }
                                                else if (mode.StartsWith(Keyword_Range))
                                                {
                                                    mode = mode.Substring(Keyword_Range.Length);
                                                    string[] parts = mode.Split(new[] { '-' }, 2);

                                                    if (int.TryParse(parts[0], out n))
                                                        slider.RangeMinimum = n;

                                                    if (parts.Length > 1 && int.TryParse(parts[1], out n))
                                                        slider.RangeMaximum = n;
                                                }
                                                else if (mode.StartsWith(Keyword_TickInterval))
                                                {
                                                    mode = mode.Substring(Keyword_TickInterval.Length);

                                                    if (int.TryParse(mode, out n))
                                                        slider.TickInterval = n;
                                                }
                                                else if (mode.StartsWith(Keyword_ToolTip))
                                                {
                                                    mode = mode.Substring(Keyword_ToolTip.Length);

                                                    switch (mode)
                                                    {
                                                        case Keyword_Left: slider.ToolTip = 0; break;
                                                        case Keyword_Right: slider.ToolTip = 1; break;
                                                        case Keyword_Top: slider.ToolTip = 2; break;
                                                        case Keyword_Bottom: slider.ToolTip = 3; break;
                                                    }
                                                }
                                                break;
                                        }
                                    }

                                    guis[id].Add(slider);
                                }
                                break;

                            case Keyword_Progress:
                                {
                                    var progress = guis[id].CreateProgress();
                                    progress.Contents = Param4;
                                    string opts = GuiApplyStyles(progress, Param3);

                                    foreach (string opt in ParseOptions(opts))
                                    {
                                        bool on = opt[0] != '-';
                                        string mode = opt.Substring(!on || opt[0] == '+' ? 1 : 0).ToLowerInvariant();

                                        switch (mode)
                                        {
                                            case Keyword_Smooth: progress.Smooth = on; break;
                                            case Keyword_Vertical: progress.Vertical = on; break;

                                            default:
                                                if (mode.StartsWith(Keyword_Range))
                                                {
                                                    mode = mode.Substring(Keyword_Range.Length);
                                                    int z = mode.IndexOf('-');
                                                    string a = mode, b;

                                                    if (z == -1)
                                                        b = string.Empty;
                                                    else
                                                    {
                                                        a = mode.Substring(0, z);
                                                        z++;
                                                        b = z == mode.Length ? string.Empty : mode.Substring(z);
                                                    }

                                                    int x, y;

                                                    if (int.TryParse(a, out x) && int.TryParse(b, out y))
                                                    {
                                                        progress.RangeMinimum = x;
                                                        progress.RangeMaximum = y;
                                                    }
                                                }
                                                else if (mode.StartsWith(Keyword_Background))
                                                {
                                                    mode = mode.Substring(Keyword_Background.Length);
                                                    progress.BackgroundColor = ParseColor(mode);
                                                }
                                                break;
                                        }
                                    }

                                    guis[id].Add(progress);
                                }
                                break;

                            case Keyword_GroupBox:
                                {
                                    var group = guis[id].CreateGroupBox();
                                    group.Contents = Param4;
                                    GuiApplyStyles(group, Param3);
                                    guis[id].Add(group);
                                }
                                break;

                            case Keyword_Tab:
                            case Keyword_Tab2:
                                {
                                    var tab = guis[id].CreateTab();
                                    tab.Contents = Param4;
                                    string opts = GuiApplyStyles(tab, Param3);

                                    foreach (string opt in ParseOptions(opts))
                                    {
                                        bool on = opt[0] != '-';
                                        string mode = opt.Substring(!on || opt[0] == '+' ? 1 : 0).ToLowerInvariant();

                                        switch (mode)
                                        {
                                            case Keyword_Background: tab.Background = on; break;
                                            case Keyword_Buttons: tab.Buttons = on; break;
                                            case Keyword_Top: tab.Align = 0; break;
                                            case Keyword_Left: tab.Align = 1; break;
                                            case Keyword_Right: tab.Align = 2; break;
                                            case Keyword_Bottom: tab.Align = 3; break;
                                            case Keyword_Wrap: tab.Wrap = on; break;

                                            default:
                                                if (mode.StartsWith(Keyword_Choose, StringComparison.OrdinalIgnoreCase))
                                                {
                                                    mode = mode.Substring(Keyword_Choose.Length);
                                                    int n;

                                                    if (int.TryParse(mode, out n))
                                                        tab.Choose = n;
                                                }
                                                break;
                                        }
                                    }

                                    guis[id].Add(tab);
                                }
                                break;

                            case Keyword_StatusBar:
                                {
                                    var status = guis[id].CreateStatusBar();
                                    status.Contents = Param4;
                                    GuiApplyStyles(status, Param3);
                                    guis[id].Add(status);
                                    guis[id].StatusBar = status;
                                }
                                break;

                            case Keyword_WebBrowser:
                                {
                                    var web = guis[id].CreateWebBrowser();
                                    web.Contents = Param4;
                                    GuiApplyStyles(web, Param3);
                                    guis[id].Add(web);
                                }
                                break;
                        }
                    }
                    break;

                #endregion

                #region Show

                case Keyword_Show:
                    {
                        bool center = false, cX = false, cY = false, auto = false, min = false, max = false, restore = false, hide = false;
                        int?[] pos = { null, null, null, null };

                        foreach (string option in ParseOptions(Param2))
                        {
                            string mode = option.ToLowerInvariant();
                            int select = -1;

                            switch (mode[0])
                            {
                                case 'w': select = 0; break;
                                case 'h': select = 1; break;
                                case 'x': select = 2; break;
                                case 'y': select = 3; break;
                            }

                            if (select == -1)
                            {
                                switch (mode)
                                {
                                    case Keyword_Center: center = true; break;
                                    case Keyword_AutoSize: auto = true; break;
                                    case Keyword_Maximize: max = true; break;
                                    case Keyword_Minimize: min = true; break;
                                    case Keyword_Restore: restore = true; break;
                                    case Keyword_NoActivate: break;
                                    case Keyword_NA: break;
                                    case Keyword_Hide: hide = true; break;
                                }
                            }
                            else
                            {
                                mode = mode.Substring(1);
                                int n;

                                if (mode.Equals(Keyword_Center, StringComparison.OrdinalIgnoreCase))
                                {
                                    if (select == 2)
                                        cX = true;
                                    else
                                        cY = true;
                                }
                                else if (mode.Length != 0 && int.TryParse(mode, out n))
                                    pos[select] = n;
                            }
                        }

                        if (auto)
                            guis[id].AutoSize();
                        else
                        {
                            var size = guis[id].Size;

                            if (pos[0] != null)
                                size.Width = (int)pos[0];
                            if (pos[1] != null)
                                size.Height = (int)pos[1];

                            guis[id].Size = size;
                        }

                        var location = guis[id].Location;

                        if (pos[2] != null)
                            location.X = (int)pos[2];
                        if (pos[3] != null)
                            location.Y = (int)pos[3];

                        var screen = Screen.PrimaryScreen.Bounds;

                        if (center)
                            cX = cY = true;

                        if (cX)
                            location.X = (screen.Width - guis[id].Size.Width) / 2 + screen.X;
                        if (cY)
                            location.Y = (screen.Height - guis[id].Size.Height) / 2 + screen.Y;

                        guis[id].Location = location;

                        guis[id].Draw(Param3);

                        if (min)
                            guis[id].Minimise();
                        else if (max)
                            guis[id].Maximise();
                        else if (restore)
                            guis[id].Restore();
                        else if (hide)
                            guis[id].Cancel();
                        else
                            guis[id].Show();
                    }
                    break;

                #endregion

                #region Misc.

                case Keyword_Submit:
                    {
                        var table = guis[id].Submit(!Keyword_NoHide.Equals(Param2, StringComparison.OrdinalIgnoreCase));

                        foreach (string key in table.Keys)
                            SetEnv("." + key, table[key]); // TODO: variable scoping with gui,submit
                    }
                    break;

                case Keyword_Cancel:
                case Keyword_Hide:
                    guis[id].Cancel();
                    break;

                case Keyword_Destroy:
                    guis[id].Destroy();
                    guis.Remove(id);
                    break;

                case Keyword_Font:
                    guis[id].Font = ParseFont(Param3, Param2);
                    break;

                case Keyword_Color:
                    guis[id].WindowColour = Keyword_Default.Equals(Param2, StringComparison.OrdinalIgnoreCase) ? Color.Transparent : ParseColor(Param2);
                    guis[id].ControlColour = Keyword_Default.Equals(Param3, StringComparison.OrdinalIgnoreCase) ? Color.Transparent : ParseColor(Param3);
                    break;

                case Keyword_Margin:
                    {
                        int d, x = guis[id].Margin.X, y = guis[id].Margin.Y;

                        if (int.TryParse(Param2, out d))
                            x = d;

                        if (int.TryParse(Param3, out d))
                            y = d;

                        guis[id].Margin = new System.Drawing.Point(x, y);
                    }
                    break;

                case Keyword_Menu:
                    break;

                case Keyword_Minimize:
                    guis[id].Minimise();
                    break;

                case Keyword_Maximize:
                    guis[id].Maximise();
                    break;

                case Keyword_Restore:
                    guis[id].Restore();
                    break;

                case Keyword_Flash:
                    guis[id].Flash(OnOff(Param2) ?? true);
                    break;

                case Keyword_Default:
                    DefaultGuiId = id;
                    break;

                #endregion

                #region Options

                default:
                    {
                        foreach (string option in ParseOptions(Command))
                        {
                            bool on = option[0] != '-';
                            string mode = option;

                            if (mode[0] == '+' || mode[0] == '-')
                                mode = mode.Substring(1);

                            if (mode.Length == 0)
                                continue;

                            mode = mode.ToLowerInvariant();

                            switch (mode)
                            {
                                case Keyword_AlwaysOnTop: guis[id].AlwaysOnTop = on; break;
                                case Keyword_Border: guis[id].Border = on; break;
                                case Keyword_Caption: guis[id].Caption = on; break;
                                case Keyword_Disabled: guis[id].Enabled = !on; break;
                                case Keyword_LastFound: guis[id].LastFound = on; break;
                                case Keyword_LastFoundExist: guis[id].LastFoundExist = on; break;
                                case Keyword_MaximizeBox: guis[id].MaximiseBox = on; break;
                                case Keyword_MinimizeBox: guis[id].MinimiseBox = on; break;
                                case Keyword_OwnDialogs: guis[id].OwnDialogs = on; break;
                                case Keyword_Owner: guis[id].Owner = on; break;
                                case Keyword_Resize: guis[id].Resize = on; break;
                                case Keyword_SysMenu: guis[id].SysMenu = on; break;
                                case Keyword_Theme: guis[id].Theme = on; break;
                                case Keyword_ToolWindow: guis[id].ToolWindow = on; break;

                                default:
                                    string arg;
                                    string[] parts;
                                    int n;
                                    Size size;
                                    if (mode.StartsWith(Keyword_Delimiter))
                                    {
                                        arg = mode.Substring(Keyword_Delimiter.Length);
                                        if (arg.Length > 0)
                                            guis[id].Delimieter = arg[0];
                                    }
                                    else if (mode.StartsWith(Keyword_Label))
                                    {
                                        arg = mode.Substring(Keyword_Label.Length);
                                        if (arg.Length > 0)
                                            guis[id].Label = arg;
                                    }
                                    else if (mode.StartsWith(Keyword_MinSize))
                                    {
                                        arg = mode.Substring(Keyword_MinSize.Length);
                                        parts = arg.Split(new[] { 'x', 'X', '*' }, 2);
                                        size = guis[id].MinimumSize;

                                        if (parts.Length > 0 && int.TryParse(parts[0], out n))
                                            size.Width = n;
                                        if (parts.Length > 1 && int.TryParse(parts[1], out n))
                                            size.Height = n;

                                        guis[id].MinimumSize = size;
                                    }
                                    else if (mode.StartsWith(Keyword_MaxSize))
                                    {
                                        arg = mode.Substring(Keyword_MaxSize.Length);
                                        parts = arg.Split(new[] { 'x', 'X', '*' }, 2);
                                        size = guis[id].MaximumSize;

                                        if (parts.Length > 0 && int.TryParse(parts[0], out n))
                                            size.Width = n;
                                        if (parts.Length > 1 && int.TryParse(parts[1], out n))
                                            size.Height = n;

                                        guis[id].MaximumSize = size;
                                    }
                                    break;
                            }
                        }
                    }
                    break;

                #endregion
            }
        }

        #region Helpers

        static BaseGui.Window GuiCreateWindow(string name)
        {
            var win = new WinForms.Window();

            if (name != "1")
                win.Label = name + win.Label;

            win.Closed += new EventHandler<BaseGui.Window.ClosedArgs>(delegate(object sender, BaseGui.Window.ClosedArgs e)
            {
                SafeInvoke(win.Label + Keyword_GuiClose);
            });

            win.Escaped += new EventHandler<BaseGui.Window.EscapedArgs>(delegate(object sender, BaseGui.Window.EscapedArgs e)
            {
                SafeInvoke(win.Label + Keyword_GuiEscape);
            });

            win.Resized += new EventHandler<BaseGui.Window.ResizedArgs>(delegate(object sender, BaseGui.Window.ResizedArgs e)
            {
                eventinfo = error = e.Mode;
                SafeInvoke(win.Label + Keyword_GuiSize);
            });

            return win;
        }

        static string GuiApplyStyles(BaseGui.Control control, string styles)
        {
            string[] opts = ParseOptions(styles), excess = new string[opts.Length];

            for (int i = 0; i < opts.Length; i++)
            {
                string mode = opts[i].ToLowerInvariant();
                bool append = false;

                bool on = mode[0] != '-';
                if (!on || mode[0] == '+')
                    mode = mode.Substring(1);

                if (mode.Length == 0)
                    continue;

                string arg = mode.Substring(1);
                int n;

                switch (mode)
                {
                    case Keyword_Left:
                        control.Alignment = ContentAlignment.MiddleLeft;
                        break;

                    case Keyword_Center:
                        control.Alignment = ContentAlignment.MiddleCenter;
                        break;

                    case Keyword_Right:
                        control.Alignment = ContentAlignment.MiddleRight;
                        break;

                    case Keyword_AltSubmit:
                        control.AltSubmit = on;
                        break;

                    case Keyword_Background:
                        control.Background = on;
                        break;

                    case Keyword_Border:
                        control.Border = on;
                        break;

                    case Keyword_Enabled:
                        control.Enabled = on;
                        break;

                    case Keyword_Disabled:
                        control.Enabled = !on;
                        break;

                    case Keyword_HScroll:
                        control.HorizontalScroll = on;
                        break;

                    case Keyword_VScroll:
                        control.VerticalScroll = on;
                        break;

                    case Keyword_TabStop:
                        control.TabStop = on;
                        break;

                    case Keyword_Theme:
                        control.Theme = on;
                        break;

                    case Keyword_Transparent:
                        control.Transparent = on;
                        break;

                    case Keyword_Visible:
                    case Keyword_Vis:
                        control.Visible = on;
                        break;

                    case Keyword_Wrap:
                        control.Wrap = on;
                        break;

                    default:
                        switch (mode[0])
                        {
                            case 'x':
                            case 'y':
                            case 'w':
                            case 'h':
                                GuiControlMove(mode, control);
                                break;

                            case 'r':
                                if (int.TryParse(arg, out n))
                                {
                                    if (control.Parent != null && control.Parent.Font != null)
                                        control.Size = new Size(control.Size.Width, (int)(n * control.Parent.Font.GetHeight()));
                                }
                                else
                                    append = true;
                                break;

                            case 'c':
                                if (arg.Length != 0 &&
                                    !mode.StartsWith(Keyword_Check, StringComparison.OrdinalIgnoreCase) &&
                                    !mode.StartsWith(Keyword_Choose, StringComparison.OrdinalIgnoreCase))
                                    control.Colour = ParseColor(arg);
                                else
                                    append = true;
                                break;

                            case 'v':
                                control.Id = arg;
                                break;

                            default:
                                append = true;
                                break;
                        }
                        break;
                }

                if (append)
                    excess[i] = opts[i];
            }

            return string.Join(Keyword_Spaces[1].ToString(), excess).Trim();
        }

        static void GuiApplyExtendedStyles(BaseGui.Control control, string type, string styles)
        {
            // UNDONE: apply extended gui control styles
        }

        static void GuiControlMove(string mode, BaseGui.Control control)
        {
            if (mode.Length < 2)
                return;

            bool alt = false, offset = false;
            string arg;
            int d;

            switch (mode[0])
            {
                case 'x':
                case 'X':
                    {
                        offset = true;
                        int x = 0;

                        switch (mode[1])
                        {
                            case 's':
                            case 'S':
                                // TODO: sectional positioning for gui controls
                                break;

                            case 'm':
                            case 'M':
                                x = alt ? control.Parent.Margin.Y : control.Parent.Margin.X;
                                break;

                            case '+':
                                {
                                    int n = control.Parent.Controls.Count - 2;

                                    if (n < 0)
                                        return;

                                    var s = control.Parent.Controls[n].Location;
                                    x = alt ? s.Y : s.X;
                                }
                                break;

                            default:
                                offset = false;
                                break;
                        }

                        arg = mode.Substring(offset ? 2 : 1);

                        if (!int.TryParse(arg, out d))
                            return;

                        d += x;

                        if (alt)
                            control.Location = new Point(control.Location.X, d);
                        else
                            control.Location = new Point(d, control.Location.Y);
                    }
                    break;

                case 'y':
                case 'Y':
                    alt = true;
                    goto case 'x';

                case 'w':
                case 'W':
                    {
                        offset = mode[1] == 'p' || mode[1] == 'P';
                        arg = mode.Substring(offset ? 2 : 1);

                        if (!int.TryParse(arg, out d))
                            return;

                        if (offset)
                        {
                            int n = control.Parent.Controls.Count - 2;
                            
                            if (n < 0)
                                return;

                            var s = control.Parent.Controls[n].Size;
                            d += alt ? s.Height : s.Width;
                        }

                        if (alt)
                            control.Size = new Size(control.Size.Width, d);
                        else
                            control.Size = new Size(d, control.Size.Height);
                    }
                    break;

                case 'h':
                case 'H':
                    alt = true;
                    goto case 'w';
            }
        }

        static BaseGui.Control GuiFindControl(string name)
        {
            return GuiFindControl(name, DefaultGui);
        }

        static BaseGui.Control GuiFindControl(string name, BaseGui.Window gui)
        {
            if (gui == null)
                return null;

            foreach (var control in gui.Controls)
                if (control.Id.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return control;

            return null;
        }

        #endregion

        /// <summary>
        /// Makes a variety of changes to a control in a GUI window.
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="ControlID"></param>
        /// <param name="Param3"></param>
        public static void GuiControl(string Command, string ControlID, string Param3)
        {
            var ctrl = GuiFindControl(ControlID);

            if (ctrl == null)
                return;

            Command = Command.ToLowerInvariant();

            switch (Command)
            {
                case Keyword_Text:
                case "":
                    ctrl.Contents = Param3;
                    break;

                case Keyword_Move:
                case Keyword_MoveDraw:
                    GuiControlMove(Param3, ctrl);
                    break;

                case Keyword_Focus:
                    ctrl.Parent.Focus(ctrl);
                    break;

                case Keyword_Enable:
                    ctrl.Enabled = true;
                    break;

                case Keyword_Disable:
                    ctrl.Enabled = false;
                    break;

                case Keyword_Hide:
                    ctrl.Visible = false;
                    break;

                case Keyword_Show:
                    ctrl.Visible = true;
                    break;

                case Keyword_Delete:
                    ctrl.Parent.Remove(ctrl);
                    break;

                case Keyword_Choose:
                    // UNDONE: choose item for gui control
                    break;

                case Keyword_Font:
                    ctrl.Parent.ChangeFont(ctrl);
                    break;

                default:
                    int n;
                    if (Command.StartsWith(Keyword_Enable) && int.TryParse(Command.Substring(Keyword_Enable.Length), out n) && (n == 1 || n == 0))
                        ctrl.Enabled = n == 1;
                    if (Command.StartsWith(Keyword_Disable) && int.TryParse(Command.Substring(Keyword_Disable.Length), out n) && (n == 1 || n == 0))
                        ctrl.Enabled = n == 0;
                    GuiApplyExtendedStyles(ctrl, ctrl.GetType().Name, Param3);
                    break;
            }
        }

        /// <summary>
        /// Retrieves various types of information about a control in a GUI window.
        /// </summary>
        /// <param name="OutputVar"></param>
        /// <param name="Command"></param>
        /// <param name="ControlID"></param>
        /// <param name="Param4"></param>
        public static void GuiControlGet(out object OutputVar, string Command, string ControlID, string Param4)
        {
            OutputVar = null;

            var ctrl = GuiFindControl(ControlID);

            if (ctrl == null)
                return;

            Command = Command.ToLowerInvariant();

            switch (Command)
            {
                case Keyword_Text:
                case "":
                    OutputVar = ctrl.Contents;
                    break;

                case Keyword_Pos:
                    {
                        var loc = new Dictionary<string, object>();
                        loc.Add("x", ctrl.Location.X);
                        loc.Add("y", ctrl.Location.Y);
                        loc.Add("w", ctrl.Size.Width);
                        loc.Add("h", ctrl.Size.Height);
                        OutputVar = loc;
                    }
                    break;

                case Keyword_Focus:
                case Keyword_Focus + "V":
                    // UNDONE: get focued Gui control
                    break;

                case Keyword_Enabled:
                    OutputVar = ctrl.Enabled ? 1 : 0;
                    break;

                case Keyword_Visible:
                    OutputVar = ctrl.Visible ? 1 : 0;
                    break;

                case Keyword_Hwnd:
                    break;
            }
        }

        /// <summary>
        /// Creates, deletes, modifies and displays menus and menu items.
        /// </summary>
        /// <param name="MenuName"></param>
        /// <param name="Cmd"></param>
        /// <param name="P3"></param>
        /// <param name="P4"></param>
        /// <param name="P5"></param>
        public static void Menu(string MenuName, string Cmd, string P3, string P4, string P5)
        {

        }
    }
}
