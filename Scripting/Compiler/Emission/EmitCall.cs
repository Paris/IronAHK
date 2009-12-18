using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection.Emit;
using System.Reflection;

namespace IronAHK.Scripting
{
    internal partial class MethodWriter
    {
        Type EmitMethodInvoke(CodeMethodInvokeExpression invoke)
        {
            MethodInfo target = null;
            Type[] types = null;
            bool staticCall = !(invoke.Method.TargetObject is CodeExpression);

            Depth++;
            Debug("Emitting method invoke " + invoke.Method.MethodName);

            if (invoke.Method.TargetObject == null && Methods.ContainsKey(invoke.Method.MethodName))
            {
                target = Methods[invoke.Method.MethodName].Method;
                types = ParameterTypes[invoke.Method.MethodName];
            }
            else if(staticCall)
            {
                var args = new ArgType[invoke.Parameters.Count];

                for (int i = 0; i < args.Length; i++)
                    args[i] = ArgType.Expression;

                target = Lookup.BestMatch(invoke.Method.MethodName, args);
            }
            else
                target = GetMethodInfo(invoke.Method);

            if (types == null && target != null)
            {
                var param = target.GetParameters();
                types = new Type[param.Length];

                for (int i = 0; i < types.Length; i++)
                    types[i] = param[i].ParameterType;
            }

            Depth++;
            for (int i = 0; i < invoke.Parameters.Count; i++)
            {
                Debug("Emitting parameter " + i);
                var generated = EmitExpression(invoke.Parameters[i], true);

                if (!staticCall)
                    ForceTopStack(generated, types[i]);
            }
            Depth--;

            Generator.Emit(OpCodes.Call, target);
            Depth--;
            return target.ReturnType;
        }

        MethodInfo GetMethodInfo(CodeMethodReferenceExpression reference)
        {
            const string id = "MethodInfo";
            if (reference.UserData.Contains(id) && reference.UserData[id] is MethodInfo)
                return (MethodInfo)reference.UserData[id];

            MethodInfo method = null;

            Type target;
            if(reference.TargetObject is CodeTypeReferenceExpression)
                target = Type.GetType(((CodeTypeReferenceExpression)reference.TargetObject).Type.BaseType);
            else if (reference.TargetObject is CodeExpression)
            {
                target = EmitExpression(reference.TargetObject as CodeExpression);
            }
            else throw new CompileException(reference.TargetObject, "Non-static method referencing neither type nor expression");

            Console.WriteLine(target);

            Type[] parameters = new Type[reference.TypeArguments.Count];

            for (int i = 0; i < reference.TypeArguments.Count; i++)
                parameters[i] = Type.GetType(reference.TypeArguments[i].BaseType);

            while (target != null)
            {
                method = parameters.Length == 0 ? target.GetMethod(reference.MethodName, Type.EmptyTypes) :
                    target.GetMethod(reference.MethodName, parameters);

                if (method != null)
                    return method;

                if (target == typeof(object))
                    break;

                target = target.BaseType;
            }

            throw new CompileException(reference, "Could not find method " + reference.MethodName);
        }
    }
}
