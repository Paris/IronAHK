using System;
using System.Reflection;
using System.Reflection.Emit;

namespace IronAHK.Scripting
{
    partial class ILMirror
    {
        delegate byte[] GetBytes<T>(T arg);
        
        public FieldInfo GrabField(FieldInfo Field)
        {
            if(Sources.Contains(Field.Module))
                return GrabField(Field, GrabType(Field.DeclaringType) as TypeBuilder);
            
            return GrabField(Field, Target);
        }
        
        FieldInfo GrabField(FieldInfo Field, TypeBuilder On)
        {
            if(FieldsDone.ContainsKey(Field))
               return FieldsDone[Field];
            
            if(!Sources.Contains(Field.Module))
                return Field;
            
            FieldBuilder CopiedField = On.DefineField(Field.Name, GrabType(Field.FieldType), Field.Attributes); 
            FieldsDone.Add(Field, CopiedField);
            
            if(Field.IsLiteral && Field.DeclaringType.IsEnum)
                CopiedField.SetConstant(Field.GetRawConstantValue());
            
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
                                                            
        static byte[] RawSerialize(Array Orig, Type ValType)
        {
            if(ValType == typeof(char))
                return RawSerialize<char>(Orig, sizeof(char), BitConverter.GetBytes);
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

