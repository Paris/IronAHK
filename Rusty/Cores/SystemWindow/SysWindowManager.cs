using System;
using System.Collections.Generic;
using System.Text;

namespace IronAHK.Rusty.Cores.SystemWindow
{
    /// <summary>
    /// Singleton Facade for easy accessing current Platform's WindowManager
    /// </summary>
    public class SysWindowManager
    {
        static readonly WindowManager instance = WindowFactory.CreateWindowManager();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static SysWindowManager() { }

        // private constructor
        SysWindowManager() { }

        public static WindowManager Instance {
            get { return instance; }
        }
    }
}
