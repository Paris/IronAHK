using System;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        enum Token { Unknown, Assign, Command, Label, Flow, Expression, Directive }

        Token GetToken(string code)
        {
            if (code.Length == 0)
                return Token.Unknown;

            char sym;
            string word;

            #region Labels and directives

            sym = code[0];

            switch (sym)
            {
                case HotkeyBound:
                    return Token.Label;

                case Directive:
                    return Token.Directive;
            }

            #endregion

            #region Flow operators

            char[] delimiters = new char[Spaces.Length + 1];
            delimiters[0] = Multicast;
            Spaces.CopyTo(delimiters, 1);
            word = code.Split(delimiters, 2)[0].ToLowerInvariant();

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
                    return Token.Flow;
            }

            #endregion

            #region Assignments and commands

            for (int i = 0; i < code.Length; i++)
            {
                sym = code[i];

                if (char.IsLetterOrDigit(sym) || sym == Resolve)
                    continue;

                while (IsSpace(sym) && i < code.Length)
                {
                    i++;
                    sym = code[i];
                }

                if (char.IsLetterOrDigit(sym))
                    return Token.Command;

                switch (sym)
                {
                    case Multicast:
                        return Token.Command;

                    case Equal:
                        return Token.Assign; // TODO: check if expression assign (var = % 1 + 2)

                    case ParenOpen:
                        return Token.Expression;

                    case HotkeyBound:
                        return Token.Label;

                    default:
                        int n = i + 1;
                        if (n < code.Length && code[n] == Equal)
                            return Token.Expression;
                        else
                            return Token.Command;
                }
            }

            #endregion

            return Token.Expression;
        }

        bool IsSpace(char sym)
        {
            return Array.IndexOf<char>(Spaces, sym) != -1;
        }
    }
}
