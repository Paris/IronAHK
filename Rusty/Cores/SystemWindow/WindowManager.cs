using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Collections.Generic;


namespace IronAHK.Rusty.Cores.SystemWindow
{
    public abstract class WindowManager
    {
        private Dictionary<string, Stack<SystemWindow>> windowGroups = new Dictionary<string,Stack<SystemWindow>>();

        #region Window Groups

        public Dictionary<string, Stack<SystemWindow>> Groups {
            get {
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

        public abstract SystemWindow GetForeGroundWindow();

        public abstract SystemWindow WindowFromPoint(Point location);


        #region Find

        public abstract SystemWindow LastFound { get; set; }

        public abstract IEnumerable<SystemWindow> AllWindows { get; }

        public abstract SystemWindow ActiveWindow { get; }

        public abstract SystemWindow FindWindow(SearchCriteria criteria);

        public SystemWindow FindWindow(string title, string text, string excludeTitle, string excludeText)
        {
            SystemWindow foundWindow = null;

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
        public abstract SystemWindow CreateWindow(IntPtr id);


        #endregion
    }
}
