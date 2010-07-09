using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace IronAHK.Scripting
{
    partial class ILMirror
    {
        public MethodInfo GrabMethod(MethodInfo Original)
        {
            if(Original == null) return null;
            
            if(MethodsDone.ContainsKey(Original)) 
                return MethodsDone[Original];
            
            if(!Sources.Contains(Original.Module))
                return MethodReplaceGenerics(Original);
            
            if((Original.Attributes & MethodAttributes.PinvokeImpl) == MethodAttributes.PinvokeImpl)
                return GrabPInvokeImpl(Original);
            
            TypeBuilder On = GrabType(Original.DeclaringType) as TypeBuilder;
            
            Type ReturnType;
            if(Sources.Contains(Original.ReturnType.Module))
                ReturnType = GrabType(Original.ReturnType);
            else ReturnType = Original.ReturnType;
            
            MethodBuilder Builder = On.DefineMethod(Original.Name, Original.Attributes, 
                ReturnType, ParameterTypes(Original));
            
            MethodsDone.Add(Original, Builder);
            
            Builder.SetImplementationFlags(Original.GetMethodImplementationFlags());
            
            CopyMethodBody(Original, Builder.GetILGenerator());
            
            return Builder; 
        }
        
        MethodInfo GrabPInvokeImpl(MethodInfo Original)
        {
            TypeBuilder On = GrabType(Original.DeclaringType) as TypeBuilder;
            
            DllImportAttribute Attr = FindDllImportAttribute(Original);
            if(Attr == null)
                throw new InvalidOperationException("P/Invoke method without a DllImportAttribute");
            
            MethodBuilder PInvoke = On.DefinePInvokeMethod(Original.Name, Attr.Value, Original.Attributes, Original.CallingConvention, 
                Original.ReturnType, ParameterTypes(Original), Attr.CallingConvention, Attr.CharSet);
            
            MethodsDone.Add(Original, PInvoke);
            
            return PInvoke;
        }
        
        DllImportAttribute FindDllImportAttribute(MethodInfo Original)
        {
            foreach(object Attr in Original.GetCustomAttributes(false))
            {
                if(Attr is DllImportAttribute)
                    return Attr as DllImportAttribute;
            }
            
            return null;
        }
        
        MethodInfo MethodReplaceGenerics(MethodInfo Original)
        {
            if(Original.IsGenericMethod)
            {
                Type[] Replace = ReplaceGenericArguments(Original.GetGenericArguments());
                return Original.GetGenericMethodDefinition().MakeGenericMethod(Replace);
            }
            else return Original;
        }
    }
}

