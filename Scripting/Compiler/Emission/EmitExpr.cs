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
            if(Expression == null)
                throw new ArgumentException("Expression can not be null", "Expression");
            
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
            else if(Expression is CodeTernaryOperatorExpression)
            {
                Generated = EmitTernaryOperator(Expression as CodeTernaryOperatorExpression);
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
            else if (Expression is CodeArrayIndexerExpression)
            {
                EmitArrayIndexerExpression(Expression as CodeArrayIndexerExpression);
                Generated = typeof(object);
            }
            else if (Expression is CodeDelegateCreateExpression)
            {
                Generated = EmitDelegateCreateExpression((CodeDelegateCreateExpression)Expression);
            }
            else if (Expression is CodePropertyReferenceExpression)
            {
                Generated = EmitPropertyReferenceExpression((CodePropertyReferenceExpression)Expression);
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

        Type EmitDelegateCreateExpression(CodeDelegateCreateExpression del)
        {
            Depth++;
            Debug("Emitting delegate: " + del.MethodName);

            // HACK: use generic method lookup for emitting delegates

            if (!Methods.ContainsKey(del.MethodName))
                throw new CompileException(del, "Delegate function does not exist in local scope");

            var method = (MethodInfo)Methods[del.MethodName].Method;
            Generator.Emit(OpCodes.Ldnull);
            Generator.Emit(OpCodes.Ldftn, method);

            var type = del.DelegateType.UserData[Parser.RawData] as Type ?? Type.GetType(del.DelegateType.BaseType);

            if (type == null)
                throw new CompileException(del, "Invalid delegate type");
            
            if(Mirror != null)
                type = Mirror.GrabType(type);
            
            var ctor = type.GetConstructors()[0];
            
            Generator.Emit(OpCodes.Newobj, ctor);

            Depth--;
           
            return type;
        }

        Type EmitPropertyReferenceExpression(CodePropertyReferenceExpression prop)
        {
            // HACK: property get method target
            var info = typeof(Rusty.Core).GetProperty(prop.PropertyName);

            if (Mirror != null)
                info = Mirror.GrabProperty(info);

            Generator.Emit(OpCodes.Call, info.GetGetMethod());
            return info.PropertyType;
        }
        
        void EmitArrayIndexerExpression(CodeArrayIndexerExpression Indexer)
        {
            var index = (CodeArrayIndexerExpression)Indexer;
   
            Generator.Emit(OpCodes.Ldloc, VarsProperty);
            EmitExpression(index.Indices[0]);
            
            Generator.Emit(OpCodes.Callvirt, GetVariable);
        }

        Type EmitBinaryOperator(CodeBinaryOperatorExpression Binary, bool ForceTypes)
        {
            bool Shortcut = Binary.Operator == CodeBinaryOperatorType.BooleanAnd || Binary.Operator == CodeBinaryOperatorType.BooleanOr;
            Label EndLabel = Generator.DefineLabel();
            
            if(Binary.Operator == CodeBinaryOperatorType.Assign)
                return EmitAssignment(Binary.Left, Binary.Right, ForceTypes);

            Type Generated;
            Depth++;
            Debug("Emitting binary operator, left hand side");
            Generated = EmitExpression(Binary.Left);

            if(Shortcut && Generated == typeof(bool))
            {
                Debug("Short-circuiting expression for "+Binary.Operator);
                Generator.Emit(OpCodes.Dup);

                if(Binary.Operator == CodeBinaryOperatorType.BooleanAnd)
                    Generator.Emit(OpCodes.Brfalse, EndLabel); // BooleanAnd jumps if left branch evals false
                else if(Binary.Operator == CodeBinaryOperatorType.BooleanOr)
                    Generator.Emit(OpCodes.Brtrue, EndLabel); // BooleanOr jumps if left branch evals true
            }

            if(ForceTypes) ForceTopStack(Generated, typeof(float));
            Debug("Emitting binary operator, right hand side");
            Generated = EmitExpression(Binary.Right);
            if(ForceTypes) ForceTopStack(Generated, typeof(float));

            if(Shortcut)
            {
                if(Binary.Operator == CodeBinaryOperatorType.BooleanAnd)
                    Generator.Emit(OpCodes.And);
                else if (Binary.Operator == CodeBinaryOperatorType.BooleanOr)
                    Generator.Emit(OpCodes.Or);

                // Handy side-effect: one bool caused by the "dup" stays on the stack
                // Resulting in the whole expression evaluating correctly anyway.
                Generator.MarkLabel(EndLabel);
                
                Depth--;
                return typeof(bool);
            }

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

        Type EmitTernaryOperator(CodeTernaryOperatorExpression ternary)
        {
            Label FalseLabel = Generator.DefineLabel();
            Label EndLabel = Generator.DefineLabel();
            Type Top;

            EmitExpression(ternary.Condition);
            Generator.Emit(OpCodes.Brfalse, FalseLabel);
            Top = EmitExpression(ternary.TrueBranch);
            ForceTopStack(Top, typeof(object));
            Generator.Emit(OpCodes.Br, EndLabel);
            Generator.MarkLabel(FalseLabel);
            Top = EmitExpression(ternary.FalseBranch);
            ForceTopStack(Top, typeof(object));
            Generator.MarkLabel(EndLabel);

            return typeof(object);
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
