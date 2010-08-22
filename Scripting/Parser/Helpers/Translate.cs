using System.Diagnostics;
using System.Text;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        [Conditional(Legacy)]
        void Translate(ref string code)
        {
            #region Variables

            var delim = new char[Spaces.Length + 1];
            delim[0] = Multicast;
            Spaces.CopyTo(delim, 1);
            int z = code.IndexOfAny(delim);
            string cmd, param;

            if (z == -1)
            {
                cmd = code;
                param = string.Empty;
            }
            else
            {
                cmd = code.Substring(0, z);
                param = code.Substring(z).TrimStart(delim);
            }

            var replaced = new StringBuilder(code.Length);

            #endregion

            #region Parameters

            string[] parts = SplitCommandParameters(param);

            if (parts.Length > 0)
                parts[parts.Length - 1] = StripCommentSingle(parts[parts.Length - 1]);

            for (int i = 0; i < parts.Length; i++)
            {
                if (IsExpressionParameter(parts[i]))
                {
                    int e = parts[i].IndexOf(Resolve) + 1;
                    if (e < parts[i].Length)
                        parts[i] = parts[i].Substring(e);
                    else
                        parts[i] = new string(StringBound, 2);
                }
                else
                {
                    parts[i] = parts[i].TrimStart(Spaces);
                    int l = parts[i].Length;
                    if (l > 1 && parts[i][0] == Resolve && parts[i][l - 1] == Resolve)
                        parts[i] = parts[i].Substring(1, l - 2);
                    else
                    {
                        string str = StringBound.ToString();
                        parts[i] = string.Concat(str, parts[i], str);
                    }
                }
            }

            #endregion

            switch (cmd.ToLowerInvariant())
            {
                #region Repeat

                case "repeat":
                    param = StripCommentSingle(param);
                    if (param.Length > 0 && !IsPrimativeObject(param))
                            param = string.Empty;
                    replaced.Append("Loop ");
                    replaced.Append(param);
                    replaced.Append(SingleSpace);
                    replaced.Append(BlockOpen);
                    break;

                case "endrepeat":
                    replaced.Append(BlockClose);
                    replaced.Append(param);
                    break;

                #endregion

                #region Setters

                case "setbatchlines":
                case "setcontroldelay":
                case "setdefaultmousespeed":
                case "setkeydelay":
                case "setmousedelay":
                case "setstorecapslockmode":
                case "settitlematchmode":
                case "setwindelay":
                case "setworkingdir":
                    replaced.Append("A_");
                    replaced.Append(cmd, 3, cmd.Length - 3);
                    replaced.Append(Equal);
                    replaced.Append(param);
                    break;

                case "setenv":
                    replaced.Append(parts[0].Substring(1, parts[0].Length - 2));
                    replaced.Append(AssignPre);
                    replaced.Append(Equal);
                    if (parts.Length > 1)
                        replaced.Append(parts[1]);
                    else
                        replaced.Append(NullTxt);
                    break;

                case "setformat":
                    if (parts.Length != 2)
                        throw new ParseException(ExTooFewParams);
                    replaced.Append("A_Format");
                    const string fast = "fast";
                    parts[0] = parts[0].Substring(1, parts[0].Length - 2);
                    if (parts[0].EndsWith(fast, System.StringComparison.OrdinalIgnoreCase))
                        parts[0] = parts[0].Substring(0, parts[0].Length - fast.Length);
                    replaced.Append(parts[0]);
                    replaced.Append(AssignPre);
                    replaced.Append(Equal);
                    replaced.Append(parts[1]);
                    break;

                case "autotrim":
                case "detecthiddentext":
                case "detecthiddenwindows":
                case "stringcasesense":
                    replaced.Append("A_");
                    replaced.Append(cmd);
                    replaced.Append(Equal);
                    replaced.Append(param);
                    break;

                #endregion

                #region If

                #region Equality

                // TODO: push single conditional command on same line as legacy converted equality-if statements

                case "ifequal":
                    if (parts.Length < 1)
                        throw new ParseException(ExTooFewParams);
                    replaced.Append(FlowIf);
                    replaced.Append(SingleSpace);
                    replaced.Append(parts[0]);
                    replaced.Append(Equal);
                    if (parts.Length > 1)
                        replaced.Append(parts[1]);
                    break;

                case "ifnotequal":
                    if (parts.Length < 1)
                        throw new ParseException(ExTooFewParams);
                    replaced.Append(FlowIf);
                    replaced.Append(SingleSpace);
                    replaced.Append(parts[0]);
                    replaced.Append(Not);
                    replaced.Append(Equal);
                    if (parts.Length > 1)
                        replaced.Append(parts[1]);
                    break;

                case "ifgreater":
                    if (parts.Length < 1)
                        throw new ParseException(ExTooFewParams);
                    replaced.Append(FlowIf);
                    replaced.Append(SingleSpace);
                    replaced.Append(parts[0]);
                    replaced.Append(Greater);
                    if (parts.Length > 1)
                        replaced.Append(parts[1]);
                    break;

                case "ifgreaterorequal":
                    if (parts.Length < 1)
                        throw new ParseException(ExTooFewParams);
                    replaced.Append(FlowIf);
                    replaced.Append(SingleSpace);
                    replaced.Append(parts[0]);
                    replaced.Append(Greater);
                    replaced.Append(Equal);
                    if (parts.Length > 1)
                        replaced.Append(parts[1]);
                    break;

                case "ifless":
                    if (parts.Length < 1)
                        throw new ParseException(ExTooFewParams);
                    replaced.Append(FlowIf);
                    replaced.Append(SingleSpace);
                    replaced.Append(parts[0]);
                    replaced.Append(Less);
                    if (parts.Length > 1)
                        replaced.Append(parts[1]);
                    break;

                case "iflessorequal":
                    if (parts.Length < 1)
                        throw new ParseException(ExTooFewParams);
                    replaced.Append(FlowIf);
                    replaced.Append(SingleSpace);
                    replaced.Append(parts[0]);
                    replaced.Append(Less);
                    replaced.Append(Equal);
                    if (parts.Length > 1)
                        replaced.Append(parts[1]);
                    break;

                #endregion

                case "ifexist":
                    if (parts.Length < 1)
                        throw new ParseException(ExTooFewParams);
                    replaced.Append(FlowIf);
                    replaced.Append(SingleSpace);
                    replaced.Append(ParenOpen);
                    replaced.Append("FileExist");
                    replaced.Append(ParenOpen);
                    replaced.Append(parts[0]);
                    replaced.Append(ParenClose, 2);
                    break;

                case "ifnotexist":
                    if (parts.Length < 1)
                        throw new ParseException(ExTooFewParams);
                    replaced.Append(FlowIf);
                    replaced.Append(SingleSpace);
                    replaced.Append(ParenOpen);
                    replaced.Append(Not);
                    replaced.Append("FileExist");
                    replaced.Append(ParenOpen);
                    replaced.Append(parts[0]);
                    replaced.Append(ParenClose, 2);
                    break;

                case "ifinstring":
                    if (parts.Length < 2)
                        throw new ParseException(ExTooFewParams);
                    replaced.Append(FlowIf);
                    replaced.Append(SingleSpace);
                    replaced.Append(ParenOpen);
                    replaced.Append("InStr");
                    replaced.Append(ParenOpen);
                    replaced.Append(parts[0]);
                    replaced.Append(Multicast);
                    replaced.Append(parts[1]);
                    replaced.Append(ParenClose, 2);
                    break;

                case "ifnotinstring":
                    if (parts.Length < 2)
                        throw new ParseException(ExTooFewParams);
                    replaced.Append(FlowIf);
                    replaced.Append(SingleSpace);
                    replaced.Append(ParenOpen);
                    replaced.Append(Not);
                    replaced.Append("InStr");
                    replaced.Append(ParenOpen);
                    replaced.Append(parts[0]);
                    replaced.Append(Multicast);
                    replaced.Append(parts[1]);
                    replaced.Append(ParenClose, 2);
                    break;

                case "ifmsgbox":
                    if (parts.Length < 1)
                        throw new ParseException(ExTooFewParams);
                    replaced.Append(FlowIf);
                    replaced.Append(SingleSpace);
                    replaced.Append(ParenOpen);
                    replaced.Append("A_MsgBox");
                    replaced.Append(Equal);
                    replaced.Append(parts[0]);
                    replaced.Append(ParenClose);
                    break;

                case "ifwinactive":
                    replaced.Append(FlowIf);
                    replaced.Append(SingleSpace);
                    replaced.Append(ParenOpen);
                    replaced.Append("WinActive");
                    replaced.Append(ParenOpen);
                    foreach (var part in parts)
                    {
                        replaced.Append(part);
                        replaced.Append(Multicast);
                    }
                    if (parts.Length > 1)
                        replaced.Remove(replaced.Length - 1, 1);
                    replaced.Append(ParenClose, 2);
                    break;

                case "ifwinexist":
                    replaced.Append(FlowIf);
                    replaced.Append(SingleSpace);
                    replaced.Append(ParenOpen);
                    replaced.Append("WinExist");
                    replaced.Append(ParenOpen);
                    foreach (var part in parts)
                    {
                        replaced.Append(part);
                        replaced.Append(Multicast);
                    }
                    if (parts.Length > 1)
                        replaced.Remove(replaced.Length - 1, 1);
                    replaced.Append(ParenClose, 2);
                    break;

                case "ifwinnotactive":
                    replaced.Append(FlowIf);
                    replaced.Append(SingleSpace);
                    replaced.Append(ParenOpen);
                    replaced.Append(Not);
                    replaced.Append("WinActive");
                    replaced.Append(ParenOpen);
                    foreach (var part in parts)
                    {
                        replaced.Append(part);
                        replaced.Append(Multicast);
                    }
                    if (parts.Length > 1)
                        replaced.Remove(replaced.Length - 1, 1);
                    replaced.Append(ParenClose, 2);
                    break;

                case "ifwinnotexist":
                    replaced.Append(FlowIf);
                    replaced.Append(SingleSpace);
                    replaced.Append(ParenOpen);
                    replaced.Append(Not);
                    replaced.Append("WinExist");
                    replaced.Append(ParenOpen);
                    foreach (var part in parts)
                    {
                        replaced.Append(part);
                        replaced.Append(Multicast);
                    }
                    if (parts.Length > 1)
                        replaced.Remove(replaced.Length - 1, 1);
                    replaced.Append(ParenClose, 2);
                    break;

                #endregion

                #region Strings

                // HACK: convert L/R paramter for legacy StringGetPos command
                case "stringgetpos":
                    if (parts.Length < 3)
                        throw new ParseException(ExTooFewParams);
                    replaced.Append(parts[0].Trim(StringBound));
                    replaced.Append(AssignPre);
                    replaced.Append(Equal);
                    replaced.Append("InStr");
                    replaced.Append(ParenOpen);
                    replaced.Append(parts[1]);
                    replaced.Append(Multicast);
                    replaced.Append(parts[2]);
                    replaced.Append(Multicast);
                    replaced.Append(FalseTxt);
                    replaced.Append(Multicast);
                    replaced.Append(parts.Length > 4 ? parts[4] : "0");
                    replaced.Append(ParenClose);
                    break;

                case "stringleft":
                    if (parts.Length < 3)
                        throw new ParseException(ExTooFewParams);
                    replaced.Append(parts[0]);
                    replaced.Append(AssignPre);
                    replaced.Append(Equal);
                    replaced.Append("SubStr");
                    replaced.Append(ParenOpen);
                    replaced.Append(parts[1]);
                    replaced.Append(Multicast);
                    replaced.Append("1");
                    replaced.Append(Multicast);
                    replaced.Append(parts[2]);
                    replaced.Append(ParenClose);
                    break;

                case "stringlen":
                    if (parts.Length < 2)
                        throw new ParseException(ExTooFewParams);
                    replaced.Append(parts[0]);
                    replaced.Append(AssignPre);
                    replaced.Append(Equal);
                    replaced.Append("StrLen");
                    replaced.Append(ParenOpen);
                    replaced.Append(parts[1]);
                    replaced.Append(ParenClose);
                    break;

                case "stringmid":
                    if (parts.Length < 3)
                        throw new ParseException(ExTooFewParams);
                    replaced.Append(parts[0]);
                    replaced.Append(AssignPre);
                    replaced.Append(Equal);
                    replaced.Append("SubStr");
                    replaced.Append(ParenOpen);
                    replaced.Append(parts[1]);
                    replaced.Append(Multicast);
                    replaced.Append(parts[2]);
                    if (parts.Length > 3)
                    {
                        replaced.Append(Multicast);
                        replaced.Append(parts[3]);
                    }
                    if (parts.Length > 4)
                    {
                        replaced.Append(Multicast);
                        replaced.Append(parts[4]);
                    }
                    replaced.Append(ParenClose);
                    break;

                case "stringright":
                    if (parts.Length < 3)
                        throw new ParseException(ExTooFewParams);
                    replaced.Append(parts[0]);
                    replaced.Append(AssignPre);
                    replaced.Append(Equal);
                    replaced.Append("SubStr");
                    replaced.Append(ParenOpen);
                    replaced.Append(parts[1]);
                    replaced.Append(Multicast);
                    replaced.Append("1");
                    replaced.Append(Add);
                    replaced.Append(Minus);
                    replaced.Append(ParenOpen);
                    replaced.Append(parts[2]);
                    replaced.Append(ParenClose);
                    replaced.Append(ParenClose);
                    break;

                case "stringtrimleft":
                    if (parts.Length < 3)
                        throw new ParseException(ExTooFewParams);
                    replaced.Append(parts[0]);
                    replaced.Append(AssignPre);
                    replaced.Append(Equal);
                    replaced.Append("SubStr");
                    replaced.Append(ParenOpen);
                    replaced.Append(parts[1]);
                    replaced.Append(Multicast);
                    replaced.Append("1");
                    replaced.Append(Add);
                    replaced.Append(parts[2]);
                    replaced.Append(ParenClose);
                    break;

                case "stringtrimright":
                    if (parts.Length < 3)
                        throw new ParseException(ExTooFewParams);
                    replaced.Append(parts[0]);
                    replaced.Append(AssignPre);
                    replaced.Append(Equal);
                    replaced.Append("SubStr");
                    replaced.Append(ParenOpen);
                    replaced.Append(parts[1]);
                    replaced.Append(Multicast);
                    replaced.Append("1");
                    replaced.Append(Multicast);
                    replaced.Append(Minus);
                    replaced.Append(ParenOpen);
                    replaced.Append(parts[2]);
                    replaced.Append(ParenClose);
                    replaced.Append(ParenClose);
                    break;

                #endregion

                #region Arithmetic

                // TODO: translate legacy EnvMult, EnvDiv etc

                #endregion

                #region Send

                case "sendevent":
                case "sendinput":
                case "sendplay":
                    replaced.Append("Send");
                    replaced.Append(Multicast);
                    replaced.Append(SingleSpace);
                    replaced.Append(param);
                    break;

                case "sendraw":
                    replaced.Append("Send");
                    replaced.Append(Multicast);
                    replaced.Append(SingleSpace);
                    ParameterPrepend(ref param, "{Raw}");
                    replaced.Append(param);
                    break;

                case "controlsendraw":
                    replaced.Append("ControlSend");
                    replaced.Append(Multicast);
                    replaced.Append(SingleSpace);
                    ParameterPrepend(ref param, "{Raw}");
                    replaced.Append(param);
                    break;

                case "sendmode":
                    code = string.Empty;
                    break;

                case "setcapslockstate":
                case "setnumlockstate":
                case "setscrolllockstate":
                    replaced.Append("SetLockState");
                    replaced.Append(Multicast);
                    replaced.Append(SingleSpace);
                    replaced.Append(cmd, 3, cmd.Length - 3 - 5);
                    replaced.Append(Multicast);
                    replaced.Append(SingleSpace);
                    replaced.Append(param);
                    break;

                #endregion

                #region Mouse

                case "leftclick":
                case "mouseclick":
                    replaced.Append("Click");
                    replaced.Append(Multicast);
                    replaced.Append(param);
                    break;

                case "leftclickdrag":
                    replaced.Append("MouseClickDrag");
                    replaced.Append(Multicast);
                    replaced.Append("Left");
                    replaced.Append(Multicast);
                    replaced.Append(param);
                    break;

                case "mousemove":
                    replaced.Append("Click");
                    replaced.Append(Multicast);
                    replaced.Append(param);
                    replaced.Append(Multicast);
                    replaced.Append("0");
                    break;

                #endregion

                #region Debug

                case "edit":
                case "listlines":
                case "listvars":
                    replaced = null;
                    break;

                #endregion

                #region Other

                case "filegetattrib":
                    if (parts.Length != 2)
                        replaced = null;
                    else
                    {
                        replaced.Append(parts[0].Substring(1, parts[0].Length - 2));
                        replaced.Append(AssignPre);
                        replaced.Append(Equal);
                        replaced.Append("FileExist");
                        replaced.Append(ParenOpen);
                        replaced.Append(parts[1]);
                        replaced.Append(ParenClose);
                    }
                    break;

                #endregion
            }

            if (replaced == null)
                code = string.Empty;
            else if (replaced.Length > 0)
                code = replaced.ToString();
        }

        [Conditional(Legacy)]
        void ParameterPrepend(ref string param, string insert)
        {
            if (IsExpressionParameter(param))
            {
                var buffer = new StringBuilder(param.Length + insert.Length + 2);
                buffer.Append(Resolve);
                buffer.Append(SingleSpace);
                buffer.Append(StringBound);
                buffer.Append(insert);
                buffer.Append(StringBound);
                buffer.Append(SingleSpace);
                buffer.Append(Concatenate);
                buffer.Append(SingleSpace);
                buffer.Append(param, 2, param.Length - 2);
                param = buffer.ToString();
            }
            else
                param = string.Concat(insert, param);
        }
    }
}
