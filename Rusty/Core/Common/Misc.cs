using System.Diagnostics;

namespace IronAHK.Rusty
{
    partial class Core
    {
        #region Disk

        static string[] Glob(string pattern)
        {
            return new string[] { };
        }

        #endregion

        #region Process

        static Process FindProcess(string name)
        {
            int id;

            if (int.TryParse(name, out id))
                return System.Diagnostics.Process.GetProcessById(id);

            var prc = System.Diagnostics.Process.GetProcessesByName(name);
            return prc.Length > 0 ? prc[0] : null;
        }

        #endregion
    }
}
