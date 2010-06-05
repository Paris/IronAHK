using System.Drawing;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        // TODO: organise Tips.cs

        /// <summary>
        /// Creates an always-on-top window anywhere on the screen.
        /// </summary>
        /// <param name="Text">
        /// <para>If blank or omitted, the existing tooltip (if any) will be hidden. Otherwise, this parameter is the text to display in the tooltip. To create a multi-line tooltip, use the linefeed character (`n) in between each line, e.g. Line1`nLine2.</para>
        /// <para>If Text is long, it can be broken up into several shorter lines by means of a continuation section, which might improve readability and maintainability.</para>
        /// </param>
        /// <param name="X">The X position of the tooltip relative to the active window (use "CoordMode, ToolTip" to change to screen coordinates). If the coordinates are omitted, the tooltip will be shown near the mouse cursor. X and Y can be expressions.</param>
        /// <param name="Y">The Y position (see <paramref name="X"/>).</param>
        /// <param name="ID">Omit this parameter if you don't need multiple tooltips to appear simultaneously. Otherwise, this is a number between 1 and 20 to indicate which tooltip window to operate upon. If unspecified, that number is 1 (the first).</param>
        public static void ToolTip(string Text, int X, int Y, int ID)
        {
            if (tooltip == null)
            {
                tooltip = new Form { Width = 0, Height = 0, Visible = false };
                tooltip.Show();
            }

            if (persistentTooltip == null)
                persistentTooltip = new ToolTip { AutomaticDelay = 0, InitialDelay = 0, ReshowDelay = 0, ShowAlways = true };

            var bounds = Screen.PrimaryScreen.WorkingArea;
            persistentTooltip.Show(Text, tooltip, new Point((bounds.Left - bounds.Right) / 2, (bounds.Bottom - bounds.Top) / 2));
        }

        /// <summary>
        /// Creates a balloon message window near the tray icon. Requires Windows 2000/XP or later.
        /// </summary>
        /// <param name="Title">
        /// <para>If all parameters are omitted, any TrayTip window currently displayed will be removed.</para>
        /// <para>Otherwise, this parameter is the title of the window, which can be up to 73 characters long (characters beyond this length are not shown).</para>
        /// <para>If Title is blank, the title line will be entirely omitted from the balloon window, making it vertically shorter.</para>
        /// </param>
        /// <param name="Text">
        /// <para>If this parameter is omitted or blank, any TrayTip window currently displayed will be removed.</para>
        /// <para>Otherwise, specify the message to display, which appears beneath Title. Only the first 265 characters of Text will be displayed. Carriage return (`r) or linefeed (`n) may be used to create multiple lines of text. For example: Line1`nLine2</para>
        /// <para>If Text is long, it can be broken up into several shorter lines by means of a continuation section, which might improve readability and maintainability.</para>
        /// </param>
        /// <param name="Seconds">
        /// <para>The approximate number of seconds to display the window, after which it will be automatically removed by the OS. Specifying a number less than 10 or greater than 30 will usually cause the minimum (10) or maximum (30) display time to be used instead. If blank or omitted, the minimum time will usually be used. This parameter can be an expression.</para>
        /// <para>The actual timeout may vary from the one specified. Microsoft explains, "if the user does not appear to be using the computer, the system does not count this time towards the timeout." (Technical details here.) Therefore, to have precise control over how long the TrayTip is displayed, use the Sleep command followed by TrayTip with no parameters, or use SetTimer as illustrated in the Examples section below.</para>
        /// </param>
        /// <param name="Options">
        /// <para>Specify one of the following digits to have a small icon appear to the left of Title:</para>
        /// <list>
        /// <item>1: Info icon</item>
        /// <item>2: Warning icon</item>
        /// <item>3: Error icon</item>
        /// <para>If omitted, it defaults to 0, which is no icon.</para>
        /// </list>
        /// </param>
        public static void TrayTip(string Title, string Text, int Seconds, int Options)
        {
            if (Tray == null)
                Tray = new NotifyIcon();

            ToolTipIcon icon = ToolTipIcon.None;

            switch (Options)
            {
                case 1: icon = ToolTipIcon.Info; break;
                case 2: icon = ToolTipIcon.Warning; break;
                case 3: icon = ToolTipIcon.Error; break;
            }

            Tray.ShowBalloonTip(Seconds * 1000, Title, Text, icon);
        }
    }
}
