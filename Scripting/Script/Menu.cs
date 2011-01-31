using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace IronAHK.Scripting
{
    partial class Script
    {
        public static void CreateTrayMenu()
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                return;

            var menu = new NotifyIcon { ContextMenu = new ContextMenu() };
            menu.ContextMenu.MenuItems.Add(new MenuItem("&Reload", delegate { Application.Restart(); }));
            menu.ContextMenu.MenuItems.Add(new MenuItem("&Exit", delegate { Environment.Exit(0); }));

            var favicon = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(Script).FullName + ".favicon.ico");

            if (favicon != null)
            {
                menu.Icon = new Icon(favicon);
                menu.Visible = true;
            }

            ApplicationExit += delegate
            {
                menu.Visible = false;
                menu.Dispose();
            };
        }
    }
}
