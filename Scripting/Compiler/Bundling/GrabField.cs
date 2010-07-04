using System;
using System.Reflection;
using System.Reflection.Emit;

namespace IronAHK.Scripting
{
    partial class ILMirror
    {
        public FieldInfo GrabField(FieldInfo Field)
        {
            return GrabField(Field, Target);
        }
        
        FieldInfo GrabField(FieldInfo Field, TypeBuilder On)
        {
            return GrabField(Field, On, false);
        }
        
        FieldInfo GrabField(FieldInfo Field, TypeBuilder On, bool Force)
        {
            if(FieldsDone.ContainsKey(Field))
               return FieldsDone[Field];
            
            if(!Force && !Sources.Contains(Field.DeclaringType))
                return Field;
            
            FieldBuilder CopiedField = On.DefineField(Prefix+Field.Name, GrabType(Field.FieldType), Field.Attributes); 
            FieldsDone.Add(Field, CopiedField);
            
            return CopiedField;
        }
    }
}

