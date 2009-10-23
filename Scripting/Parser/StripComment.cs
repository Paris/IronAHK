using System;
using System.IO;
using System.Text;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        string StripComment(string code)
        {
            var reader = new StringReader(code);
            var buf = new StringBuilder(code.Length);
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                buf.Append(StripCommentSingle(line));
                buf.Append(Environment.NewLine);
            }

            int length = Environment.NewLine.Length;
            buf.Remove(buf.Length - length, length);

            return buf.ToString();
        }

        string StripCommentSingle(string code)
        {
            for (int i = 0; i < code.Length; i++)
            {
                bool spaced = i > 0 && IsSpace(code[i - 1]);

#if LEGACY
                if (code.Length - i > Comment.Length && code.Substring(i, Comment.Length) == Comment && (i == 0 || spaced))
#endif
#if !LEGACY
                if (code[i] == Comment && (i == 0 || spaced))
#endif
                    return code.Substring(0, i - (spaced ? 1 : 0));
            }

            return code;
        }
    }
}
