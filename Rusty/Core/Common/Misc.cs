using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace IronAHK.Rusty
{
    partial class Core
    {
        #region Process

        static Process FindProcess(string name)
        {
            int id;

            if (int.TryParse(name, out id))
                return System.Diagnostics.Process.GetProcessById(id);

            const string exe = ".exe";

            if (name.EndsWith(exe, StringComparison.OrdinalIgnoreCase))
                name = name.Substring(0, name.Length - exe.Length);

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
