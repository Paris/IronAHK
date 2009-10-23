using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection.Emit;
using System.Reflection;
using System.Collections.Generic;

namespace IronAHK.Scripting
{
    internal partial class MethodWriter
    {
        public void Emit()
        {
            EmitStatementCollection(Member.Statements);
            Generator.Emit(OpCodes.Ret);
        }

        void EmitStatementCollection(CodeStatementCollection Statements)
        {
            Debug("Emitting statements [Depth: "+Loops.Count+", Length: "+Statements.Count+"]");
            foreach(CodeStatement Statement in Statements)
                EmitStatement(Statement);
        }

        void EmitStatement(CodeStatement Statement)
        {
            Depth++;
            Debug("Emitting statement");
            if(Statement is CodeAssignStatement)
            {
                EmitAssignStatement(Statement as CodeAssignStatement);
            }
            else if(Statement is CodeExpressionStatement)
            {
                EmitExpressionStatement(Statement as CodeExpressionStatement);
            }
            else if(Statement is CodeIterationStatement)
            {
                EmitIterationStatement(Statement as CodeIterationStatement);
            }
            else if(Statement is CodeConditionStatement)
            {
                EmitConditionStatement(Statement as CodeConditionStatement);
            }
            else
            {
                Depth++;
                Debug("Unhandled statement: "+Statement.GetType());
                Depth--;
            }
            Depth--;
        }

        void EmitConditionStatement(CodeConditionStatement Condition)
        {
            Label False = Generator.DefineLabel();
            Label End = Generator.DefineLabel();

            // TODO: This could probably be done more efficiently
            EmitExpression(Condition.Condition);
            Generator.Emit(OpCodes.Brfalse, False);

            // Execute code for true and jump to end
            EmitStatementCollection(Condition.TrueStatements);
            Generator.Emit(OpCodes.Br, End);

            // Execute code for false and move on
            Generator.MarkLabel(False);
            EmitStatementCollection(Condition.FalseStatements);

            Generator.MarkLabel(End);
        }

        void EmitIterationStatement(CodeIterationStatement Iterate)
        {
            // Used for break, continue and A_Index later on
            LoopMetadata Meta = new LoopMetadata {
                Begin = Generator.DefineLabel(),
                End = Generator.DefineLabel(),
            };
            Loops.Push(Meta);

            EmitStatement(Iterate.InitStatement);

            // The beginning of our loop: check the limit
            Generator.MarkLabel(Meta.Begin);

            EmitExpression(Iterate.TestExpression);
            Generator.Emit(OpCodes.Brfalse, Meta.End);

            // Emit the actual statements within
            EmitStatementCollection(Iterate.Statements);

            // Increase the counter by one
            EmitStatement(Iterate.IncrementStatement);

            // Start all over again
            Generator.Emit(OpCodes.Br, Meta.Begin);
            Generator.MarkLabel(Meta.End);

            Loops.Pop();
        }

        void EmitAssignExpression(CodeAssignExpression Expr)
        {
            EmitAssignment(Expr.Left, Expr.Right);
        }

        void EmitAssignStatement(CodeAssignStatement Assign)
        {
            EmitAssignment(Assign.Left, Assign.Right);
        }

        void EmitAssignment(CodeExpression Left, CodeExpression Right)
        {
            Depth++;
            Debug("Emitting assignment statement");

            if(Left is CodeComplexVariableReferenceExpression)
            {
                EmitComplexVariable(Left as CodeComplexVariableReferenceExpression, 3);
                EmitExpression(Right);
                if (Right is CodePrimitiveExpression && ((CodePrimitiveExpression)Right).Value is decimal)
                    Generator.Emit(OpCodes.Box, typeof(float));
                Generator.Emit(OpCodes.Call, SetEnv);
            }
            else if(Left is CodeVariableReferenceExpression)
            {
                var Reference = Left as CodeVariableReferenceExpression;
                
                LocalBuilder Var;
                if(Locals.ContainsKey(Reference.VariableName))
                    Var = Locals[Reference.VariableName];
                else
                {
                    Var = Generator.DeclareLocal(typeof(int));
                    Locals.Add(Reference.VariableName, Var);
                }

                EmitExpression(Right);
                Generator.Emit(OpCodes.Stloc, Var);
            }
            else throw new CompileException(Left, "Left hand is unassignable");


            Depth--;
        }

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

        void EmitComplexVariable(CodeComplexVariableReferenceExpression Complex, int Get)
        {
            Depth++;

            Debug("Emitting complex variable reference");

            Generator.Emit(OpCodes.Ldc_I4, Complex.Parts.Length);
            Generator.Emit(OpCodes.Newarr, typeof(string));

            if(Get > 0)
                Get--;

            for(int i = 0; i < Complex.Parts.Length; i++)
            {
                Generator.Emit(OpCodes.Dup);
                Generator.Emit(OpCodes.Ldc_I4, i);
                EmitExpression(Complex.Parts[i], Get);
                Generator.Emit(OpCodes.Stelem_Ref);
            }

            Generator.Emit(OpCodes.Call, typeof(string).GetMethod("Concat", new Type[] { typeof(string[]) }));

            Depth++;
            if(Get == 0)
            {
                Debug("Getting as instructed");
                Generator.Emit(OpCodes.Call, GetEnv);
            }
            else Debug("I'm a good boy and not getting");

            Depth--;
            Depth--;
        }

        void EmitPrimitive(CodePrimitiveExpression Primitive)
        {
            Depth++;
            if(Primitive.Value is string)
            {
                Debug("Pushing primitive string : \""+(Primitive.Value as string)+"\"");
                Generator.Emit(OpCodes.Ldstr, Primitive.Value as string);
            }
            else if(Primitive.Value is int)
            {
                Debug("Pushing primitive integer : "+((int)Primitive.Value));
                Generator.Emit(OpCodes.Ldc_I4, (int)Primitive.Value);
            }
            else if(Primitive.Value is decimal)
            {
                Debug("Pushing decimal : "+((decimal) Primitive.Value));
                Generator.Emit(OpCodes.Ldc_R4, ((float) ((decimal) Primitive.Value)));
            }
            else
            {
                Debug("Unhandled primitive: "+Primitive.Value.GetType());
            }
            Depth--;
        }

        void EmitDynamicName(CodeArrayCreateExpression Dynamic)
        {
            Depth++;
            Debug("Emitting dynamic name expression");
            // Bail if it's not a string, we only use it for that
            if(Dynamic.CreateType.BaseType != "System.String")
                throw new CompileException(Dynamic, "CodeArrayCreateExpression is only used for dynamic names. System.String only");

            Generator.Emit(OpCodes.Ldc_I4, Dynamic.Initializers.Count);
            Generator.Emit(OpCodes.Newarr, typeof(string));

            for(int i = 0; i < Dynamic.Initializers.Count; i++)
            {
                Generator.Emit(OpCodes.Dup);
                Generator.Emit(OpCodes.Ldc_I4, i);
                EmitExpression(Dynamic.Initializers[i]);
                Generator.Emit(OpCodes.Stelem_Ref);
            }
            Depth--;
        }

        #region Methods
        void EmitMethodInvoke(CodeMethodInvokeExpression Invoke)
        {
            if(Invoke.Method.MethodName == string.Empty)
                throw new CompileException(Invoke, "Empty method name");

            Depth++;
            Debug("Emitting method invoke expression for "+Invoke.Method.MethodName);
            ArgType[] Args = new ArgType[Invoke.Parameters.Count];

            Depth++;
            for(int i = 0; i < Args.Length; i++)
            {
                Debug("Emitting parameter "+i);
                EmitExpression(Invoke.Parameters[i]);
                Args[i] = ArgType.Expression;
            }
            Depth--;

            var Type = Invoke.Method.TargetObject as CodeTypeReferenceExpression;
            if(Type != null)
            {
                EmitCannedMethod(Type, Invoke);
                return;
            }

            MethodInfo Info = Lookup.BestMatch(Invoke.Method.MethodName, Args);

            if(Info == null)
                throw new CompileException(Invoke, "Could not look up method "+Invoke.Method.MethodName);

            Generator.Emit(OpCodes.Call, Info);
            Depth--;
        }

        // Method to quickly resolve methods emitted frequently by parser
        void EmitCannedMethod(CodeTypeReferenceExpression Type, CodeMethodInvokeExpression Invoke)
        {
            Depth++;
            Debug("Emitting canned method "+Invoke.Method.MethodName);
            if(Type.Type.BaseType == "System.String" && Invoke.Method.MethodName == "Concat")
            {
                Generator.Emit(OpCodes.Call, typeof(string).GetMethod("Concat", new Type[] { typeof(string[]) }));
            }
            else throw new CompileException(Invoke, "No canned method named "+Invoke.Method.MethodName+" for type "+Type.Type.BaseType);
            Depth--;
        }
        #endregion
    }
}
