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
                EmitStatement(Statement);

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
                object[] Contents = Primitive.Value as object[];
                Type Element = T.GetElementType();
                Generator.Emit(OpCodes.Ldc_I4, Contents.Length);
                Generator.Emit(OpCodes.Newarr, Element);

                int i = 0;
                foreach(object Value in Contents)
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
                Generator.Emit(OpCodes.Ldstr, Value as string);
            }
            else if(T == typeof(int))
            {
                Debug("Pushing primitive integer : "+((int) Value));
                Generator.Emit(OpCodes.Ldc_I4, (int) Value);
            }
            else if(T == typeof(decimal))
            {
                Debug("Pushing decimal : "+((decimal) Value));
                Generator.Emit(OpCodes.Ldc_R4, ((float) ((decimal) Value)));
                Generated = typeof(float);
            }
            else if (T == typeof(object[]))
            {
                Debug("Pushing object[" + ((object[])Value).Length + "]");
                var array = new CodeArrayCreateExpression();
                array.CreateType = new CodeTypeReference(typeof(object));

                foreach (object sub in (object[])Value)
                    array.Initializers.Add(new CodePrimitiveExpression(sub));

                EmitDynamicName(array);
                Generated = typeof(object[]);
            }
            else if(T == typeof(bool))
            {
                bool val = (bool) Value;
                Debug("Pushing bool: "+Value);
                Generator.Emit(OpCodes.Ldc_I4, val ? 1 : 0);
                Generated = typeof(bool);
            }
            else if(T == typeof(double))
            {
                Debug("Pushing double: "+((double) Value));
                Generator.Emit(OpCodes.Ldc_R4, ((float) ((double) Value)));
                Generated = typeof(double);
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
