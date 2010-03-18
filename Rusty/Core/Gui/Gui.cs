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
                case Keyword_Add:
                    // TODO: create new control
                    break;

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

                default:
                    {
                        foreach (string option in ParseOptions(Param2))
                        {
                            // TODO: gui options
                        }
                    }
                    break;
            }
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
