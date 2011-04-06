using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace IronAHK.Rusty.Common
{
    static class GuiInvoker
    {
        // TODO: GuiInvoker class is probably unnecessary

        public delegate void Action();

        /// <summary>
        /// Threadsave Text Property setter
        /// </summary>
        /// <param name="control"></param>
        /// <param name="text"></param>
        public static void SetText(Control control, string text){
            if(control.InvokeRequired) {
                Invoke(control, () => SetText(control, text));
            } else {
                control.Text = text;
            }
        }

        public static void Invoke(Control c, Action action) {
            c.Invoke(action);
        }
    }
}
