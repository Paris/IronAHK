using System;
using System.CodeDom;

namespace IronAHK.Scripting
{
    partial class Emit
    {
        void EmitExpression(CodeExpression expr)
        {
            if (expr is CodeMethodInvokeExpression)
                EmitInvoke((CodeMethodInvokeExpression)expr);
            else if (expr is CodeArrayCreateExpression)
                EmitArray((CodeArrayCreateExpression)expr);
            else if (expr is CodeComplexVariableReferenceExpression)
                EmitComplexReference((CodeComplexVariableReferenceExpression)expr);
            else if (expr is CodePrimitiveExpression)
                EmitPrimitive((CodePrimitiveExpression)expr);
            else if (expr is CodeBinaryOperatorExpression)
                EmitBinary((CodeBinaryOperatorExpression)expr);
            else if (expr is CodeTernaryOperatorExpression)
                EmitTernary((CodeTernaryOperatorExpression)expr);
            else if (expr is CodeVariableReferenceExpression)
                EmitVariableReference((CodeVariableReferenceExpression)expr);
            else if (expr is CodeFieldReferenceExpression)
                EmitFieldReference((CodeFieldReferenceExpression)expr);
            else if (expr is CodeTypeReferenceExpression)
                EmitTypeReference((CodeTypeReferenceExpression)expr);
            else
                throw new ArgumentException("Unrecognised expression: " + expr.GetType().ToString());
        }

        #region Methods

        void EmitInvoke(CodeMethodInvokeExpression invoke)
        {
            if (invoke.Method.TargetObject is CodeTypeReferenceExpression &&
                Type.GetType(((CodeTypeReferenceExpression)invoke.Method.TargetObject).Type.BaseType) == typeof(Script))
            {
                string name = invoke.Method.MethodName;

                if (name == Parser.InternalMethods.LabelCall.MethodName && invoke.Parameters.Count == 1)
                {
                    EmitGoto(new CodeGotoStatement((string)((CodePrimitiveExpression)invoke.Parameters[0]).Value));
                    return;
                }
                else if (name == Parser.InternalMethods.IfElse.MethodName && invoke.Parameters.Count == 1)
                {
                    EmitExpression(invoke.Parameters[0]);
                    return;
                }
                else if (name == Parser.InternalMethods.Operate.MethodName && invoke.Parameters.Count == 3)
                {
                    EmitExpression(invoke.Parameters[1]);
                    writer.Write(' ');

                    var op = (Script.Operator)Enum.Parse(typeof(Script.Operator), ((CodeFieldReferenceExpression)invoke.Parameters[0]).FieldName);
                    writer.Write(ScriptOperator(op));

                    writer.Write(' ');
                    EmitExpression(invoke.Parameters[2]);
                    return;
                }
            }

            if (invoke.Method.TargetObject != null && 
                !(invoke.Method.TargetObject is CodeTypeReferenceExpression && IsInternalType((CodeTypeReferenceExpression)invoke.Method.TargetObject)))
            {
                depth++;
                EmitExpression(invoke.Method.TargetObject);
                writer.Write('.');
                depth--;
            }

            writer.Write(invoke.Method.MethodName);
            writer.Write('(');

            for (int i = 0; i < invoke.Parameters.Count; i++)
            {
                depth++;
                if (i > 0)
                    writer.Write(", ");
                EmitExpression(invoke.Parameters[i]);
                depth--;
            }

            writer.Write(')');
        }

        void EmitReturn(CodeMethodReturnStatement returns)
        {
            writer.Write("return");

            if (returns.Expression != null)
            {
                writer.Write(' ');
                depth++;
                EmitExpression(returns.Expression);
                depth--;
            }
        }

        #endregion

        #region Variables

        void EmitVariableDeclaration(CodeVariableDeclarationStatement var)
        {
            writer.Write(var.Name);
            writer.Write(" := ");

            if (var.InitExpression == null)
                writer.Write("null");
            else
            {
                depth++;
                EmitExpression(var.InitExpression);
                depth--;
            }
        }

        void EmitVariableReference(CodeVariableReferenceExpression var)
        {
            writer.Write(var.VariableName);
        }

        void EmitAssignment(CodeAssignStatement assign)
        {
            EmitExpression(assign.Left);
            writer.Write(" := ");
            EmitExpression(assign.Right);
        }

        void EmitArray(CodeArrayCreateExpression array)
        {
            writer.Write('[');

            depth++;
            foreach (CodeExpression expr in array.Initializers)
                EmitExpression(expr);
            depth--;

            writer.Write(']');
        }

        void EmitFieldReference(CodeFieldReferenceExpression field)
        {
            if (field.TargetObject != null)
            {
                depth++;
                EmitExpression(field.TargetObject);
                depth--;
            }

            writer.Write(field.FieldName);
        }

        #region Complex

        void EmitComplexReference(CodeComplexVariableReferenceExpression var)
        {
            var name = var.QualifiedName;

            if (name is CodePrimitiveExpression)
            {
                writer.Write((string)(((CodePrimitiveExpression)name).Value));
            }
            else if (name is CodeArrayCreateExpression)
            {
                var array = (CodeArrayCreateExpression)name;

                foreach (CodeExpression part in array.Initializers)
                {
                    if (part is CodePrimitiveExpression)
                        EmitPrimitive((CodePrimitiveExpression)part);
                    else if (part is CodeComplexVariableReferenceExpression)
                    {
                        writer.Write('%');
                        EmitComplexReference((CodeComplexVariableReferenceExpression)part);
                        writer.Write('%');
                    }
                    else
                        throw new ArgumentException("var");
                }
            }
            else
                throw new ArgumentException("var");
        }

        #endregion

        #endregion

        #region Operators

        void EmitBinary(CodeBinaryOperatorExpression binary)
        {
            writer.Write('(');

            depth++;
            EmitExpression(binary.Left);
            depth--;

            writer.Write(' ');
            writer.Write(Operator(binary.Operator));
            writer.Write(' ');

            depth++;
            EmitExpression(binary.Right);
            depth--;

            writer.Write(')');
        }

        void EmitTernary(CodeTernaryOperatorExpression ternary)
        {
            depth++;
            EmitExpression(ternary.Condition);
            depth--;
            writer.Write('?');

            depth++;
            EmitExpression(ternary.TrueBranch);
            depth--;
            writer.Write(':');

            depth++;
            EmitExpression(ternary.FalseBranch);
            depth--;
        }

        #endregion

        #region Misc

        void EmitPrimitive(CodePrimitiveExpression primitive)
        {
            if (primitive.Value == null)
                writer.Write("null");
            else if (primitive.Value is string)
            {
                writer.Write('"');
                writer.Write((string)primitive.Value);
                writer.Write('"');
            }
            else if (primitive.Value is decimal)
                writer.Write(((decimal)primitive.Value).ToString());
            else if (primitive.Value is double)
                writer.Write(((double)primitive.Value).ToString());
            else if (primitive.Value is float)
                writer.Write(((float)primitive.Value).ToString());
            else if (primitive.Value is int)
                writer.Write(((int)primitive.Value).ToString());
            else if (primitive.Value is bool)
                writer.Write(((bool)primitive.Value) ? "true" : "false");
            else
                throw new ArgumentException("Unrecognised primitive: " + primitive.Value.ToString());
        }

        void EmitTypeReference(CodeTypeReferenceExpression type)
        {
            if (IsInternalType(type))
                return;

            writer.Write(type.Type.BaseType);
        }

        bool IsInternalType(CodeTypeReferenceExpression type)
        {
            return type.Type.BaseType == typeof(Rusty.Core).FullName;
        }

        #endregion
    }
}
