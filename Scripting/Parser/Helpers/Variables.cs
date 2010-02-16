using System;
using System.CodeDom;
using System.Collections.Generic;
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
            if (name.Equals("A_LineNumber", StringComparison.OrdinalIgnoreCase))
                return new CodePrimitiveExpression(line);
            else if (name.Equals("A_LineFile", StringComparison.OrdinalIgnoreCase))
                return new CodePrimitiveExpression(fileName);
            else
                return VarId(name);
        }
        
        #endregion

        #region Wrappers

        CodeComplexVariableReferenceExpression VarId(string name)
        {
            return VarId(VarNameOrBasicString(name, true));
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

#pragma warning disable 0162
            if (UseComplexVar)
                return (CodeComplexVariableReferenceExpression)var;
            else
                return (CodeMethodInvokeExpression)(CodeComplexVariableReferenceExpression)var;
#pragma warning restore 0162
        }

        CodeExpression ComplexVarAssign(object var)
        {
            if (!(var is CodeComplexAssignStatement))
                throw new ArgumentException();

#pragma warning disable 0162
            if (UseComplexVar)
                return (CodeBinaryOperatorExpression)(CodeComplexAssignStatement)var;
            else
                return (CodeExpression)var;
#pragma warning restore 0162
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

            foreach (CodeExpression part in parts)
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

        #endregion
    }
}
