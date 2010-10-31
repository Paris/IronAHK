using System.Collections.Generic;
using System.Text;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        List<object> SplitTokens(string code)
        {
            var list = new List<object>();
            bool json = false;

            for (int i = 0; i < code.Length; i++)
            {
                char sym = code[i];

                #region Spaces
                if (IsSpace(sym))
                    continue;
                #endregion
                #region Comments
                else if (IsCommentAt(code, i))
                    MoveToEOL(code, ref i);
                #endregion
                #region Identifiers
                else if (IsIdentifier(sym) || sym == Resolve || (sym == Concatenate && i + 1 < code.Length && IsIdentifier(code[i + 1])))
                {
                    var id = new StringBuilder(code.Length);
                    id.Append(sym);
                    i++;
                    
                    // UNDONE: optimise split tokens

                    for (; i < code.Length; i++)
                    {
                        sym = code[i];
                        if ((sym == 'e' || sym == 'E') && IsPrimativeObject(id.ToString()) && id.ToString().IndexOf("0x") != 0 && i + 1 < code.Length)
                        {
                            id.Append(sym);
                            sym = code[++i];
                            if (!(sym == '+' || sym == '-' || char.IsDigit(sym)))
                                throw new ParseException(ExInvalidExponent);
                            id.Append(sym);
                        }
                        else if (IsIdentifier(sym) || sym == Resolve || (sym == Concatenate && (i + 1 < code.Length ? code[i + 1] != Equal : true)))
                            id.Append(sym);
                        else
                        {
                            if (sym == ParenOpen && !IsKeyword(id.ToString()) && !id.ToString().Contains(Concatenate.ToString()))
                                id.Append(ParenOpen);
                            else
                                i--;
                            break;
                        }
                    }

                    string seq = id.ToString();
                    var parts = IsPrimativeObject(seq) ? new[] { seq } : seq.Split(Concatenate);

                    if (parts[0].Length != 0)
                        list.Add(parts[0]);

                    for (int n = 1; n < parts.Length; n++)
                    {
                        list.Add(ArrayOpen.ToString());
                        string str = StringBound.ToString();
                        list.Add(string.Concat(str, parts[n], str));
                        list.Add(ArrayClose.ToString());
                    }
                }
                #endregion
                #region Strings
                else if (sym == StringBound)
                {
                    var str = new StringBuilder(code.Length);
                    str.Append(StringBound);
                    i++;

                    for (int max = code.Length + 1; i < max; i++)
                    {
                        if (i == code.Length)
                            throw new ParseException(ExUntermStr);

                        sym = code[i];
                        str.Append(sym);

                        if (sym == StringBound)
                        {
                            int n = i + 1;
                            if (n < code.Length && code[n] == StringBound)
                                i = n;
                            else
                                break;
                        }
                    }

                    list.Add(str.ToString());
                }
                #endregion
                #region Operators
                else
                {
                    var op = new StringBuilder(3);
                    int n = i + 1;
                    char symNext = n < code.Length ? code[n] : Reserved;
                    bool tri = false;

                    #region 3x
                    if (sym == symNext)
                    {
                        bool peekAssign = false;

                        switch (sym)
                        {
                            case Divide:
                            case Greater:
                            case Less:
                                peekAssign = true;
                                goto case Add;

                            case Add:
                            case Minus:
                            case Multiply:
                            case BitOR:
                            case BitAND:
                                op.Append(sym);
                                op.Append(symNext);
                                i++;
                                tri = true;
                                if (peekAssign)
                                {
                                    n = i + 1;
                                    if (n < code.Length && code[n] == Equal)
                                    {
                                        op.Append(code[n]);
                                        i = n;
                                    }
                                }
                                break;
                        }
                    }
                    #endregion

                    if (!tri)
                    {
                        #region 2x
                        if (symNext == Equal)
                        {
                            switch (sym)
                            {
                                case AssignPre:
                                case Add:
                                case Minus:
                                case Multiply:
                                case Divide:
                                case Concatenate:
                                case BitAND:
                                case BitXOR:
                                case BitOR:
                                case Not:
                                case Equal:
                                case Greater:
                                case Less:
                                    op.Append(sym);
                                    op.Append(symNext);
                                    i++;
                                    break;
                            }
                        }
                        else if ((sym == Less && symNext == Greater) || (sym == TernaryA && symNext == TernaryA))
                        {
                            op.Append(sym);
                            op.Append(symNext);
                            i++;
                        }
                        #endregion
                        #region 1x
                        else
                        {
                            switch (sym)
                            {
                                case Add:
                                case Minus:
                                case Multiply:
                                case Not:
                                case BitNOT:
                                case BitAND:
                                case Greater:
                                case Less:
                                case BitXOR:
                                case BitOR:
                                case ParenOpen:
                                case ParenClose:
                                case Equal:
                                case Concatenate:
                                case TernaryB:
                                case Divide:
                                case ArrayOpen:
                                case ArrayClose:
                                    op.Append(sym);
                                    break;

                                case BlockOpen:
                                    if (json)
                                    {
                                        op.Append(sym);
                                        break;
                                    }
                                    blockOpen = true;
                                    int j = i + 2;
                                    if (j < code.Length && !IsCommentAt(code, j))
                                    {
                                        blockOpen = false;
                                        json = true;
                                        goto case BlockOpen;
                                    }
                                    j--;
                                    if (j < code.Length)
                                    {
                                        if (code[j] == BlockClose)
                                        {
                                            json = true;
                                            goto case BlockClose;
                                        }
                                        else if (!IsSpace(code[j]))
                                            throw new ParseException(ExUnexpected);
                                    }
                                    return list;

                                case BlockClose:
                                    if (!json)
                                        goto default;
                                    op.Append(sym);
                                    break;

                                default:
                                    if (sym == Resolve || sym == Multicast)
                                        goto case Add;
                                    throw new ParseException(ExUnexpected);
                            }
                        }
                        #endregion
                    }

                    if (op.Length == 0)
                        op.Append(sym);
                    list.Add(op.ToString());
                }
                #endregion
            }

            return list;
        }

        void RemoveExcessParentheses(List<object> parts)
        {
            while (parts.Count > 1)
            {
                int level = 0;
                int last = parts.Count - 1;

                if (!(--last > 1 &&
                    parts[0] is string && ((string)parts[0]).Length == 1 && ((string)parts[0])[0] == ParenOpen &&
                    parts[last] is string && ((string)parts[last]).Length == 1 && ((string)parts[last])[0] == ParenClose))
                    return;

                for (int i = 0; i < last; i++)
                {
                    var check = parts[i] as string;

                    if (string.IsNullOrEmpty(check))
                        continue;

                    switch (check[check.Length - 1])
                    {
                        case ParenOpen:
                            level++;
                            break;

                        case ParenClose:
                            if (check.Length != 1)
                                break;
                            else if (--level < 0)
                                throw new ParseException(ExUnbalancedParens);
                            break;
                    }
                }

                if (level != 0)
                    return;

                parts.RemoveAt(last);
                parts.RemoveAt(0);
            }
        }
    }
}
