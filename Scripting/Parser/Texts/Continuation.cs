using System;

namespace IronAHK.Scripting
{
    partial class Parser
    {
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
                case Multicast:
                    return true;

                case Not:
                case BitNOT:
                case BitXOR:
                case Address:
                case Less:
                case Greater:
                case Multiply:
                    return !(IsHotstringLabel(code) || IsHotkeyLabel(code));

                case Add:
                case Subtract:
                    if (1 < code.Length && code[1] == code[0])
                        return false;
                    if (code[0] == Add)
                        goto case Not;
                    return true;

                case TernaryB:
                    return !(code.Length > 1 && !IsSpace(code[1]));
            }

            if (code.Length >= AndTxt.Length && code.Substring(0, AndTxt.Length).Equals(AndTxt, StringComparison.OrdinalIgnoreCase))
                return true;
            else if (code.Length >= OrTxt.Length && code.Substring(0, OrTxt.Length).Equals(OrTxt, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
    }
}
