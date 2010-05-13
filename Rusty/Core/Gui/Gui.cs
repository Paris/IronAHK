using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
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
                guis = new Dictionary<string, Form>();

            string id = GuiId(ref Command);

            if (!guis.ContainsKey(id))
                guis.Add(id, GuiCreateWindow(id));

            switch (Command.ToLowerInvariant())
            {
                #region Add

                case Keyword_Add:
                    {
                        Control control = null;
                        GuiControlEdit(ref control, Param2, Param3, Param4);
                        guis[id].Controls.Add(control);
                        GuiApplyInitialStyles(control);
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
                            guis[id].AutoSize = true;
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

                        guis[id].ResumeLayout(true);

                        if (min)
                            guis[id].WindowState = FormWindowState.Minimized;
                        else if (max)
                            guis[id].WindowState = FormWindowState.Maximized;
                        else if (restore)
                            guis[id].WindowState = FormWindowState.Normal;
                        else if (hide)
                            guis[id].Hide();
                        else
                            guis[id].Show();
                    }
                    break;

                #endregion

                #region Misc.

                case Keyword_Submit:
                    {
                        if (!Keyword_NoHide.Equals(Param2, StringComparison.OrdinalIgnoreCase))
                            guis[id].Hide();

                        foreach (Control ctrl in guis[id].Controls)
                            SetEnv("." + ctrl.Name, ctrl.Text);
                    }
                    break;

                case Keyword_Cancel:
                case Keyword_Hide:
                    guis[id].Hide();
                    break;

                case Keyword_Destroy:
                    guis[id].Hide();
                    guis[id].Dispose();
                    guis.Remove(id);
                    break;

                case Keyword_Font:
                    guis[id].Font = ParseFont(Param3, Param2);
                    break;

                case Keyword_Color:
                    guis[id].BackColor = Keyword_Default.Equals(Param2, StringComparison.OrdinalIgnoreCase) ? Color.Transparent : ParseColor(Param2);
                    guis[id].ForeColor = Keyword_Default.Equals(Param3, StringComparison.OrdinalIgnoreCase) ? Color.Transparent : ParseColor(Param3);
                    break;

                case Keyword_Margin:
                    {
                        int d, x = guis[id].Margin.Left, y = guis[id].Margin.Top;

                        if (int.TryParse(Param2, out d))
                            x = d;

                        if (int.TryParse(Param3, out d))
                            y = d;

                        guis[id].Margin = new Padding(x, y, guis[id].Margin.Right, guis[id].Margin.Bottom);
                    }
                    break;

                case Keyword_Menu:
                    break;

                case Keyword_Minimize:
                    guis[id].WindowState = FormWindowState.Minimized;
                    break;

                case Keyword_Maximize:
                    guis[id].WindowState = FormWindowState.Maximized;
                    break;

                case Keyword_Restore:
                    guis[id].WindowState = FormWindowState.Normal;
                    break;

                case Keyword_Flash:
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                        Windows.FlashWindow(guis[id].Handle, OnOff(Param2) ?? true);
                    break;

                case Keyword_Default:
                    DefaultGuiId = id;
                    break;

                case Keyword_TreeView:
                    {
                        var tree = GuiFindControl(Param2, guis[id]);

                        if (tree == null || !typeof(TreeView).IsAssignableFrom(tree.GetType()))
                            DefaultTreeView = null;
                        else
                            DefaultTreeView = (TreeView)tree;
                    }
                    break;

                case Keyword_ListView:
                    {
                        var list = GuiFindControl(Param2, guis[id]);

                        if (list == null || !typeof(ListView).IsAssignableFrom(list.GetType()))
                            DefaultListView = null;
                        else
                            DefaultListView = (ListView)list;
                    }
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
                                case Keyword_AlwaysOnTop: guis[id].TopMost = on; break;
                                case Keyword_Border: break;
                                case Keyword_Caption: break;
                                case Keyword_Disabled: guis[id].Enabled = !on; break;
                                case Keyword_LastFound: break;
                                case Keyword_LastFoundExist: break;
                                case Keyword_MaximizeBox: guis[id].MaximizeBox = on; break;
                                case Keyword_MinimizeBox: guis[id].MinimizeBox = on; break;
                                case Keyword_OwnDialogs: ((GuiInfo)guis[id].Tag).OwnDialogs = on; break;
                                case Keyword_Owner: break;
                                case Keyword_Resize: break;
                                case Keyword_SysMenu: break;
                                case Keyword_Theme: Application.EnableVisualStyles(); break;
                                case Keyword_ToolWindow: break;

                                default:
                                    string arg;
                                    string[] parts;
                                    int n;
                                    Size size;
                                    if (mode.StartsWith(Keyword_Delimiter))
                                    {
                                        arg = mode.Substring(Keyword_Delimiter.Length);
                                        if (arg.Length > 0)
                                            ((GuiInfo)guis[id].Tag).Delimiter = arg[0];
                                    }
                                    else if (mode.StartsWith(Keyword_Label))
                                    {
                                        arg = mode.Substring(Keyword_Label.Length);
                                        if (arg.Length > 0)
                                            guis[id].Name = arg;
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

        static void GuiControlEdit(ref Control control, string type, string options, string content)
        {
            string opts = null;

            switch (type.ToLowerInvariant())
            {
                #region Text
                case Keyword_Text:
                    {
                        var text = (Label)(control ?? new Label());
                        control = text;
                        text.Text = content;
                    }
                    break;
                #endregion

                #region Edit
                case Keyword_Edit:
                    {
                        var edit = (TextBox)(control ?? new TextBox());
                        control = edit;
                        edit.Text = content;
                        opts = GuiApplyStyles(edit, options);

                        foreach (string opt in ParseOptions(opts))
                        {
                            bool on = opt[0] != '-';
                            string mode = opt.Substring(!on || opt[0] == '+' ? 1 : 0).ToLowerInvariant();

                            switch (mode)
                            {
                                case Keyword_Limit:
                                    if (!on)
                                        edit.MaxLength = int.MaxValue;
                                    break;

                                case Keyword_Lowercase: edit.CharacterCasing = on ? CharacterCasing.Lower : CharacterCasing.Normal; break;
                                case Keyword_Multi: edit.Multiline = on; break;
                                case Keyword_Number: break;
                                case Keyword_Password: edit.PasswordChar = '●'; break;
                                case Keyword_Readonly: edit.ReadOnly = on; break;
                                case Keyword_Uppercase: edit.CharacterCasing = on ? CharacterCasing.Upper : CharacterCasing.Normal; break;
                                case Keyword_WantCtrlA: break;
                                case Keyword_WantReturn: edit.AcceptsReturn = on; break;
                                case Keyword_WantTab: edit.AcceptsTab = on; break;
                                case Keyword_Wrap: edit.WordWrap = on; break;

                                default:
                                    int n;
                                    if (mode.StartsWith(Keyword_Limit) && int.TryParse(mode.Substring(Keyword_Limit.Length), out n))
                                        edit.MaxLength = n;
                                    break;
                            }
                        }
                    }
                    break;
                #endregion

                #region UpDown
                case Keyword_UpDown:
                    {
                        var updown = (NumericUpDown)(control ?? new NumericUpDown());
                        control = updown;
                        updown.Value = decimal.Parse(content);
                        opts = GuiApplyStyles(updown, options);

                        foreach (string opt in ParseOptions(opts))
                        {
                            bool on = opt[0] != '-';
                            string mode = opt.Substring(!on || opt[0] == '+' ? 1 : 0).ToLowerInvariant();

                            switch (mode)
                            {
                                case Keyword_Horz: break;
                                case Keyword_Left: break;
                                case Keyword_Wrap: break;
                                case "16": break;
                                case "0x80": break;

                                default:
                                    if (mode.StartsWith(Keyword_Range))
                                    {
                                        string[] range = mode.Substring(Keyword_Range.Length).Split(new[] { '-' }, 2);
                                        decimal n;

                                        if (decimal.TryParse(range[0], out n))
                                            updown.Minimum = n;

                                        if (range.Length > 1 && decimal.TryParse(range[1], out n))
                                            updown.Maximum = n;
                                    }
                                    break;
                            }
                        }
                    }
                    break;
                #endregion

                #region Picture
                case Keyword_Picture:
                case Keyword_Pic:
                    {
                        var pic = (PictureBox)(control ?? new PictureBox());
                        control = pic;
                        bool exists = File.Exists(content);

                        if (exists)
                            pic.ImageLocation = content;
                        
                        GuiApplyStyles(pic, options);

                        if (exists && pic.Size.IsEmpty)
                        {
                            var image = Image.FromFile(pic.ImageLocation);
                            pic.Size = image.Size;
                        }
                    }
                    break;
                #endregion

                #region Button
                case Keyword_Button:
                    {
                        var button = (Button)(control ?? new Button());
                        control = button;
                        button.Text = content;
                    }
                    break;
                #endregion

                #region CheckBox
                case Keyword_CheckBox:
                    {
                        var check = (CheckBox)(control ?? new CheckBox());
                        control = check;
                        check.Text = content;
                        opts = GuiApplyStyles(check, options);

                        foreach (string opt in ParseOptions(opts))
                        {
                            switch (opt.ToLowerInvariant())
                            {
                                case Keyword_Check3:
                                case Keyword_CheckedGray:
                                    check.CheckState = CheckState.Indeterminate;
                                    break;

                                case Keyword_Checked:
                                    check.CheckState = CheckState.Checked;
                                    break;

                                default:
                                    if (opt.StartsWith(Keyword_Checked, StringComparison.OrdinalIgnoreCase))
                                    {
                                        string arg = opt.Substring(Keyword_Checked.Length).Trim();
                                        int n;

                                        if (int.TryParse(arg, out n))
                                            check.CheckState = n == -1 ? CheckState.Indeterminate : n == 1 ? CheckState.Checked : CheckState.Unchecked;
                                    }
                                    break;
                            }
                        }
                    }
                    break;
                #endregion

                #region Radio
                case Keyword_Radio:
                    {
                        var radio = (RadioButton)(control ?? new RadioButton());
                        control = radio;
                        radio.Text = content;
                        radio.Checked = false;
                        opts = GuiApplyStyles(radio, options);

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
                    }
                    break;
                #endregion

                #region DropDownList
                case Keyword_DropDownList:
                case Keyword_DDL:
                    {
                        var ddl = (ComboBox)(control ?? new ComboBox());
                        control = ddl;
                        ddl.Text = content;
                        opts = GuiApplyStyles(ddl, options);

                        foreach (string opt in ParseOptions(opts))
                        {
                            bool on = opt[0] != '-';
                            string mode = opt.Substring(!on || opt[0] == '+' ? 1 : 0).ToLowerInvariant();

                            switch (mode)
                            {
                                case Keyword_Sort: ddl.Sorted = on; break;
                                case Keyword_Uppercase: ddl.Text = ddl.Text.ToUpperInvariant(); break;
                                case Keyword_Lowercase: ddl.Text = ddl.Text.ToLowerInvariant(); break;

                                default:
                                    if (mode.StartsWith(Keyword_Choose, StringComparison.OrdinalIgnoreCase))
                                    {
                                        mode = mode.Substring(Keyword_Choose.Length);
                                        int n;

                                        if (int.TryParse(mode, out n) && n > -1 && n < ddl.Items.Count)
                                            ddl.SelectedIndex = n;
                                    }
                                    break;
                            }
                        }
                    }
                    break;
                #endregion

                #region ComboBox
                case Keyword_ComboBox:
                    {
                        var combo = (ComboBox)(control ?? new ComboBox());
                        control = combo;
                        combo.Text = content;
                        opts = GuiApplyStyles(combo, options);

                        foreach (string opt in ParseOptions(opts))
                        {
                            bool on = opt[0] != '-';
                            string mode = opt.Substring(!on || opt[0] == '+' ? 1 : 0).ToLowerInvariant();

                            switch (mode)
                            {
                                case Keyword_Limit: break;
                                case Keyword_Simple: break;
                            }
                        }
                    }
                    break;
                #endregion

                #region ListBox
                case Keyword_ListBox:
                    {
                        var listbox = new ListBox();
                        listbox.Text = content;
                        opts = GuiApplyStyles(listbox, options);

                        bool multi = false, read = false;

                        foreach (string opt in ParseOptions(opts))
                        {
                            bool on = opt[0] != '-';
                            string mode = opt.Substring(!on || opt[0] == '+' ? 1 : 0).ToLowerInvariant();

                            switch (mode)
                            {
                                case Keyword_Multi:
                                case "8":
                                    multi = on;
                                    break;

                                case Keyword_Readonly: read = on; break;
                                case Keyword_Sort: listbox.Sorted = on; break;

                                default:
                                    if (mode.StartsWith(Keyword_Choose, StringComparison.OrdinalIgnoreCase))
                                    {
                                        mode = mode.Substring(Keyword_Choose.Length);
                                        int n;

                                        if (int.TryParse(mode, out n) && n > -1 && n < listbox.Items.Count)
                                            listbox.SelectedIndex = n;
                                    }
                                    break;
                            }
                        }

                        listbox.SelectionMode = multi ? SelectionMode.MultiExtended : read ? SelectionMode.None : SelectionMode.One;
                    }
                    break;
                #endregion

                #region ListView
                case Keyword_ListView:
                    {
                        var lv = (ListView)(control ?? new ListView());
                        control = lv;
                        opts = GuiApplyStyles(lv, options);

                        foreach (string opt in ParseOptions(opts))
                        {
                            bool on = opt[0] != '-';
                            string mode = opt.Substring(!on || opt[0] == '+' ? 1 : 0).ToLowerInvariant();

                            switch (mode)
                            {
                                case Keyword_Checked: lv.CheckBoxes = on; break;
                                case Keyword_Grid: lv.GridLines = on; break;
                                case Keyword_Hdr: break;
                                case "lv0x10": break;
                                case "lv0x20": break;
                                case Keyword_Multi: lv.MultiSelect = on; break;
                                case Keyword_NoSortHdr: break;
                                case Keyword_Readonly: break;
                                case Keyword_Sort: lv.Sorting = on ? SortOrder.Ascending : SortOrder.None; break;
                                case Keyword_SortDesc: lv.Sorting = on ? SortOrder.Descending : SortOrder.None; break;
                                case Keyword_WantF2: break;
                            }
                        }
                    }
                    break;
                #endregion

                #region TreeView
                case Keyword_TreeView:
                    {
                        var tree = (TreeView)(control ?? new TreeView());
                        control = tree;
                        opts = GuiApplyStyles(tree, options);

                        foreach (string opt in ParseOptions(opts))
                        {
                            bool on = opt[0] != '-';
                            string mode = opt.Substring(!on || opt[0] == '+' ? 1 : 0).ToLowerInvariant();

                            switch (mode)
                            {
                                case Keyword_Buttons: break;
                                case Keyword_HScroll: break;
                                case Keyword_Lines: break;
                                case Keyword_Readonly: break;
                                case Keyword_WantF2: break;

                                default:
                                    if (mode.StartsWith(Keyword_ImageList))
                                    {
                                        mode = mode.Substring(Keyword_ImageList.Length);

                                        // UNDONE: TreeView control ImageList
                                    }
                                    break;
                            }
                        }
                    }
                    break;
                #endregion

                #region Hotkey
                case Keyword_Hotkey:
                    {
                        // TODO: subclass edit for hotkey control
                    }
                    break;
                #endregion

                #region DateTime
                case Keyword_DateTime:
                    {
                        var date = (DateTimePicker)(control ?? new DateTimePicker());
                        control = date;
                        opts = GuiApplyStyles(date, options);

                        foreach (string opt in ParseOptions(opts))
                        {
                            bool on = opt[0] != '-';
                            string mode = opt.Substring(!on || opt[0] == '+' ? 1 : 0).ToLowerInvariant();

                            switch (mode)
                            {
                                case "1": break;
                                case "2": break;
                                case Keyword_Right: break;
                                case Keyword_LongDate: break;
                                case Keyword_Time: break;

                                default:
                                    if (mode.StartsWith(Keyword_Range))
                                    {
                                        string[] range = mode.Substring(Keyword_Range.Length).Split(new[] { '-' }, 2);

                                    }
                                    else if (mode.StartsWith(Keyword_Choose))
                                    {
                                        mode = mode.Substring(Keyword_Choose.Length);

                                    }
                                    break;
                            }
                        }
                    }
                    break;
                #endregion

                #region MonthCal
                case Keyword_MonthCal:
                    {
                        var cal = (MonthCalendar)(control ?? new MonthCalendar());
                        control = cal;
                        opts = GuiApplyStyles(cal, options);

                        foreach (string opt in ParseOptions(opts))
                        {
                            bool on = opt[0] != '-';
                            string mode = opt.Substring(!on || opt[0] == '+' ? 1 : 0).ToLowerInvariant();

                            switch (mode)
                            {
                                case "4": break;
                                case "8": break;
                                case "16": break;
                                case Keyword_Multi: break;

                                default:
                                    if (mode.StartsWith(Keyword_Range, StringComparison.OrdinalIgnoreCase))
                                    {
                                        string[] range = mode.Substring(Keyword_Range.Length).Split(new[] { '-' }, 2);

                                    }
                                    break;
                            }
                        }
                    }
                    break;
                #endregion

                #region Slider
                case Keyword_Slider:
                    {
                        var slider = (TrackBar)(control ?? new TrackBar());
                        control = slider;
                        opts = GuiApplyStyles(slider, options);

                        foreach (string opt in ParseOptions(opts))
                        {
                            bool on = opt[0] != '-';
                            string mode = opt.Substring(!on || opt[0] == '+' ? 1 : 0).ToLowerInvariant();

                            switch (mode)
                            {
                                case Keyword_Center: break;
                                case Keyword_Invert: break;
                                case Keyword_Left: break;
                                case Keyword_NoTicks: break;
                                case Keyword_Thick: break;
                                case Keyword_Vertical: break;

                                default:
                                    if (mode.StartsWith(Keyword_Line))
                                    {
                                        mode = mode.Substring(Keyword_Line.Length);

                                    }
                                    else if (mode.StartsWith(Keyword_Page))
                                    {
                                        mode = mode.Substring(Keyword_Page.Length);

                                    }
                                    else if (mode.StartsWith(Keyword_Range))
                                    {
                                        mode = mode.Substring(Keyword_Range.Length);
                                        string[] parts = mode.Split(new[] { '-' }, 2);

                                    }
                                    else if (mode.StartsWith(Keyword_TickInterval))
                                    {
                                        mode = mode.Substring(Keyword_TickInterval.Length);

                                    }
                                    else if (mode.StartsWith(Keyword_ToolTip))
                                    {
                                        mode = mode.Substring(Keyword_ToolTip.Length);

                                        switch (mode)
                                        {
                                            case Keyword_Left: break;
                                            case Keyword_Right: break;
                                            case Keyword_Top: break;
                                            case Keyword_Bottom: break;
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    break;
                #endregion

                #region Progress
                case Keyword_Progress:
                    {
                        var progress = (ProgressBar)(control ?? new ProgressBar());
                        control = progress;
                        opts = GuiApplyStyles(progress, options);

                        foreach (string opt in ParseOptions(opts))
                        {
                            bool on = opt[0] != '-';
                            string mode = opt.Substring(!on || opt[0] == '+' ? 1 : 0).ToLowerInvariant();

                            switch (mode)
                            {
                                case Keyword_Smooth: break;
                                case Keyword_Vertical: break;

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

                                        }
                                    }
                                    else if (mode.StartsWith(Keyword_Background))
                                    {
                                        mode = mode.Substring(Keyword_Background.Length);
                                    }
                                    break;
                            }
                        }
                    }
                    break;
                #endregion

                #region GroupBox
                case Keyword_GroupBox:
                    {
                        var group = (GroupBox)(control ?? new GroupBox());
                        control = group;
                        group.Text = content;
                    }
                    break;
                #endregion

                #region Tab
                case Keyword_Tab:
                case Keyword_Tab2:
                    {
                        var tab = (TabPage)(control ?? new TabPage());
                        control = tab;
                        opts = GuiApplyStyles(tab, options);

                        foreach (string opt in ParseOptions(opts))
                        {
                            bool on = opt[0] != '-';
                            string mode = opt.Substring(!on || opt[0] == '+' ? 1 : 0).ToLowerInvariant();

                            switch (mode)
                            {
                                case Keyword_Background: break;
                                case Keyword_Buttons: break;
                                case Keyword_Top: break;
                                case Keyword_Left: break;
                                case Keyword_Right: break;
                                case Keyword_Bottom: break;
                                case Keyword_Wrap: break;

                                default:
                                    if (mode.StartsWith(Keyword_Choose, StringComparison.OrdinalIgnoreCase))
                                    {
                                        mode = mode.Substring(Keyword_Choose.Length);
                                    }
                                    break;
                            }
                        }
                    }
                    break;
                #endregion

                #region StatusBar
                case Keyword_StatusBar:
                    {
                        var status = (StatusBar)(control ?? new StatusBar());
                        control = status;
                        status.Text = content;
                    }
                    break;
                #endregion

                #region WebBrowser
                case Keyword_WebBrowser:
                    {
                        var web = (WebBrowser)(control ?? new WebBrowser());
                        control = web;
                        web.Navigate(content);
                    }
                    break;
                #endregion
            }

            if (opts == null)
                GuiApplyStyles(control, options);
        }

        static Form GuiCreateWindow(string name)
        {
            if (name == "1")
                name = string.Empty;

            var win = new Form { Name = name, Tag = new GuiInfo { } };

            win.FormClosed += new FormClosedEventHandler(delegate(object sender, FormClosedEventArgs e)
            {
                SafeInvoke(win.Name + Keyword_GuiClose);
            });

            win.KeyDown += new KeyEventHandler(delegate(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Escape)
                {
                    e.Handled = true;
                    SafeInvoke(win.Name + Keyword_GuiEscape);
                }
            });

            win.Resize += new EventHandler(delegate(object sender, EventArgs e)
            {
                SafeInvoke(win.Name + Keyword_GuiSize);
            });

            return win;
        }

        static void GuiApplyInitialStyles(Control control)
        {
            if (control.Location.IsEmpty)
            {
                int n = control.Parent.Controls.Count - 1;

                if (n == 0)
                    control.Location = new Point(control.Parent.Margin.Left, control.Parent.Margin.Top);
                else
                {
                    var last = control.Parent.Controls[n - 1];
                    var location = last.Location;
                    location.Y += last.Size.Height + last.Margin.Bottom + control.Margin.Top;
                    control.Location = location;
                }
            }

            if (control.Size.IsEmpty)
                control.Size = control.PreferredSize;
        }

        static string GuiApplyStyles(Control control, string styles)
        {
            control.Size = control.PreferredSize;

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
                        break;

                    case Keyword_Center:
                        break;

                    case Keyword_Right:
                        break;

                    case Keyword_AltSubmit:
                        break;

                    case Keyword_Background:
                        break;

                    case Keyword_Border:
                        SafeSetProperty(control, "BorderStyle", on ? BorderStyle.FixedSingle : BorderStyle.None);
                        break;

                    case Keyword_Enabled:
                        control.Enabled = on;
                        break;

                    case Keyword_Disabled:
                        control.Enabled = !on;
                        break;

                    case Keyword_HScroll:
                        break;

                    case Keyword_VScroll:
                        break;

                    case Keyword_TabStop:
                        control.TabStop = on;
                        break;

                    case Keyword_Theme:
                        break;

                    case Keyword_Transparent:
                        control.BackColor = Color.Transparent;
                        break;

                    case Keyword_Visible:
                    case Keyword_Vis:
                        control.Visible = on;
                        break;

                    case Keyword_Wrap:
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
                                    control.ForeColor = ParseColor(arg);
                                else
                                    append = true;
                                break;

                            case 'v':
                                control.Name = arg;
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

        static void GuiControlMove(string mode, Control control)
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
                                x = alt ? control.Parent.Margin.Top : control.Parent.Margin.Left;
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

        static Control GuiFindControl(string name)
        {
            return GuiFindControl(name, DefaultGui);
        }

        static Control GuiFindControl(string name, Form gui)
        {
            if (gui == null)
                return null;

            foreach (Control control in gui.Controls)
                if (control.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
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
                    ctrl.Text = Param3;
                    break;

                case Keyword_Move:
                case Keyword_MoveDraw:
                    GuiControlMove(Param3, ctrl);
                    break;

                case Keyword_Focus:
                    ctrl.Focus();
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
                    ctrl.Parent.Controls.Remove(ctrl);
                    ctrl.Dispose();
                    break;

                case Keyword_Choose:
                    // UNDONE: choose item for gui control
                    break;

                case Keyword_Font:
                    // TODO: change control font
                    break;

                default:
                    int n;
                    if (Command.StartsWith(Keyword_Enable) && int.TryParse(Command.Substring(Keyword_Enable.Length), out n) && (n == 1 || n == 0))
                        ctrl.Enabled = n == 1;
                    if (Command.StartsWith(Keyword_Disable) && int.TryParse(Command.Substring(Keyword_Disable.Length), out n) && (n == 1 || n == 0))
                        ctrl.Enabled = n == 0;
                    //GuiApplyExtendedStyles(ctrl, ctrl.GetType().Name, Param3);
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
                    OutputVar = ctrl.Text;
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
    }
}
