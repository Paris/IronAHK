using System.CodeDom;
using System.Reflection;
using System.Reflection.Emit;

namespace IronAHK.Scripting
{
    partial class Compiler
    {
        bool CopiedProperties = false;
        
        void EmitNamespace(AssemblyBuilder Parent, CodeNamespace Namespace)
        {
            ModuleBuilder Module = Parent.DefineDynamicModule(Namespace.Name, AName.Name);
            
            if(Mirror != null)
            {
                Mirror.Module = Module;
                
                if(!CopiedProperties)
                    CopyProperties();
            }
            
            foreach(CodeTypeDeclaration Type in Namespace.Types)
            {
                EmitType(Module, Type);
            }
            
            Module.CreateGlobalFunctions();
        }
        
        void CopyProperties()
        {
            // Normally, all used properties would get copied over. However, IA uses properties to specify reserved
            // variables. In the current implementation, these variables are attained by the Script class through 
            // reflection. Therefore, these properties will never be explicitly referenced in the IL, so we need
            // to copy them and register them as properties.
            foreach(PropertyInfo Property in typeof(Rusty.Core).GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                if(Property.Name.StartsWith("A_"))
                    Mirror.GrabProperty(Property);
            }
            
            CopiedProperties = true;
        }
    }
}
