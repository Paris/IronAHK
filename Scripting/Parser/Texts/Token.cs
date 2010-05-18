using System;
using System.IO;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        #region Control

        enum Token { Unknown, Assign, Command, Label, Hotkey, Flow, Expression, Directive }

        Token GetToken(string code)
        {
            code = code.TrimStart(Spaces);

            if (code.Length == 0)
                return Token.Unknown;

            if (IsFlowOperator(code))
                return Token.Flow;
            else if (IsLabel(code))
                return Token.Label;
            else if (IsHotkeyLabel(code) || IsHotstringLabel(code))
                return Token.Hotkey;
            else if (IsAssignment(code))
                return Token.Assign;
            else if (IsDirective(code))
                return Token.Directive;
            else if (IsCommand(code))
                return Token.Command;
            else
                return Token.Expression;
        }

        #endregion

        #region Space

        bool IsSpace(char sym)
        {
            return Array.IndexOf(Spaces, sym) != -1;
        }

        bool IsSpace(string code)
        {
            foreach (var sym in code)
                if (!IsSpace(sym))
                    return false;
            return true;
        }

        #endregion

        bool IsFlowOperator(string code)
        {
            const int offset = 3;
            var delimiters = new char[Spaces.Length + offset];
            delimiters[0] = Multicast;
            delimiters[1] = BlockOpen;
            delimiters[2] = ParenOpen;
            Spaces.CopyTo(delimiters, offset);
            string word = code.Split(delimiters, 2)[0].ToLowerInvariant();

            switch (word)
            {
                case FlowBreak:
                case FlowContinue:
                case FlowElse:
                case FlowGosub:
                case FlowGoto:
                case FlowIf:
                case FlowLoop:
                case FlowReturn:
                case FlowWhile:
                case FunctionLocal:
                case FunctionGlobal:
                case FunctionStatic:
                    return true;
            }

            return false;
        }

        #region Labels

        bool IsLabel(string code)
        {
            for (int i = 0; i < code.Length; i++)
            {
                char sym = code[i];

                if (IsIdentifier(sym))
                    continue;

                switch (sym)
                {
                    case HotkeyBound:
                        if (i == 0)
                            return false;
                        else if (i == code.Length - 1)
                            return true;
                        else
                        {
                            string sub = StripCommentSingle(code.Substring(i));
                            return sub.Length == 0 || IsSpace(sub);
                        }

                    case ParenOpen:
                    case ParenClose:
                        break;

                    default:
                        return false;
                }
            }

            return false;
        }

        bool IsHotkeyLabel(string code)
        {
            int z = code.IndexOf(HotkeySignal);

            if (z == -1)
                return false;

            bool p = false;

            for (int i = 0; i < z; i++)
            {
                char sym = code[i];

                switch (sym)
                {
                    case '#':
                    case '!':
                    case '^':
                    case '+':
                    case '<':
                    case '>':
                    case '*':
                    case '~':
                    case '$':
                        break;

                    case '&':
                        p = false;
                        break;

                    default:
                        if (!IsSpace(sym) && !char.IsLetterOrDigit(sym))
                        {
                            if (p)
                                return false;
                            else
                                p = true;
                        }
                        break;
                }
            }

            return true;
        }

        bool IsHotstringLabel(string code)
        {
            if (!code.Contains(HotkeySignal))
                return false;

            // TODO: hotstring tokens

            return code.Length > 0 && code[0] == HotkeyBound;
        }

        #endregion

        bool IsAssignment(string code)
        {
            int i = 0;

            while (i < code.Length && (IsIdentifier(code[i]) || code[i] == Resolve)) i++;

            if (i == 0 || i == code.Length)
                return false;

            while (IsSpace(code[i])) i++;

            return i < code.Length && code[i] == Equal;
        }

        bool IsCommand(string code)
        {
            int i = 0;

            while (i < code.Length && IsIdentifier(code[i])) i++;

            if (i == 0)
                return false;
            else if (i == code.Length)
                return true;
            else if (code[i] == Multicast)
                return true;
            else if (IsSpace(code[i]))
            {
                i++;
                while (i < code.Length && IsSpace(code[i])) i++;

                if (i < code.Length && code[i] == Equal)
                    return false;
                else if (IsCommentAt(code, i))
                    return true;

                if (IsIdentifier(code[i]))
                    return !IsKeyword(code[i]);

                int y = i + 1, z = i + 2;

                if (y < code.Length)
                {
                    if (code[y] == Equal)
                        return false;
                    else if (z < code.Length && code[i] == code[y] && code[z] == Equal)
                        return false;
                    else if (LaxExpressions)
                    {
                        if (IsOperator(code.Substring(i, 1)) && code.Contains(" ? "))
                            return false;
                    }
                }

                string pre = code.Substring(0, i).TrimEnd(Spaces);
                return !IsPrimativeObject(pre);
            }
            else
                return false;
        }

        bool IsDirective(string code)
        {
            return code.Length > 2 && code[0] == Directive;
        }

        bool IsFunction(string code, string next)
        {
            if (code.Length == 0 || code[0] == ParenOpen)
                return false;

            int stage = 0;
            bool str = false;

            for (int i = 0; i < code.Length; i++)
            {
                char sym = code[i];

                switch (stage)
                {
                    case 0:
                        if (sym == ParenOpen)
                            stage++;
                        else if (!IsIdentifier(sym))
                            return false;
                        break;

                    case 1:
                        if (sym == StringBound)
                            str = !str;
                        else if (!str && sym == ParenClose)
                            stage++;
                        break;

                    case 2:
                        if (sym == BlockOpen)
                            return true;
                        else if (IsCommentAt(code, i))
                            goto next;
                        else if (!IsSpace(sym))
                            return false;
                        break;
                }
            }

        next:
            if (next.Length == 0)
                return false;

            var reader = new StringReader(next);

            while (reader.Peek() != -1)
            {
                var sym = (char)reader.Read();

                if (sym == BlockOpen)
                    return true;
                else if (!IsSpace(sym))
                    return false;
            }

            return false;
        }
    }
}
