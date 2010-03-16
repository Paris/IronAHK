using System;
using System.Collections.Generic;
using System.Drawing;

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
                    guis[id].Show();
                    break;

                case Keyword_Submit:
                    guis[id].Submit(!Keyword_NoHide.Equals(Param2, StringComparison.OrdinalIgnoreCase));
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
