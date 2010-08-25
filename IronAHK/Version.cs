using System;
using System.Reflection;

namespace IronAHK
{
    public partial class Program
    {
        public static Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }
    }
}
