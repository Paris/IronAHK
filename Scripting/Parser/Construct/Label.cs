using System.CodeDom;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        CodeMethodInvokeExpression ParseLabel(CodeLine line)
        {
            string code = line.Code;
            int z = code.Length - 1;
            string name = z > 0 ? code.Substring(0, z) : string.Empty;
            if (code.Length < 2 || code[z] != HotkeyBound)
                throw new ParseException("Invalid label name");

            PushLabel(line, name, name, true);

            return LocalLabelInvoke(name);
        }

        void PushLabel(CodeLine line, string name, string realname, bool fallthrough)
        {
            var last = CloseTopLabelBlock();

            if (fallthrough && last != null)
                last.Statements.Add(LocalLabelInvoke(name));

            var method = LocalMethod(name);
            var block = new CodeBlock(line, method.Name, method.Statements, CodeBlock.BlockKind.Label, blocks.Count == 0 ? null : blocks.Peek())
            {
                Type = CodeBlock.BlockType.Within,
                Name = realname
            };
            CloseTopSingleBlock();
            blocks.Push(block);

            methods.Add(method.Name, method);
        }
    }
}
