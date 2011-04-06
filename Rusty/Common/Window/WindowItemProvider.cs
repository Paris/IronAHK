using System;
using System.Collections.Generic;
using System.Text;

namespace IronAHK.Rusty.Common
{
    partial class Window
    {
        /// <summary>
        /// Singleton Facade for easy accessing current Platform's WindowManager
        /// </summary>
        public class WindowItemProvider
        {
            static readonly WindowManagerBase instance = WindowProvider.CreateWindowManager();

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static WindowItemProvider() { }

            // private constructor
            WindowItemProvider() { }

            public static WindowManagerBase Instance
            {
                get { return instance; }
            }
        }
    }
}
