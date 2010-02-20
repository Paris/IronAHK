using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        [Conditional("LEGACY")]
        void Translate(ref string code)
        {
            #region Variables

            char[] delim = new char[Spaces.Length + 1];
            delim[1] = Multicast;
            Spaces.CopyTo(delim, 1);
            int z = code.IndexOfAny(Spaces);
            string cmd, param;

            if (z == -1)
            {
                cmd = code;
                param = string.Empty;
            }
            else
            {
                z--;
                cmd = code.Substring(0, z);
                param = code.Substring(z);
            }

            string[] parts = null;
            var replaced = new StringBuilder(code.Length);

            if (param.Length > 0 && param[0] == Multicast)
                param = param.Substring(1);

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
                case "setcapslockstate":
                case "setcontroldelay":
                case "setdefaultmousespeed":
                case "setformat":
                case "setkeydelay":
                case "setmousedelay":
                case "setnumlockstate":
                case "setscrolllockstate":
                case "setstorecapslockmode":
                case "settitlematchmode":
                case "setwindelay":
                case "setworkingdir":
                    replaced.Append(cmd, 3, cmd.Length - 3);
                    replaced.Append(AssignPre);
                    replaced.Append(Equal);
                    replaced.Append(param);
                    break;

                case "autotrim":
                case "detecthiddentext":
                case "detecthiddenwindows":
                case "stringcasesense":
                    replaced.Append("A_");
                    replaced.Append(cmd);
                    replaced.Append(AssignPre);
                    replaced.Append(Equal);
                    replaced.Append(param);
                    break;

                #endregion

                #region If

                // TODO: convert legacy if commands
                case "ifequal":
                case "ifnotequal":
                case "ifgreater":
                case "ifgreaterorequal":
                case "ifless":
                case "iflessorequal":
                case "ifexist":
                case "ifnotexist":
                case "ifinstring":
                case "ifnotinstring":
                case "ifmsgbox":
                case "ifwinactive":
                case "ifwinexist":
                case "ifwinnotactive":
                case "ifwinnotexist":
                    break;

                #endregion

                #region Strings

                // TODO: convert legacy StringGetPos command to StrPos()?
                //case "stringgetpos":
                //    break;

                case "stringleft":
                    SplitParameters(param, ref parts, 3);
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
                    SplitParameters(param, ref parts, 2);
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
                    SplitParameters(param, ref parts, 5);
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
                    SplitParameters(param, ref parts, 3);
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
                    SplitParameters(param, ref parts, 3);
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
                    SplitParameters(param, ref parts, 3);
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
            }

            if (replaced.Length > 0)
                code = replaced.ToString();
        }

        [Conditional("LEGACY")]
        void SplitParameters(string code, ref string[] parameters, int max)
        {
            if (max == 1)
            {
                parameters = new string[] { code };
                return;
            }

            var list = new List<string>();
            var buffer = new StringBuilder(code.Length);

            for (int i = 0; i < code.Length; i++)
            {
                char sym = code[i];

                if (sym == Multicast && (i > 0 ? code[i - 1] != Escape : true))
                {
                    if (list.Count == max)
                    {
                        buffer.Append(code.Substring(i));
                        break;
                    }

                    list.Add(buffer.ToString());
                    buffer.Length = 0;
                }
                else
                    buffer.Append(sym);
            }

            if (buffer.Length > 0)
            {
                if (list.Count == max)
                    list[max - 1] += Multicast.ToString() + buffer.ToString();
                else
                    list.Add(buffer.ToString());
            }

            // TODO: recognise forced expression mode parameters for legacy command conversions

            int last = list.Count - 1;
            if (last > -1)
                list[last] = StripCommentSingle(list[last]);

            parameters = list.ToArray();
        }
    }
}
