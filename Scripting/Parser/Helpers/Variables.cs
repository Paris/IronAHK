using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        #region Name

        string VarNormalisedName(string name)
        {
            return name.ToLowerInvariant();
        }

        CodeArrayIndexerExpression VarId(string name)
        {
            return VarId(VarIdExpand(VarNormalisedName(name)));
        }

        CodeArrayIndexerExpression VarId(CodeExpression name)
        {
            var scope = Scope + ScopeVar;

            if (name is CodePrimitiveExpression)
            {
                var raw = (CodePrimitiveExpression)name;

                if (raw.Value is string)
                    return VarRef(scope + (string)raw.Value);
            }

            return VarRef(StringConcat(new CodePrimitiveExpression(scope), name));
        }

        #endregion

        #region Reference

        CodeArrayIndexerExpression VarRef(params CodeExpression[] name)
        {
            var vars = new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(Script)), VarProperty);
            return new CodeArrayIndexerExpression(vars, name);
        }

        CodeArrayIndexerExpression VarRef(string name)
        {
            return VarRef(new CodePrimitiveExpression(name));
        }

        CodeBinaryOperatorExpression VarAssign(CodeArrayIndexerExpression name, CodeExpression value)
        {
            return new CodeBinaryOperatorExpression(name, CodeBinaryOperatorType.Assign, value);
        }

        #endregion

        #region Expansion

        CodeExpression VarIdExpand(string code)
        {
            code = EscapedString(code, true);

            object result;

            if (IsPrimativeObject(code, out result))
                return new CodePrimitiveExpression(result);

            if (code.IndexOf(Resolve) == -1)
                return new CodePrimitiveExpression(code);

            if (!DynamicVars)
            {
                throw new ParseException(ExNoDynamicVars);
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
                        parts.Add(VarRefOrPrimitive(VarIdOrConstant(sub.ToString())));
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

            return StringConcat(all);
        }

        CodeExpression VarIdOrConstant(string name)
        {
            switch (name.ToLowerInvariant())
            {
                case "a_linenumber":
                    return new CodePrimitiveExpression(line);

                case "a_linefile":
                    return new CodePrimitiveExpression(Path.GetFullPath(fileName));

                case "a_scriptdir":
                    return new CodePrimitiveExpression(Path.GetDirectoryName(Path.GetFullPath(fileName)));

                case "a_scriptfullpath":
                    return new CodePrimitiveExpression(Path.GetFullPath(fileName));

                case "a_scriptname":
                    return new CodePrimitiveExpression(Path.GetFileName(Path.GetFullPath(fileName)));

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

        CodeArrayIndexerExpression InternalVariable
        {
            get { return VarRef(string.Concat(Scope, ScopeVar + "\0", InternalID)); }
        }

        #endregion

        #region Checks

        bool IsVarAssignment(object expr)
        {
            return expr is CodeBinaryOperatorExpression && ((CodeBinaryOperatorExpression)expr).Operator == CodeBinaryOperatorType.Assign;
        }

        bool IsVarReference(object expr)
        {
            return expr is CodeArrayIndexerExpression;
        }
        
        #endregion

        #region Casts

        CodeExpression VarRefOrPrimitive(object var)
        {
            if (var is CodePrimitiveExpression)
                return (CodePrimitiveExpression)var;

            if (!IsVarReference(var))
                throw new ArgumentException();

            return (CodeArrayIndexerExpression)var;
        }

        CodeExpression VarMixedExpr(object part)
        {
            return IsVarReference(part) ? VarRefOrPrimitive(part) : IsVarAssignment(part) ? (CodeBinaryOperatorExpression)part : part is CodeExpression ? (CodeExpression)part : new CodePrimitiveExpression(part);
        }

        #endregion
    }
}
