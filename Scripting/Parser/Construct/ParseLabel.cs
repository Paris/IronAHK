using System.CodeDom;
using System.Collections.Generic;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        CodeMethodInvokeExpression ParseLabel(CodeLine line)
        {
            #region Name
            
            string code = line.Code;
            int z = code.Length - 1;
            string name = z > 0 ? code.Substring(0, z) : null;
            if (code.Length < 2 || code[z] != HotkeyBound || !IsIdentifier(name))
                throw new ParseException("Invalid label name");

            #endregion

            #region Fall through labels

            var last = CheckTopBlock();
            if (last != null)
                last.Statements.Add(LocalLabelInvoke(name));

            #endregion

            #region Block

            var method = LocalMethod(name);
            var block = new CodeBlock(line, method.Name, method.Statements, CodeBlock.BlockKind.Label) { Type = CodeBlock.BlockType.Within };
            blocks.Push(block);

            #endregion

            methods.Add(method.Name, method);

            return LocalLabelInvoke(name);
        }

        CodeMethodInvokeExpression LocalLabelInvoke(string name)
        {
            var invoke = LocalMethodInvoke(name);
            invoke.Parameters.Add(new CodePrimitiveExpression(null));
            return invoke;
        }

        CodeBlock CheckTopBlock()
        {
            if (blocks.Count != 0 && blocks.Peek().Kind == CodeBlock.BlockKind.Label)
                return blocks.Pop();
            return null;
        }
    }
}
