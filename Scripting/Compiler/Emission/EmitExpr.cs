using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection.Emit;
using System.Reflection;

namespace IronAHK.Scripting
{
    internal partial class MethodWriter
    {
        void EmitExpressionStatement(CodeExpressionStatement Expression)
        {
            Depth++;
            Debug("Emitting expression statement");
            EmitExpression(Expression.Expression);
            Depth--;
        }

        void EmitExpression(CodeExpression Expression)
        {
            EmitExpression(Expression, 0);
        }

        void EmitExpression(CodeExpression Expression, int Get)
        {
            Depth++;
            Debug("Emitting expression");

            if(Expression is CodeMethodInvokeExpression)
            {
                EmitMethodInvoke(Expression as CodeMethodInvokeExpression);
            }
            else if(Expression is CodeArrayCreateExpression)
            {
                EmitDynamicName(Expression as CodeArrayCreateExpression);
            }
            else if(Expression is CodePrimitiveExpression)
            {
                EmitPrimitive(Expression as CodePrimitiveExpression);
            }
            else if(Expression is CodeComplexVariableReferenceExpression)
            {
                EmitComplexVariable(Expression as CodeComplexVariableReferenceExpression, Get);
            }
            else if(Expression is CodeBinaryOperatorExpression)
            {
                EmitBinaryOperator(Expression as CodeBinaryOperatorExpression);
            }
            else if (Expression is CodeAssignExpression)
            {
                EmitAssignExpression(Expression as CodeAssignExpression);
            }
            else if(Expression is CodeVariableReferenceExpression)
            {
                var Expr = Expression as CodeVariableReferenceExpression;
                if(!Locals.ContainsKey(Expr.VariableName))
                    throw new CompileException(Expr, "Undefined variable: "+Expr.VariableName);

                LocalBuilder Builder = Locals[Expr.VariableName];
                Generator.Emit(OpCodes.Ldloc, Builder);
            }
            else
            {
                Depth++;
                Debug("Unhandled expression: "+Expression.GetType());
                Depth--;
            }

            Depth--;
        }

        void EmitBinaryOperator(CodeBinaryOperatorExpression Binary)
        {
            Depth++;
            Debug("Emitting binary operator, left hand side");
            EmitExpression(Binary.Left);
            Debug("Emitting binary operator, right hand side");
            EmitExpression(Binary.Right);

            switch(Binary.Operator)
            {
                case CodeBinaryOperatorType.Add:
                    Generator.Emit(OpCodes.Add);
                    break;

                case CodeBinaryOperatorType.Subtract:
                    Generator.Emit(OpCodes.Sub);
                    break;

                case CodeBinaryOperatorType.Multiply:
                    Generator.Emit(OpCodes.Mul);
                    break;

                case CodeBinaryOperatorType.Divide:
                    Generator.Emit(OpCodes.Div);
                    break;

                case CodeBinaryOperatorType.LessThan:
                    Generator.Emit(OpCodes.Clt);
                    break;

                case CodeBinaryOperatorType.GreaterThan:
                    Generator.Emit(OpCodes.Cgt);
                    break;

                case CodeBinaryOperatorType.ValueEquality:
                    Generator.Emit(OpCodes.Ceq);
                    break;

                default:
                    Debug("Unhandled operator: "+Binary.Operator);
                    break;
            }

            Depth--;
        }
    }
}
