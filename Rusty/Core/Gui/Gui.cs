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
        /// <param name="Command"></param>
        /// <param name="Param2"></param>
        /// <param name="Param3"></param>
        /// <param name="Param4"></param>
        public static void Gui(string Command, string Param2, string Param3, string Param4)
        {
            if (guis == null)
                guis = new Dictionary<string, BaseGui.Window>();

            string id = GuiId(ref Command);

            if (!guis.ContainsKey(id))
                guis.Add(id, new WinForms.Window());

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
                                break;

                            case Keyword_UpDown:
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
                                break;

                            case Keyword_ComboBox:
                                break;

                            case Keyword_ListBox:
                                break;

                            case Keyword_ListView:
                                break;

                            case Keyword_TreeView:
                                break;

                            case Keyword_Hotkey:
                                break;

                            case Keyword_DateTime:
                                break;

                            case Keyword_MonthCal:
                                break;

                            case Keyword_Slider:
                                break;

                            case Keyword_Progress:
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
                                break;

                            case Keyword_StatusBar:
                                {
                                    var status = guis[id].CreateStatusBar();
                                    status.Contents = Param4;
                                    GuiApplyStyles(status, Param3);
                                    guis[id].Add(status);
                                }
                                break;

                            case Keyword_WebBrowser:
                                break;
                        }
                    }
                    break;

                #endregion

                #region Show

                case Keyword_Show:
                    {
                        bool center = false, cX = false, cY = false, auto = false, min = false, max = false, restore = false, noactivate = false, na = false, hide = false;
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
                                    case Keyword_NoActivate: noactivate = true; break;
                                    case Keyword_NA: na = true; break;
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
                    defaultGui = id;
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

        static string GuiApplyStyles(BaseGui.Control control, string styles)
        {
            string[] opts = ParseOptions(styles), excess = new string[opts.Length];

            for (int i = 0; i < opts.Length; i++)
            {
                string mode = opts[i].ToLowerInvariant();
                bool append = false;

                bool on = mode[0] != '-';
                if (mode[0] == '+' || mode[0] == '-')
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
                                if (int.TryParse(arg, out n))
                                    control.Location = new Point(n, control.Location.Y);
                                break;

                            case 'y':
                                if (int.TryParse(arg, out n))
                                    control.Location = new Point(control.Location.X, n);
                                break;

                            case 'w':
                                if (int.TryParse(arg, out n))
                                    control.Size = new Size(n, control.Size.Height);
                                break;

                            case 'h':
                                if (int.TryParse(arg, out n))
                                    control.Size = new Size(control.Size.Width, n);
                                break;

                            case 'r':
                                if (control.Parent != null && control.Parent.Font != null && int.TryParse(arg, out n))
                                    control.Size = new Size(control.Size.Width, (int)(n * control.Parent.Font.GetHeight()));
                                break;

                            case 'c':
                                if (arg.Length != 0 && !mode.StartsWith(Keyword_Check, StringComparison.OrdinalIgnoreCase))
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

        /// <summary>
        /// Makes a variety of changes to a control in a GUI window.
        /// </summary>
        /// <param name="Command">See list below.</param>
        /// <param name="ControlID">
        /// <para>If the target control has an associated variable, specify the variable's name as the ControlID (this method takes precedence over the ones described next). For this reason, it is usually best to assign a variable to any control that will later be accessed via GuiControl or GuiControlGet, even if that control is not an input-capable type (such as GroupBox or Text).</para>
        /// <para>Otherwise, ControlID can be either ClassNN (the classname and instance number of the control) or the name/text of the control, both of which can be determined via Window Spy. When using name/text, the matching behavior is determined by SetTitleMatchMode. Note: a picture control's file name (as it was specified at the time the control was created) may be used as its ControlID.</para>
        /// </param>
        /// <param name="Param3">This parameter is omitted except where noted in the list of sub-commands below.</param>
        public static void GuiControl(string Command, string ControlID, string Param3)
        {

        }

        /// <summary>
        /// Retrieves various types of information about a control in a GUI window.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the result. If the command cannot complete (see ErrorLevel below), this variable is made blank.</param>
        /// <param name="Sub_command">See list below.</param>
        /// <param name="ControlID">
        /// <para>If blank or omitted, it behaves as though the name of the output variable was specified. For example, GuiControlGet, MyEdit is the same as GuiControlGet, MyEdit,, MyEdit.</para>
        /// <para>If the target control has an associated variable, specify the variable's name as the ControlID (this method takes precedence over the ones described next). For this reason, it is usually best to assign a variable to any control that will later be accessed via GuiControl or GuiControlGet, even if that control is not input-capable (such as GroupBox or Text).</para>
        /// <para>Otherwise, ControlID can be either ClassNN (the classname and instance number of the control) or the name/text of the control, both of which can be determined via Window Spy. When using name/text, the matching behavior is determined by SetTitleMatchMode. Note: a picture control's file name (as it was specified at the time the control was created) may be used as its ControlID.</para>
        /// </param>
        /// <param name="Param4">This parameter is omitted except where noted in the list of sub-commands below.</param>
        public static void GuiControlGet(out string OutputVar, string Sub_command, string ControlID, string Param4)
        {
            OutputVar = null;
        }

        /// <summary>
        /// Creates, deletes, modifies and displays menus and menu items. Changes the tray icon and its tooltip. Controls whether the main window of a compiled script can be opened.
        /// </summary>
        /// <param name="MenuName">
        /// <para>It can be TRAY or the name of any custom menu. A custom menu is automatically created the first time its name is used with the Add command. For example: Menu, MyMenu, Add, Item1</para>
        /// <para>Once created, a custom menu can be displayed with the Show command. It can also be attached as a submenu to one or more other menus via the Add command.</para>
        /// </param>
        /// <param name="Cmd">These 4 parameters are dependent on each other. See list below for the allowed combinations.</param>
        /// <param name="P3">See <paramref name="Cmd"/>.</param>
        /// <param name="P4">See <paramref name="Cmd"/>.</param>
        /// <param name="P5">See <paramref name="Cmd"/>.</param>
        public static void Menu(string MenuName, string Cmd, string P3, string P4, string P5)
        {

        }
    }
}
