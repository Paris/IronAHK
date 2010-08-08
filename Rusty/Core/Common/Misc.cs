using System;
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

        #region Text

        static string NormaliseEol(string text, string eol = null)
        {
            const string CR = "\r", LF = "\n", CRLF = "\r\n";

            eol = eol ?? Environment.NewLine;

            switch (eol)
            {
                case CR:
                    return text.Replace(CRLF, CR).Replace(LF, CR);

                case LF:
                    return text.Replace(CRLF, LF).Replace(CR, LF);

                case CRLF:
                    return text.Replace(CR, string.Empty).Replace(LF, CRLF);
            }

            return text;
        }

        #endregion
    }
}
