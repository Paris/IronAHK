using System;
using System.IO;
using System.Text;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        string StripComment(string code)
        {
            if (string.IsNullOrEmpty(code))
                return code;

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
            bool spaced = false;
            for (int i = 0; i < code.Length; i++)
            {
                if (IsCommentAt(code, i))
                    return code.Substring(0, i - (spaced ? 1 : 0));
                spaced = IsSpace(code[i]);
            }

            return code;
        }

        bool IsCommentAt(string code, int offset)
        {
            bool spaced = offset == 0 || IsSpace(code[offset - 1]);
#if LEGACY
            return code.Length - offset >= Comment.Length && code.Substring(offset, Comment.Length) == Comment && spaced;
#endif
#if !LEGACY
            return code[offset] == Comment && spaced;
#endif
        }

        bool IsCommentLine(string code)
        {
#if LEGACY
            return code.Length >= Comment.Length && code.Substring(0, Comment.Length) == Comment;
#endif
#if !LEGACY
            return code.Length > 0 && code[0] == Comment;
#endif
        }

        bool IsEmptyStatement(string code)
        {
            for (int i = 0; i < code.Length; i++)
            {
                if (IsCommentAt(code, i))
                    return true;
                else if (!IsSpace(code[i]))
                    return false;
            }

            return true;
        }
    }
}
