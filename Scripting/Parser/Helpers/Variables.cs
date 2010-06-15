using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        #region Names

        CodeExpression VarNameOrBasicString(string code, bool asValue)
        {
            code = EscapedString(code, true);

            if (asValue)
            {
                object result;
                if (IsPrimativeObject(code, out result))
                    return new CodePrimitiveExpression(result);

                if (code.IndexOf(Resolve) == -1)
                    return new CodePrimitiveExpression(code);
            }

            var parts = new List<CodeExpression>();
            var sub = new StringBuilder();
            bool id = false;

            for (int i = 0; i < code.Length; i++)
            {
                char sym = code[i];

                if (sym == Resolve && (i == 0 || code[i - 1] != Escape))
                {
                    if (id)
                    {
                        if (sub.Length == 0)
                            throw new ParseException(ExEmptyVarRef, i);
                        parts.Add(ComplexVarRef(VarIdOrConstant(sub.ToString())));
                        sub.Length = 0;
                        id = false;
                    }
                    else
                    {
                        parts.Add(new CodePrimitiveExpression(sub.ToString()));
                        sub.Length = 0;
                        id = true;
                    }
                }
                else if (id && !IsIdentifier(sym))
                    throw new ParseException(ExInvalidVarToken, i);
                else
                    sub.Append(sym);
            }

            if (sub.Length != 0)
                parts.Add(new CodePrimitiveExpression(sub.ToString()));

            if (parts.Count == 1)
                return new CodePrimitiveExpression(code);

            CodeExpression[] all = parts.ToArray();

            if (asValue)
                return StringConcat(all);
            else
                return ComplexVarRef(new CodeComplexVariableReferenceExpression(all));
        }

        CodeExpression VarIdOrConstant(string name)
        {
            switch (name.ToLowerInvariant())
            {
                case "a_linenumber":
                    return new CodePrimitiveExpression(line);

                case "a_linefile":
                    return new CodePrimitiveExpression(Path.GetFullPath(fileName));

                case "a_thisfunc":
                    return new CodePrimitiveExpression(Scope);

                case "a_thislabel":
                    {
                        if (blocks.Count == 0)
                            return new CodePrimitiveExpression(string.Empty);

                        var all = blocks.ToArray();
                        for (int i = all.Length - 1; i > -1; i--)
                            if (all[i].Kind == CodeBlock.BlockKind.Label)
                                return new CodePrimitiveExpression(all[i].Name);
                    }
                    return new CodePrimitiveExpression(string.Empty);

                default:
                    return VarId(name);
            }
        }
        
        #endregion

        #region Wrappers

        CodeComplexVariableReferenceExpression VarId(string name)
        {
            return VarId(VarNameOrBasicString(name.ToLowerInvariant(), true));
        }

        CodeComplexVariableReferenceExpression VarId(CodeExpression name)
        {
            return new CodeComplexVariableReferenceExpression(new CodePrimitiveExpression(Scope + ScopeVar), name);
        }

        #endregion

        #region Complex

        CodeExpression ComplexVarRef(object var)
        {
            if (var is CodePrimitiveExpression)
                return (CodePrimitiveExpression)var;

            if (!(var is CodeComplexVariableReferenceExpression))
                throw new ArgumentException();

            return (CodeComplexVariableReferenceExpression)var;
        }

        CodeExpression ComplexVarAssign(object var)
        {
            if (!(var is CodeComplexAssignStatement))
                throw new ArgumentException();

            return (CodeBinaryOperatorExpression)(CodeComplexAssignStatement)var;
        }

        CodeExpression WrappedComplexVar(object part)
        {
            return part is CodeComplexVariableReferenceExpression ? ComplexVarRef(part) :
                part is CodeComplexAssignStatement ? ComplexVarAssign(part) : (CodeExpression)part;
        }

        #endregion

        #region Misc

        CodeExpression StringConcat(params CodeExpression[] parts)
        {
            var list = new List<CodeExpression>(parts.Length);

            foreach (var part in parts)
            {
                if (part is CodePrimitiveExpression)
                {
                    var value = ((CodePrimitiveExpression)part).Value;
                    if (value is string && ((string)value).Length == 0)
                        continue;
                }

                list.Add(part);
            }

            if (list.Count == 1)
                return list[0];

            Type str = typeof(string);
            var method = (CodeMethodReferenceExpression)InternalMethods.Concat;
            var all = new CodeArrayCreateExpression(str, list.ToArray());
            return new CodeMethodInvokeExpression(method, all);
        }

        CodeComplexVariableReferenceExpression InternalVariable
        {
            get { return new CodeComplexVariableReferenceExpression(new CodePrimitiveExpression(".\0")); }
        }

        #endregion
    }
}
