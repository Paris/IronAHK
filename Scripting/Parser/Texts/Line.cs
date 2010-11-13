using System;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        void MoveToEOL(string code, ref int i)
        {
            while (i < code.Length)
            {
                switch (code[i])
                {
                    case CR:
                        int n = i + 1;
                        if (n < code.Length && code[n] == LF)
                            i = n;
                        goto case LF;

                    case LF:
                        return;

                    default:
                        i++;
                        break;
                }
            }
        }

        bool IsContinuationLine(string code)
        {
            if (code.Length == 0)
                return false;

            switch (code[0])
            {
                case Divide:
                case BitOR:
                case Concatenate:
                case Equal:
                case TernaryA:
                case Not:
                case BitNOT:
                case BitXOR:
                case Address:
                case Less:
                case Greater:
                    return !(IsHotstringLabel(code) || IsHotkeyLabel(code));

                case Multiply:
                    if (1 < code.Length && code[1] == MultiComA)
                        return false;
                    goto case Not;

                case Add:
                case Subtract:
                    if (1 < code.Length && code[1] == code[0])
                        return false;
                    goto case Not;

                case TernaryB:
                    if (code.Length > 1 && code[1] == Equal)
                        return true;
                    return !(code.Length > 1 && !IsSpace(code[1]));

                default:
                    if (code[0] == Multicast)
                        goto case Divide;
                    break;
            }

            if (code.Length >= AndTxt.Length && code.Substring(0, AndTxt.Length).Equals(AndTxt, StringComparison.OrdinalIgnoreCase))
                return true;
            else if (code.Length >= OrTxt.Length && code.Substring(0, OrTxt.Length).Equals(OrTxt, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
    }
}
