using System;
using System.CodeDom;
using System.Reflection.Emit;

namespace IronAHK.Scripting
{
    partial class MethodWriter
    {
        Type EmitVariableReference(CodeVariableReferenceExpression Expr)
        {
            if(!Locals.ContainsKey(Expr.VariableName))
                throw new CompileException(Expr, "Undefined variable: "+Expr.VariableName);

            LocalBuilder Builder = Locals[Expr.VariableName];
            Generator.Emit(OpCodes.Ldloc, Builder);

            return Builder.LocalType;
        }

        void EmitVariableDeclarationStatement(CodeVariableDeclarationStatement Statement)
        {
            if(Locals.ContainsKey(Statement.Name))
                throw new CompileException(Statement, "Attempt to redefine local variable "+Statement.Name);

            Type Top = EmitExpression(Statement.InitExpression);
            LocalBuilder Local = Generator.DeclareLocal(Top);
            Locals.Add(Statement.Name, Local);

            Generator.Emit(OpCodes.Stloc, Local);
        }

        void EmitArgumentReference(CodeArgumentReferenceExpression Argument)
        {
            Depth++;
            Debug("Emitting argument reference");
            Generator.Emit(OpCodes.Ldarg, 0); // for now only used to refer to the sole object[] parameter
            Depth--;
        }

        void EmitAssignStatement(CodeAssignStatement Assign, bool ForceTypes)
        {
            EmitAssignment(Assign.Left, Assign.Right, ForceTypes);
        }

        void EmitAssignment(CodeExpression Left, CodeExpression Right, bool ForceTypes)
        {
            Depth++;
            Debug("Emitting assignment statement");

            if(Left is CodeVariableReferenceExpression)
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

                EmitExpression(Right, ForceTypes);
                Generator.Emit(OpCodes.Stloc, Var);
            }
            else throw new CompileException(Left, "Left hand is unassignable");

            Depth--;
        }

        void EmitDynamicName(CodeArrayCreateExpression Dynamic)
        {
            Depth++;
            Debug("Emitting dynamic name expression");
            // Bail if it's not a string, we only use it for that

            Type ElementType;
            if(Dynamic.CreateType.BaseType == "System.String")
                ElementType = typeof(string);
            else if(Dynamic.CreateType.BaseType == "System.Object")
                ElementType = typeof(object);
            else throw new CompileException(Dynamic, "Unable to create array of type "+Dynamic.CreateType.BaseType);

            Generator.Emit(OpCodes.Ldc_I4, Dynamic.Initializers.Count);
            Generator.Emit(OpCodes.Newarr, ElementType);

            for(int i = 0; i < Dynamic.Initializers.Count; i++)
            {
                Generator.Emit(OpCodes.Dup);
                Generator.Emit(OpCodes.Ldc_I4, i);
                Type Generated = EmitExpression(Dynamic.Initializers[i]);
                ForceTopStack(Generated, ElementType);

                Generator.Emit(OpCodes.Stelem_Ref);
            }
            Depth--;
        }

        string ResolveName(CodeVariableReferenceExpression Var)
        {
            const string sep = ".";
            if (IsEntryPoint)
                return string.Concat(sep, Var.VariableName);
            else
                return string.Concat(Member.Name, sep, Var.VariableName);
        }

        void ConditionalBox(Type Top)
        {
            if(Top != typeof(object)) Generator.Emit(OpCodes.Box, Top);
        }

        void ForceTopStack(Type Top, Type Wanted)
        {
            ForceTopStack(Top, Wanted, true);
        }

        void ForceTopStack(Type Top, Type Wanted, bool ForceTypes)
        {
            Depth++;
            if(Top != Wanted)
            {
                Debug("Forcing top stack "+Top+" to "+Wanted);
                if(Wanted == typeof(string))
                {
                    ConditionalBox(Top);
                    Generator.Emit(OpCodes.Call, ForceString);
                }
                else if (Wanted == typeof(decimal))
                {
                    ConditionalBox(Top);
                    Generator.Emit(OpCodes.Call, ForceDecimal);
                }
                else if (Wanted == typeof(long))
                {
                    ConditionalBox(Top);
                    Generator.Emit(OpCodes.Call, ForceLong);
                }
                else if (Wanted == typeof(int))
                {
                    ConditionalBox(Top);
                    Generator.Emit(OpCodes.Call, ForceInt);
                }
                else if (Wanted == typeof(bool))
                {
                    ConditionalBox(Top);
                    Generator.Emit(OpCodes.Call, ForceBool);
                }
                else if (Wanted == typeof(object))
                {
                    ConditionalBox(Top);
                }
                else if (Wanted == typeof(object[]) && Top.IsArray)
                {
                }
                else
                {
                    Debug("WARNING: Can not force " + Wanted);
                }
            }
            Depth--;
        }
    }
}
