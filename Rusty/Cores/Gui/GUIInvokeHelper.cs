using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace IronAHK.Rusty.Cores.Gui
{
    static class GUIInvokeHelper
    {
        /*
        private Label label1;
        private TextBox txtMessage;
         * */
        delegate void SetTextCallback(Control control, string text);

        /// <summary>
        /// Threadsave Text Property setter
        /// </summary>
        /// <param name="control"></param>
        /// <param name="text"></param>
        public static void SetText(Control control, string text){

            if(control.InvokeRequired) {
                SetTextCallback d = new SetTextCallback(SetText);
                control.Invoke(d, new object[] { control, text });
            } else {
                control.Text = text;
            }
        }
    }
}
