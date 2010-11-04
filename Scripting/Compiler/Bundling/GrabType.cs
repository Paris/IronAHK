using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace IronAHK.Scripting
{
    partial class ILMirror
    {
        public Type GrabType(Type Copy)
        {
            return GrabType(Copy, null);
        }
        
        protected Type GrabType(Type Copy, TypeBuilder On)
        {
            if(Copy == null) return null;
            
            if(TypesDone.ContainsKey(Copy))
                return TypesDone[Copy];
            
            if(!Sources.Contains(Copy.Module)) 
                return TypeReplaceGenerics(Copy);
            
            if(Copy.IsByRef)
                return GrabType(Copy.GetElementType()).MakeByRefType();
            
            if(Copy.IsArray)
                return GrabType(Copy.GetElementType()).MakeArrayType();
            
            if(On == null)
                On = GrabType(Copy.DeclaringType) as TypeBuilder;
            
            var OurInterfaces = new List<Type>();
            Type[] Interfaces;
            
            if(!Copy.IsEnum)
            {
                // To not specify an interface implementation if one of the parent types
                // already implements one of the interfaces returned by GetInterfaces.
                foreach(Type T in Copy.GetInterfaces())
                {
                    if(!T.IsAssignableFrom(Copy.BaseType))
                        OurInterfaces.Add(T);
                }
                Interfaces = OurInterfaces.ToArray();
            }
            else Interfaces = null;
            
            TypeBuilder Ret;
            if(On == null) Ret = Module.DefineType(Copy.Name, Copy.Attributes, GrabType(Copy.BaseType), Interfaces);
            else Ret = On.DefineNestedType(Copy.Name, Copy.Attributes, GrabType(Copy.BaseType), Interfaces);
            
            TypesDone.Add(Copy, Ret);
            
            // We need to copy over the static constructor explicitly, because it is never called in the IL
            ConstructorInfo StaticConstr = FindStaticConstructor(Copy);
            if(StaticConstr != null)
                GrabConstructor(StaticConstr, Ret);
            
            // Enum fields need to be copied over on .NET to avoid a TypeLoadException
            // Interestingly, enum types without fields are perfectly fine with mono.
            if(Copy.IsEnum)
            {
                GrabField(Copy.GetField("value__"), Ret);
                
                foreach(FieldInfo Field in Copy.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    if(Field.DeclaringType != Copy) continue;
                    GrabField(Field);
                }
            }
            
            // - If we are copying over a delegate, we need to guarantee that all members are copied over,
            //   if not we'll cause a runtime error somewhere along the pipeline (for example: mono fails
            //   on an assertion).
            // - If we are copying over a class with an abstract parent, we need to copy over all methods
            //   to prevent a TypeLoadException at runtime (non-abstract types containing methods without
            //   a body cause this)
            // Delegates have a native-code constructor that is needed, too
            if(Copy.BaseType == typeof(MulticastDelegate))
            {
                ConstructorInfo NativeCtor = Copy.GetConstructors()[0];
                GrabConstructor(NativeCtor, Ret);
                
                GrabMethod(Copy.GetMethod("Invoke"), Ret);
                GrabMethod(Copy.GetMethod("BeginInvoke"), Ret);
                GrabMethod(Copy.GetMethod("EndInvoke"), Ret);
            }
            else if(Copy.IsExplicitLayout || Copy.BaseType.IsAbstract || OurInterfaces.Count > 0)
            {
                foreach(MethodInfo Method in Copy.GetMethods())
                {
                    if(Method.DeclaringType != Copy) continue;
                    GrabMethod(Method, Ret);
                }
                
                foreach(MethodInfo Method in Copy.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if(Method.DeclaringType != Copy) continue;
                    GrabMethod(Method, Ret);
                }
                
                foreach(FieldInfo Field in Copy.GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
                {
                    if(Field.DeclaringType != Copy) continue;
                    GrabField(Field, Ret);
                }
            }
            
            if(Copy.IsEnum || Copy.BaseType == typeof(MulticastDelegate))
                Ret.CreateType();
            
            return Ret;            
        }
        
        Type TypeReplaceGenerics(Type Orig)
        {
            if(Orig.IsGenericType)
            {
                Type[] Replace = Orig.GetGenericArguments();
                if(!ReplaceGenericArguments(Replace))
                    return Orig;
                
                return Orig.GetGenericTypeDefinition().MakeGenericType(Replace);
            }
            else return Orig;
        }
        
        bool ReplaceGenericArguments(Type[] Replace)
        {
            bool Replaced = false;
            for(int i = 0; i < Replace.Length; i++)
            {
                Type New = GrabType(Replace[i]);
                if(New != Replace[i])
                {
                    Replace[i] = New;
                    Replaced = true;
                }
            }
            
            return Replaced;
        }
        
        Type[] ParameterTypes(MethodBase Original)
        {
            ParameterInfo[] Params = Original.GetParameters();
            Type[] Ret = new Type[Params.Length];
            
            for(int i = 0; i < Params.Length; i++)
            {
                if(Sources.Contains(Params[i].ParameterType.Module))
                    Ret[i] = GrabType(Params[i].ParameterType);
                else Ret[i] = Params[i].ParameterType;
            }
            
            return Ret;
        }
    }
}

