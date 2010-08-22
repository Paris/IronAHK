using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace IronAHK.Scripting
{
    partial class ILMirror
    {
        public MethodInfo GrabMethod(MethodInfo Original)
        {
            return GrabMethod(Original, null);
        }
        
        protected MethodInfo GrabMethod(MethodInfo Original, TypeBuilder On)
        {
            if(Original == null) return null;
            
            if(MethodsDone.ContainsKey(Original)) 
                return MethodsDone[Original];
            
            if(!Sources.Contains(Original.Module))
                return MethodReplaceGenerics(Original);
            
            if(On == null)
                On = GrabType(Original.DeclaringType) as TypeBuilder;
            
            if((Original.Attributes & MethodAttributes.PinvokeImpl) == MethodAttributes.PinvokeImpl)
                return GrabPInvokeImpl(Original, On);
            
            Type ReturnType = GrabType(Original.ReturnType);
            Type[] Parameters = ParameterTypes(Original);
            
            if(MethodsDone.ContainsKey(Original))
                return MethodsDone[Original];
            
            MethodBuilder Builder = On.DefineMethod(Original.Name, Original.Attributes, ReturnType, Parameters);
            
            MethodsDone.Add(Original, Builder);
            
            // Explicit interface implementations require specifying which method is being overridden.
            if(Original.IsFinal)
            {
                if(Original.Name.Contains("."))
                {
                    MethodInfo Overriding = FindBaseMethod(Original, Original.DeclaringType);
                    
                    if(Overriding != null)
                        On.DefineMethodOverride(Builder, Overriding);
                }
                else
                {
                    // Crawl among the interfaces attempting to guess the method that is being overridden.
                    foreach(Type T in Original.DeclaringType.GetInterfaces())
                    {
                        foreach(MethodInfo M in T.GetMethods())
                        {
                            if(M.IsAbstract && M.Name == Original.Name)
                            {
                                On.DefineMethodOverride(Builder, M);
                                
                                goto foundoverride; // For lack of "break 2" statement.
                            }
                        }
                    }
                    
                }
            }
            
        foundoverride:
            
            Builder.SetImplementationFlags(Original.GetMethodImplementationFlags());
            
            CopyMethodBody(Original, Builder);
            
            return Builder; 
        }
        
        public MethodInfo GrabPInvokeImpl(MethodInfo Original)
        {
            return GrabPInvokeImpl(Original, null);
        }
        
        protected MethodInfo GrabPInvokeImpl(MethodInfo Original, TypeBuilder On)
        {
            if(On == null)
                On = GrabType(Original.DeclaringType) as TypeBuilder;
            
            DllImportAttribute Attr = FindCustomAttribute<DllImportAttribute>(Original);
            if(Attr == null)
                throw new InvalidOperationException("P/Invoke method without a DllImportAttribute");
            
            Type ReturnType = GrabType(Original.ReturnType);
            Type[] Parameters = ParameterTypes(Original);
            
            if(MethodsDone.ContainsKey(Original))
                return MethodsDone[Original];
            
            MethodBuilder PInvoke = On.DefinePInvokeMethod(Original.Name, Attr.Value, Original.Attributes, Original.CallingConvention, 
                ReturnType, Parameters, Attr.CallingConvention, Attr.CharSet);
            
            PInvoke.SetImplementationFlags(Original.GetMethodImplementationFlags());
            
            MethodsDone.Add(Original, PInvoke);
            
            return PInvoke;
        }
        
        MethodInfo MethodReplaceGenerics(MethodInfo Original)
        {
            if(Original.DeclaringType.IsGenericType)
            {
                Type NewDeclaring = GrabType(Original.DeclaringType);
                
                if(NewDeclaring != Original.DeclaringType)
                {
                    MethodInfo Ret = TypeBuilder.GetMethod(NewDeclaring, FindMatchingGenericMethod(Original) as MethodInfo);
                    return MethodReplaceGenerics(Ret);
                }
            }
            
            if(Original.IsGenericMethod)
            {
                Type[] Replace = Original.GetGenericArguments();
                
                if(!ReplaceGenericArguments(Replace))
                    return Original;
                
                return Original.GetGenericMethodDefinition().MakeGenericMethod(Replace);
            }
            
            return Original;
        }
        
        MethodInfo FindBaseMethod(MethodInfo Original, Type Source)
        {
            foreach(Type T in Source.GetInterfaces())
            {
                InterfaceMapping Mapping = Source.GetInterfaceMap(T);
                
                for(int i = 0; i < Mapping.TargetMethods.Length; i++)
                {
                    if(Mapping.TargetMethods[i] == Original)
                        return Mapping.InterfaceMethods[i];
                }
            }
                    
            return null;
        }
        
        MethodBase FindMatchingGenericMethod(MethodBase Orig)
        {
            Type Generic = Orig.DeclaringType.GetGenericTypeDefinition();
            ParameterInfo[] OrigParams = Orig.GetParameters();
            
            if(Orig.IsConstructor)
            {
                foreach(ConstructorInfo Info in Generic.GetConstructors())
                {
                    if(GenericMethodIsEquivalent(Orig, Info, OrigParams))
                        return Info;
                }
            }
            else
            {
                foreach(MethodInfo Info in Generic.GetMethods())
                {
                    if(GenericMethodIsEquivalent(Orig, Info, OrigParams))
                        return Info;
                }
            }
            
            throw new Exception("Could not find matching method");
        }
        
        static bool GenericMethodIsEquivalent(MethodBase Orig, MethodBase GenericCousin, ParameterInfo[] OrigParams)
        {
            if(Orig.Name != GenericCousin.Name) return false;
            
            if(Orig.Attributes != GenericCousin.Attributes) return false;
            
            ParameterInfo[] Params = Orig.GetParameters();
            if(Params.Length != OrigParams.Length) return false;
            
            int i;
            for(i = 0; i < Params.Length; i++)
            {
                // TODO: Check if original parameter type matches original generic argument
                if(Params[i].ParameterType != OrigParams[i].ParameterType &&
                   !Params[i].ParameterType.IsGenericParameter)
                    return false;
            }
            
            return true;
        }
    }
}
