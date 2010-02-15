using System.Text;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        string EscapedString(string code, bool resolve)
        {
            if (code.Length == 0)
                return string.Empty;

            var buffer = new StringBuilder(code.Length + 32);
            bool escaped = false;

            foreach (char sym in code)
            {
                if (escaped)
                {
                    switch (sym)
                    {
                        case 'n': buffer.Append('\n'); break;
                        case 'r': buffer.Append('\r'); break;
                        case 'b': buffer.Append('\b'); break;
                        case 't': buffer.Append('\t'); break;
                        case 'v': buffer.Append('\v'); break;
                        case 'a': buffer.Append('\a'); break;
                        case 'f': buffer.Append('\f'); break;

                        case '0': buffer.Append('\0'); break; // UNDONE: should \0 be in non-legacy versions?

                        case Resolve:
                            if (resolve)
                                buffer.Append(Escape);
                            goto default;

                        default:
                            buffer.Append(sym);
                            break;
                    }

                    escaped = false;
                }
                else if (sym == Escape)
                    escaped = true;
                else
                    buffer.Append(sym);
            }

            return buffer.ToString();
        }
    }
}
