using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection.Emit;
using System.Reflection;

namespace IronAHK.Scripting
{
    internal partial class MethodWriter
    {
        Type EmitExpressionStatement(CodeExpressionStatement Expression, bool ForceTypes)
        {
            Depth++;
            Debug("Emitting expression statement");
            Type Generated = EmitExpression(Expression.Expression, ForceTypes);
            Depth--;

            return Generated;
        }

        Type EmitExpression(CodeExpression Expression, bool ForceTypes)
        {
            Depth++;
            Debug("Emitting expression");
            Type Generated;

            if(Expression is CodeMethodInvokeExpression)
            {
                Generated = EmitMethodInvoke(Expression as CodeMethodInvokeExpression);
            }
            else if(Expression is CodeArrayCreateExpression)
            {
                EmitDynamicName(Expression as CodeArrayCreateExpression);
                Generated = typeof(string[]);
            }
            else if(Expression is CodePrimitiveExpression)
            {
                Generated = EmitPrimitive(Expression as CodePrimitiveExpression);
            }
            else if(Expression is CodeComplexVariableReferenceExpression)
            {
                EmitComplexVariable(Expression as CodeComplexVariableReferenceExpression, false);
                Generated = typeof(object);
            }
            else if(Expression is CodeBinaryOperatorExpression)
            {
                Generated = EmitBinaryOperator(Expression as CodeBinaryOperatorExpression, ForceTypes);
            }
            else if (Expression is CodeAssignExpression)
            {
                EmitAssignExpression(Expression as CodeAssignExpression);
                Generated = null;
            }
            else if(Expression is CodeVariableReferenceExpression)
            {
                var Expr = Expression as CodeVariableReferenceExpression;
                if(!Locals.ContainsKey(Expr.VariableName))
                    throw new CompileException(Expr, "Undefined variable: "+Expr.VariableName);

                LocalBuilder Builder = Locals[Expr.VariableName];
                Generator.Emit(OpCodes.Ldloc, Builder);

                Generated = Builder.LocalType;
            }
            else
            {
                Depth++;
                Debug("Unhandled expression: "+Expression.GetType());
                Generated = null;
                Depth--;
            }

            Depth--;

            return Generated;
        }

        Type EmitExpression(CodeExpression Expression)
        {
            return EmitExpression(Expression, true);
        }

        Type EmitBinaryOperator(CodeBinaryOperatorExpression Binary, bool ForceTypes)
        {
            Type Generated;
            Depth++;
            Debug("Emitting binary operator, left hand side");
            Generated = EmitExpression(Binary.Left);
            if(ForceTypes) ForceTopStack(Generated, typeof(float));
            Debug("Emitting binary operator, right hand side");
            Generated = EmitExpression(Binary.Right);
            if(ForceTypes) ForceTopStack(Generated, typeof(float));

            switch(Binary.Operator)
            {
                case CodeBinaryOperatorType.Add:
                    Generator.Emit(OpCodes.Add);
                    Generated = typeof(float);
                    break;

                case CodeBinaryOperatorType.Subtract:
                    Generator.Emit(OpCodes.Sub);
                    Generated = typeof(float);
                    break;

                case CodeBinaryOperatorType.Multiply:
                    Generator.Emit(OpCodes.Mul);
                    Generated = typeof(float);
                    break;

                case CodeBinaryOperatorType.Divide:
                    Generator.Emit(OpCodes.Div);
                    Generated = typeof(float);
                    break;

                case CodeBinaryOperatorType.LessThan:
                    Generator.Emit(OpCodes.Clt);
                    Generated = typeof(bool);
                    break;

                case CodeBinaryOperatorType.GreaterThan:
                    Generator.Emit(OpCodes.Cgt);
                    Generated = typeof(bool);
                    break;

                case CodeBinaryOperatorType.ValueEquality:
                    Generator.Emit(OpCodes.Ceq);
                    Generated = typeof(bool);
                    break;

                default:
                    Debug("Unhandled operator: "+Binary.Operator);
                    Generated = null;
                    break;
            }

            Depth--;

            return Generated;
        }
    }
}
