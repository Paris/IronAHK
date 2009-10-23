using System;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// Method is exclusive to Win32.
        /// </summary>
        public class Win32Required : Exception { }

        /// <summary>
        /// Internal error level exception.
        /// </summary>
        class ErrorLevelException : Exception
        {
            int level = 1;

            public int Level
            {
                get { return level; }
                set { level = value; }
            }
        }
    }
}
