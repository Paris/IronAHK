using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace IronAHK.Scripting
{
    partial class MethodWriter
    {
        Type EmitMethodInvoke(CodeMethodInvokeExpression invoke)
        {
            Depth++;
            Debug("Emitting method "+invoke.Method.MethodName);
                
            MethodInfo target = null;
            Type[] types = null;
            
            #region Lookup target function
            // First we check the local methods
            if(Methods.ContainsKey(invoke.Method.MethodName))
            {
                types = ParameterTypes[invoke.Method.MethodName];
                target = Methods[invoke.Method.MethodName].Method;
            }
            
            // Then the methods provided by rusty
            if(target == null)
                target = Lookup.BestMatch(invoke.Method.MethodName, invoke.Parameters.Count);
            
            // Lastly, the native methods
            if(target == null && invoke.Method.TargetObject != null)
                target = GetMethodInfo(invoke.Method);
            
            if(target == null)
                throw new CompileException(invoke, "Could not look up method "+invoke.Method.MethodName);
            
            bool hasParams = false;
            if(types == null) // Rusty-defined or native method
            {
                ParameterInfo[] Params = target.GetParameters();
                types = new Type[Params.Length];
                for(int i = 0; i < Params.Length; i++)
                    types[i] = Params[i].ParameterType;
                
                hasParams = types.Length == 0 ? false : types[types.Length-1] == typeof(object[]);
            }
            #endregion
            
            #region Emit parameters
            int p = 0;
            
            var ByRef = new Dictionary<LocalBuilder, CodeComplexVariableReferenceExpression>();
            
            for(p = 0; p < invoke.Parameters.Count; p++)
            {
                Depth++;
                Debug("Emitting parameter "+p);
                
                // Collapse superfluous arguments in array if last argument is object[]
                // _and_ it's not a call into a user-defined function.
                if(hasParams)
                {
                    if(p == types.Length-1)
                    {
                        Type Generated = EmitExpression(invoke.Parameters[p]);
                        
                        // If the last argument is object[], we don't bother about params
                        if(Generated == typeof(object[]))
                            break;
                        
                        LocalBuilder Temp = Generator.DeclareLocal(typeof(object));
                        ForceTopStack(Generated, typeof(object));
                        Generator.Emit(OpCodes.Stloc, Temp);
                        
                        EmitArrayCreation(typeof(object), invoke.Parameters.Count - p);
                        
                        Generator.Emit(OpCodes.Dup);
                        Generator.Emit(OpCodes.Ldc_I4, 0);
                        Generator.Emit(OpCodes.Ldloc, Temp);
                        Generator.Emit(OpCodes.Stelem_Ref);
                        
                        Depth--;
                        continue;
                    }
                    else if(p > types.Length-1)
                    {
                        EmitArrayInitializer(typeof(object), invoke.Parameters[p], p - types.Length + 1);
                        
                        Depth--;
                        continue;
                    }
                }
                
                if(p < types.Length)
                {
                    Type Generated = EmitExpression(invoke.Parameters[p]);

                    if(types[p].IsByRef)
                    {
                        // Variables passed by reference need to be stored in a local
                        Debug("Parameter "+p+" was by reference");
                        LocalBuilder Temporary = Generator.DeclareLocal(typeof(object));
                        Generator.Emit(OpCodes.Stloc, Temporary);
                        Generator.Emit(OpCodes.Ldloca, Temporary);

                        if(invoke.Parameters[p] is CodeComplexVariableReferenceExpression)
                            ByRef.Add(Temporary, invoke.Parameters[p] as CodeComplexVariableReferenceExpression);
                    }
                    else
                    {
                        ForceTopStack(Generated, types[p]);
                    }
                }
                
                Depth--;
            }
            
            // Anything not specified pads to the default IL value
            // Not to be confused with default values for parameters (ahk-style)
            while(p < types.Length)
            {
                if(types[p].IsByRef)
                {
                    Type NotRef = types[p].GetElementType();
                    EmitLiteral(NotRef, GetDefaultValueOfType(NotRef));
                    LocalBuilder Temporary = Generator.DeclareLocal(NotRef);
                    Generator.Emit(OpCodes.Stloc, Temporary);
                    Generator.Emit(OpCodes.Ldloca, Temporary);
                }
                else EmitLiteral(types[p], GetDefaultValueOfType(types[p])); 
                p++;
            }
            #endregion
            
            Generator.Emit(OpCodes.Call, target);
            
            // Save the variables passed to reference back in Rusty's variable handling
            foreach(LocalBuilder Builder in ByRef.Keys)
            {
                EmitComplexVariableReference(ByRef[Builder], false);
                Generator.Emit(OpCodes.Ldloc, Builder);
                EmitSaveVar();
                Generator.Emit(OpCodes.Pop);
            }
            
            Depth--;
            return target.ReturnType;
        }

        object GetDefaultValueOfType(Type t)
        {
            bool array = t.IsArray;
            var rank = t.IsArray ? t.GetArrayRank() : -1;

            if (t == typeof(string))
                return string.Empty;
            else if (t == typeof(object[]))
                return new object[] { };
            else if (t == typeof(bool))
                return false;
            else if (t == typeof(int))
                return default(int);
            else if (t == typeof(decimal))
                return default(decimal);
            else if (t == typeof(float))
                return default(float);
            else if (!t.IsInterface && !t.IsClass)
                return t.GetConstructor(new Type[] { }).Invoke(new object[] { });
            else
                return null;
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
