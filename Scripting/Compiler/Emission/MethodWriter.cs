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
            EmitStatement(Statement, true);
        }

        void EmitStatement(CodeStatement Statement, bool ForceTypes)
        {
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
            Depth++;
            Type Generated;

            if(Primitive.Value is string)
            {
                Debug("Pushing primitive string : \""+(Primitive.Value as string)+"\"");
                Generator.Emit(OpCodes.Ldstr, Primitive.Value as string);
                Generated = typeof(string);
            }
            else if(Primitive.Value is int)
            {
                Debug("Pushing primitive integer : "+((int)Primitive.Value));
                Generator.Emit(OpCodes.Ldc_I4, (int)Primitive.Value);
                Generated = typeof(int);
            }
            else if(Primitive.Value is decimal)
            {
                Debug("Pushing decimal : "+((decimal) Primitive.Value));
                Generator.Emit(OpCodes.Ldc_R4, ((float) ((decimal) Primitive.Value)));
                Generated = typeof(float);
            }
            else
            {
                Debug("Unhandled primitive: "+Primitive.Value.GetType());
                Generated = null;
            }
            Depth--;

            return Generated;
        }
    }
}
