using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace IronAHK.Scripting
{
    partial class ILMirror
    {
        delegate byte[] GetBytes<T>(T arg);
        
        public FieldInfo GrabField(FieldInfo Field)
        {
            return GrabField(Field, null);
        }
        
        protected FieldInfo GrabField(FieldInfo Field, TypeBuilder On)
        {
            if(Field == null) return null;
            
            if(FieldsDone.ContainsKey(Field))
               return FieldsDone[Field];
            
            if(!Sources.Contains(Field.Module))
                return Field;
            
            if(On == null)
                On = GrabType(Field.DeclaringType) as TypeBuilder;
            
            FieldBuilder CopiedField = On.DefineField(Field.Name, GrabType(Field.FieldType), Field.Attributes); 
            FieldsDone.Add(Field, CopiedField);
            
            if(Field.IsLiteral && Field.DeclaringType.IsEnum)
                CopiedField.SetConstant(Field.GetRawConstantValue());
            
            FieldOffsetAttribute Attr = FindCustomAttribute<FieldOffsetAttribute>(Field);
            if(Attr != null)
                CopiedField.SetOffset(Attr.Value);
            
            // Fields like these mostly have a backing field that comes with them in the
            // <PrivateImplementationDetails> class of the assembly. This backing field
            // refers to a bit of data in the .sdata section, which is serialized in a 
            // way not exposed by the .NET framework. The best thing we can do is to try
            // to replicate this format.
            if(Field.IsStatic && Field.IsInitOnly)
            {
                object Val = Field.GetValue(null);
                Type ValType = Val.GetType();
                
                if(ValType.IsArray)
                {
                    byte[] Raw = RawSerialize((Array) Val, ValType.GetElementType());
                    FieldBuilder Backing = ImplementationDetails.DefineInitializedData(Field.Name, Raw, Field.Attributes);
                    
                    // This is used later on when we recognize the initilization pattern (see TryReplaceBackingField)
                    BackingFields.Add(CopiedField, Backing);
                }
            }
            
            return CopiedField;
        }
        
        public PropertyInfo GrabProperty(PropertyInfo Orig)
        {
            return GrabProperty(Orig, null);
        }
        
        // Copying the get_ and set_ methods is not enough to register as a property.
        protected PropertyInfo GrabProperty(PropertyInfo Orig, TypeBuilder On)
        {
            if(Orig == null)
                return null;
            
            if(PropertiesDone.ContainsKey(Orig))
                return PropertiesDone[Orig];
            
            if(!Sources.Contains(Orig.Module))
                return Orig;
            
            if(On == null)
                On = GrabType(Orig.DeclaringType) as TypeBuilder;
            
            Type PropertyType = GrabType(Orig.PropertyType);
            
            if(PropertiesDone.ContainsKey(Orig))
                return PropertiesDone[Orig];
            
            PropertyBuilder Builder = On.DefineProperty(Orig.Name, Orig.Attributes, PropertyType, null);
            
            PropertiesDone.Add(Orig, Builder);
            
            MethodInfo SetMethod = Orig.GetSetMethod(), GetMethod = Orig.GetGetMethod();
            
            if(SetMethod != null)
                Builder.SetSetMethod(GrabMethod(SetMethod) as MethodBuilder);
            
            if(GetMethod != null)
                Builder.SetGetMethod(GrabMethod(GetMethod) as MethodBuilder);
            
            return Builder;
        }
        
        static byte[] RawSerialize(Array Orig, Type ValType)
        {
            if(ValType == typeof(char))
                return RawSerialize<char>(Orig, sizeof(char), BitConverter.GetBytes);
            else if(ValType == typeof(uint))
                return RawSerialize<uint>(Orig, sizeof(uint), BitConverter.GetBytes);
            else throw new InvalidOperationException("Can not serialize array of type "+ValType);
        }
        
        // Helper method to serialize to the raw byte format used for arrays
        static byte[] RawSerialize<T>(Array Orig, int size, GetBytes<T> Bytes) 
        {
            byte[] Ret = new byte[Orig.Length*size+size];
            
            for(int i = 0; i < Ret.Length-size; i += size)
            {
                byte[] raw = Bytes((T) Orig.GetValue(i/size));
                for(int j = 0; j < size; j++)
                    Ret[i+j] = raw[j];
            }
            
            return Ret;
        }
    }
}

