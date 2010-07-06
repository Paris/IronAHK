using System;
using System.CodeDom;
using System.Collections.Generic;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        Dictionary<string, string> conditionIds;

        CodeMethodReturnStatement ParseHotkey(List<CodeLine> lines, int index)
        {
            string code = lines[index].Code;

            bool hotstring = code[0] == HotkeyBound;
            string mode = string.Empty;
            if (hotstring)
            {
                int z = code.IndexOf(HotkeyBound, 1) + 1;
                mode = code.Substring(0, z);
                code = code.Substring(z);
            }

            string[] parts = code.Split(new[] { HotkeySignal }, 2, StringSplitOptions.None);

            if (parts.Length == 0 || parts[0].Length == 0)
                throw new ParseException("Blank hotkey definition");

            if (hotstring)
                parts[0] = string.Concat(mode, parts[0]);

            
            string name = Script.LabelMethodName(parts[0]);
            string cond = HotkeyConditionId();
            if (cond.Length != 0)
                name += "_" + cond;
            PushLabel(lines[index], name, parts[0], false);

            if (parts.Length > 0 && !IsEmptyStatement(parts[1]))
            {
                bool remap = IsRemap(parts[1]);

                if (hotstring)
                {
                    var send = (CodeMethodInvokeExpression)InternalMethods.Send;
                    send.Parameters.Add(new CodePrimitiveExpression(remap ? parts[1].TrimStart(Spaces).Substring(0, 1) : parts[1]));
                    blocks.Pop().Statements.Add(send);
                }
                else
                {
                    lines.Insert(index + 1, new CodeLine(lines[index].FileName, lines[index].LineNumber, parts[1]));
                    lines[index].Code = string.Concat(parts[0], HotkeySignal);
                    blocks.Peek().Type = CodeBlock.BlockType.Expect;
                }
            }

            CodeMethodInvokeExpression invoke;

            if (hotstring)
            {
                invoke = (CodeMethodInvokeExpression)InternalMethods.Hotstring;
                invoke.Parameters.Add(new CodePrimitiveExpression(parts[0].Substring(mode.Length)));
                invoke.Parameters.Add(new CodePrimitiveExpression(name));

                string options = mode.Substring(1, mode.Length - 2);
                if (!string.IsNullOrEmpty(HotstringNewOptions))
                    options = string.Concat(HotstringNewOptions, SingleSpace.ToString(), options);
                invoke.Parameters.Add(new CodePrimitiveExpression(options));
            }
            else
            {
                invoke = (CodeMethodInvokeExpression)InternalMethods.Hotkey;
                invoke.Parameters.Add(new CodePrimitiveExpression(parts[0]));
                invoke.Parameters.Add(new CodePrimitiveExpression(name));
                invoke.Parameters.Add(new CodePrimitiveExpression(string.Empty));
            }

            prepend.Add(invoke);
            persistent = true;

            return new CodeMethodReturnStatement();
        }

        #region Conditional

        string HotkeyConditionId()
        {
            const string sep = ".";

            if (conditionIds == null)
            {
                conditionIds = new Dictionary<string, string>();
                conditionIds.Add(sep + sep + sep + sep + sep + sep + sep, string.Empty);
            }

            var criteria = string.Concat(
                IfWinActive_WinTitle, sep, IfWinActive_WinText, sep,
                IfWinExist_WinTitle, sep, IfWinExist_WinText, sep,
                IfWinNotActive_WinTitle, sep, IfWinNotActive_WinText, sep,
                IfWinNotExist_WinTitle, sep, IfWinNotExist_WinText);

            if (conditionIds.ContainsKey(criteria))
                return conditionIds[criteria];

            string id = InternalID;
            conditionIds.Add(criteria, id);
            return id;
        }

        #endregion
    }
}
