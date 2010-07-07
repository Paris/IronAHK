using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using IronAHK.Rusty;

namespace IronAHK.Scripting
{
    partial class Compiler : ICodeCompiler
    {
        AssemblyName AName;
        AssemblyBuilder ABuilder;
        
        Assembly LinkingTo;
        MethodBuilder EntryPoint;
        MethodCollection Methods;
        ILMirror Mirror;

        public Compiler()
        {
            Methods = new MethodCollection();
            Mirror = new ILMirror();
        }
        
        public void LinkTo(string file)
        {
            LinkingTo = File.Exists(file) ? Assembly.LoadFrom(file) : Assembly.Load(Path.GetFileNameWithoutExtension(file));
                
            MineTypes(LinkingTo);
        }
        
        void Setup(CompilerParameters Options)
        {
            if (string.IsNullOrEmpty(Options.OutputAssembly))
            {
                if (Options.GenerateInMemory)
                    Options.OutputAssembly = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".exe");
                else
                    throw new ArgumentNullException();
            }

            string name = Path.GetFileName(Options.OutputAssembly);
            string dir = Path.GetDirectoryName(Path.GetFullPath(Options.OutputAssembly));
            AName = new AssemblyName(name);
            ABuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(AName, AssemblyBuilderAccess.RunAndSave, dir);

            foreach (var type in new[] { typeof(Core), typeof(Script) })
                MineMethods(type);

            foreach (var assembly in Options.ReferencedAssemblies)
                LinkTo(assembly);
        }

        void MineTypes(Assembly Asm)
        {
            foreach(var T in Asm.GetTypes())
                MineMethods(T);
        }

        void MineMethods(Type Typ)
        {
            if (!Typ.IsPublic || Typ.IsAbstract)
                return;

            Debug("Adding type " + Typ.Name);
            Mirror.Sources.Add(Typ);

            foreach(var Method in Typ.GetMethods())
            {
                // We skip private methods for privacy, abstract/nonstatic/constructor/generic methods for convenience
                // Also, properties because exposing those is plain silly
                if(Method.IsPrivate || Method.IsAbstract || Method.IsConstructor || !Method.IsStatic || Method.IsGenericMethod ||
                   Method.Name.StartsWith("get_") || Method.Name.StartsWith("set_"))
                    continue;
        
                Methods.Add(Method);
            }
        }

        [Conditional("DEBUG")]
        void Debug(string message)
        {
            Console.WriteLine(message);
        }

        public void Save()
        {
            Mirror.Complete();
            ABuilder.Save(AName.Name);
        }
    }
}
