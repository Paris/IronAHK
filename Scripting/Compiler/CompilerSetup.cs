// Setup.cs created by Tobias Kappé at 14:59 21-12-2008
// Unless stated otherwise, this is licenced under the new BSD license

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using IronAHK.Rusty;

namespace IronAHK.Scripting
{
    internal partial class Compiler : ICodeCompiler
    {
        AssemblyName AName;
        AssemblyBuilder ABuilder;
        
        Assembly LinkingTo;
        MethodBuilder EntryPoint;
        MethodCollection Methods;

        public Compiler()
        {
            Methods = new MethodCollection();
        }
        
        public void LinkTo(string File)
        {
            LinkingTo = Assembly.LoadFile(File);
                
            MineTypes(LinkingTo);
        }
        
        void Setup(CompilerParameters Options)
        { 
            string name = Path.GetFileName(Options.OutputAssembly);
            if (name.Length == 0)
                throw new ArgumentNullException();
          
            AName = new AssemblyName(name);
            ABuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(AName, AssemblyBuilderAccess.Save);

#if DEBUG
            // from http://blogs.msdn.com/rmbyers/archive/2005/06/26/432922.aspx
            Type daType = typeof(DebuggableAttribute);
            ConstructorInfo daCtor = daType.GetConstructor(new Type[] { typeof(DebuggableAttribute.DebuggingModes) });
            CustomAttributeBuilder daBuilder = new CustomAttributeBuilder(daCtor, new object[] {
                DebuggableAttribute.DebuggingModes.DisableOptimizations |
                DebuggableAttribute.DebuggingModes.Default });
            ABuilder.SetCustomAttribute(daBuilder);
            // HACK: this doesn't work, I still get warning messages in VS08
#endif

            foreach (string assembly in Options.ReferencedAssemblies)
            {
                ABuilder.DefineDynamicModule(Path.GetFileName(assembly));
                LinkTo(assembly);
            }
        }

        void MineTypes(Assembly Asm)
        {
            foreach(Type T in Asm.GetTypes())
            {
                MineMethods(T);
            }
        }

        void MineMethods(Type Typ)
        {
            foreach(MethodInfo Method in Typ.GetMethods())
            {
                // We skip private methods for privacy, abstract/nonstatic/constructor/generic methods for convenience
                // Also, properties because exposing those is plain silly
                if(Method.IsPrivate || Method.IsAbstract || Method.IsConstructor || !Method.IsStatic || Method.IsGenericMethod ||
                   Method.Name.StartsWith("get_") || Method.Name.StartsWith("set_"))
                    continue;
        
                Methods.Add(Method);
            }
        }

        public void Save()
        {
            ABuilder.Save(AName.Name); // TODO: needs to save in requested directory
        }
    }
}
