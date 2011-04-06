using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Collections.Generic;

namespace IronAHK.Rusty.Common
{
    partial class Window
    {
        /// <summary>
        /// Platform Independent Windowmanager.
        /// This Class is abstract.
        /// </summary>
        public abstract class WindowManagerBase
        {
            private Dictionary<string, Stack<WindowItemBase>> windowGroups = new Dictionary<string, Stack<WindowItemBase>>();

            #region Window Groups

            public Dictionary<string, Stack<WindowItemBase>> Groups
            {
                get
                {
                    return windowGroups;
                }
            }

            #endregion

            /// <summary>
            /// Minimizes all Windows
            /// </summary>
            public abstract void MinimizeAll();

            /// <summary>
            /// Undo Minimize All Windows
            /// </summary>
            public abstract void MinimizeAllUndo();

            public abstract WindowItemBase GetForeGroundWindow();

            public abstract WindowItemBase WindowFromPoint(Point location);


            #region Find

            public abstract WindowItemBase LastFound { get; set; }

            public abstract IEnumerable<WindowItemBase> AllWindows { get; }

            public abstract WindowItemBase ActiveWindow { get; }

            public abstract WindowItemBase FindWindow(SearchCriteria criteria);

            public WindowItemBase FindWindow(string title, string text, string excludeTitle, string excludeText)
            {
                WindowItemBase foundWindow = null;

                if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(text) && string.IsNullOrEmpty(excludeTitle) && string.IsNullOrEmpty(excludeText))
                    foundWindow = LastFound;
                else
                {
                    var criteria = SearchCriteria.FromString(title, text, excludeTitle, excludeText);
                    foundWindow = FindWindow(criteria);
                }
                return foundWindow;
            }

            #endregion

            #region Specail

            /// <summary>
            /// Creates a concrete Window
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public abstract WindowItemBase CreateWindow(IntPtr id);


            #endregion
        }
    }
}
