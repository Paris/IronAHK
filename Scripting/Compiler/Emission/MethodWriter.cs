using System;
using System.CodeDom;
using System.Reflection.Emit;

namespace IronAHK.Scripting
{
    partial class MethodWriter
    {
        public void Emit()
        {
            EmitStatementCollection(Member.Statements);

            if(Method.ReturnType != typeof(void))
                Generator.Emit(OpCodes.Ldstr, "");
            Generator.Emit(OpCodes.Ret);

            CheckLabelsResolved();
        }

        void EmitStatementCollection(CodeStatementCollection Statements)
        {
            if(Depth != 0) Depth++;

            Debug("Emitting statements [Depth: "+Loops.Count+", Length: "+Statements.Count+"]");
            foreach(CodeStatement Statement in Statements)
            {
                EmitStatement(Statement);
            }

            Depth--;
        }

        void EmitStatement(CodeStatement Statement)
        {
            EmitStatement(Statement, true);
        }

        void EmitStatement(CodeStatement Statement, bool ForceTypes)
        {
            if(Statement == null) return;

            Depth++;
            Debug("Emitting statement");
            if(Statement is CodeAssignStatement)
            {
                EmitAssignStatement(Statement as CodeAssignStatement, ForceTypes);
            }
            else if(Statement is CodeExpressionStatement)
            {
                EmitExpressionStatement(Statement as CodeExpressionStatement, ForceTypes);
            }
            else if(Statement is CodeIterationStatement)
            {
                EmitIterationStatement(Statement as CodeIterationStatement);
            }
            else if(Statement is CodeConditionStatement)
            {
                EmitConditionStatement(Statement as CodeConditionStatement);
            }
            else if(Statement is CodeGotoStatement)
            {
                EmitGotoStatement(Statement as CodeGotoStatement);
            }
            else if(Statement is CodeLabeledStatement)
            {
                EmitLabeledStatement(Statement as CodeLabeledStatement);
            }
            else if(Statement is CodeMethodReturnStatement)
            {
                EmitReturnStatement(Statement as CodeMethodReturnStatement);
            }
            else if(Statement is CodeVariableDeclarationStatement)
            {
                EmitVariableDeclarationStatement(Statement as CodeVariableDeclarationStatement);
            }
            else
            {
                Depth++;
                Debug("Unhandled statement: "+Statement.GetType());
                Depth--;
            }
            Depth--;
        }

        Type EmitPrimitive(CodePrimitiveExpression Primitive)
        {
            Type T;
            if(Primitive.Value == null) T = null;
            else T = Primitive.Value.GetType();

            if(T != null && T.IsArray)
            {
                var Contents = Primitive.Value as object[];
                Type Element = T.GetElementType();
                Generator.Emit(OpCodes.Ldc_I4, Contents.Length);
                Generator.Emit(OpCodes.Newarr, Element);

                int i = 0;
                foreach(var Value in Contents)
                {
                    Generator.Emit(OpCodes.Dup);
                    Generator.Emit(OpCodes.Ldc_I4, i);

                    EmitLiteral(Element, Value);
                    Generator.Emit(OpCodes.Stelem_Ref);

                    i++;
                }

                return T;
            }
            else return EmitLiteral(T, Primitive.Value);
        }

        Type EmitLiteral(Type T, object Value)
        {
            Depth++;
            Type Generated = T;

            if (Value == null)
            {
                Debug("Pushing null");
                Generator.Emit(OpCodes.Ldnull);
                Generated = typeof(Nullable);
            }
            else if(T == typeof(string))
            {
                Debug("Pushing primitive string : \""+(Value as string)+"\"");

                if (((string)Value).Length == 0)
                    Generator.Emit(OpCodes.Ldsfld, typeof(string).GetField("Empty"));
                else
                    Generator.Emit(OpCodes.Ldstr, Value as string);
            }
            else if(T == typeof(int))
            {
                var val = (int)Value;
                Debug("Pushing primitive integer : " + val);

                switch (val)
                {
                    case -1: Generator.Emit(OpCodes.Ldc_I4_M1); break;
                    case 0: Generator.Emit(OpCodes.Ldc_I4_0); break;
                    case 1: Generator.Emit(OpCodes.Ldc_I4_1); break;
                    case 2: Generator.Emit(OpCodes.Ldc_I4_2); break;
                    case 3: Generator.Emit(OpCodes.Ldc_I4_3); break;
                    case 4: Generator.Emit(OpCodes.Ldc_I4_4); break;
                    case 5: Generator.Emit(OpCodes.Ldc_I4_5); break;
                    case 6: Generator.Emit(OpCodes.Ldc_I4_6); break;
                    case 7: Generator.Emit(OpCodes.Ldc_I4_7); break;
                    case 8: Generator.Emit(OpCodes.Ldc_I4_8); break;
                    default: Generator.Emit(OpCodes.Ldc_I4, val); break;
                }
            }
            else if(T == typeof(decimal))
            {
                Debug("Pushing decimal : "+((decimal) Value));
                
                // HACK:  push real decimals without downcasting
                // i.e. new decimal(decimal.GetBits((decimal)Value));
                
                Generator.Emit(OpCodes.Ldc_R8, ((double)((decimal)Value)));
                Generated = typeof(double);
            }
            else if (T == typeof(object[]))
            {
                Debug("Pushing object[" + ((object[])Value).Length + "]");
                var array = new CodeArrayCreateExpression();
                array.CreateType = new CodeTypeReference(typeof(object));

                foreach (var sub in (object[])Value)
                    array.Initializers.Add(new CodePrimitiveExpression(sub));

                EmitDynamicName(array);
            }
            else if(T == typeof(bool))
            {
                var val = (bool) Value;
                Debug("Pushing bool: "+Value);
                
                Generator.Emit(val ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
            }
            else if(T == typeof(double))
            {
                Debug("Pushing double: "+((double) Value));
                Generator.Emit(OpCodes.Ldc_R8, ((double)Value));
            }
            else if (T == typeof(long))
            {
                Debug("Pushing long: " + (long)Value);
                Generator.Emit(OpCodes.Ldc_I8, (long)Value);
            }
            else if (T.IsGenericType && T.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                Debug("Pushing nullable: " + Value);
                EmitLiteral(Value == null ? typeof(Nullable) : T.GetGenericArguments()[0], Value);
            }
            else
            {
                Debug("Unhandled primitive: " + T);
                Generated = null;
            }
            Depth--;

            return Generated;
        }
    }
}
