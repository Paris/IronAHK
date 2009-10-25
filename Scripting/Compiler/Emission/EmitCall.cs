using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection.Emit;
using System.Reflection;

namespace IronAHK.Scripting
{
    internal partial class MethodWriter
    {
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
            Type target, rusty = typeof(Rusty.Core);
            if (Type.Type.BaseType.Equals(rusty.FullName, StringComparison.OrdinalIgnoreCase))
                target = rusty;
            else
            {
                try
                {
                    target = System.Type.GetType(Type.Type.BaseType, true, false);
                }
                catch (Exception)
                {
                    throw new CompileException(Type, "Could not access type " + Type.Type.BaseType);
                }
            }
            try
            {
                Type[] types = null;
                if (target.FullName == typeof(string).FullName && Invoke.Method.MethodName == "Concat")
                    types = new Type[] { typeof(string[]) };
                MethodInfo method = types == null ? target.GetMethod(Invoke.Method.MethodName) : target.GetMethod(Invoke.Method.MethodName, types);
                if (method == null)
                    throw new ArgumentNullException();
                Generator.Emit(OpCodes.Call, method);
            }
            catch (Exception)
            {
                throw new CompileException(Invoke, string.Format("Could not find method {0} in type {1}", Invoke.Method.MethodName, Type.Type.BaseType));
            }
            Depth--;
        }
    }
}
