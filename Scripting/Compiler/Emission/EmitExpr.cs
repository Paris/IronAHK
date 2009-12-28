using System;
using System.CodeDom;
using System.Reflection;
using System.Reflection.Emit;

namespace IronAHK.Scripting
{
    partial class MethodWriter
    {
        Type EmitExpressionStatement(CodeExpressionStatement Expression, bool ForceTypes)
        {
            Depth++;
            Debug("Emitting expression statement");
            Type Generated = EmitExpression(Expression.Expression, ForceTypes);

            if (Generated != typeof(void))
                Generator.Emit(OpCodes.Pop);

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
            else if(Expression is CodeBinaryOperatorExpression)
            {
                Generated = EmitBinaryOperator(Expression as CodeBinaryOperatorExpression, ForceTypes);
            }
            else if(Expression is CodeVariableReferenceExpression)
            {
                Generated = EmitVariableReference(Expression as CodeVariableReferenceExpression);
            }
            else if (Expression is CodeFieldReferenceExpression)
            {
                Generated = EmitCodeFieldReference(Expression as CodeFieldReferenceExpression);
            }
            else if(Expression is CodeArgumentReferenceExpression)
            {
                EmitArgumentReference(Expression as CodeArgumentReferenceExpression);
                Generated = typeof(object[]);
            }
            else
            {
                Depth++;
                Debug("Unhandled expression: " + Expression.GetType());
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

        Type EmitCodeFieldReference(CodeFieldReferenceExpression field)
        {
            Depth++;
            Debug("Emitting field reference: " + field.FieldName);

            Type target = Type.GetType((field.TargetObject as CodeTypeReferenceExpression).Type.BaseType);
            FieldInfo fi = target.GetField(field.FieldName);

            try
            {
                Generator.Emit(OpCodes.Ldc_I4, (int)fi.GetValue(null));
            }
            catch (InvalidCastException)
            {
                throw new CompileException(field, "Enumerator " + target.Name + " does not have base type of int32");
            }

            Depth--;
            return fi.FieldType;
        }
    }
}
