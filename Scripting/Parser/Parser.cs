using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;

namespace IronAHK.Scripting
{
    partial class Parser : ICodeParser
    {
        Dictionary<string, CodeMemberMethod> methods;
        Type core;
        const string mainScope = "";
        CodeMemberMethod main;

        /// <summary>
        /// Return a DOM representation of a script.
        /// </summary>
        public CodeCompileUnit CompileUnit
        {
            get
            {
                CodeCompileUnit unit = new CodeCompileUnit();

                string prefix = core.Namespace;

                CodeNamespace space = new CodeNamespace(prefix + ".Script");
                unit.Namespaces.Add(space);

                space.Imports.Add(new CodeNamespaceImport("System"));
                space.Imports.Add(new CodeNamespaceImport("System.Windows.Forms"));
                space.Imports.Add(new CodeNamespaceImport(prefix));

                var container = new CodeTypeDeclaration("Class");
                space.Types.Add(container);

                var start = new CodeEntryPointMethod();
                if (methods.ContainsKey(mainScope))
                    start.Statements.AddRange(methods[mainScope].Statements);
                container.Members.Add(start);

                foreach (CodeMemberMethod method in methods.Values)
                    if (method.GetType() != typeof(CodeEntryPointMethod))
                        container.Members.Add(method);

                return unit;
            }
        }

        public Parser()
        {
            methods = new Dictionary<string, CodeMemberMethod>();
            methods.Add(mainScope, new CodeEntryPointMethod());
            main = methods[mainScope];

            core = typeof(IronAHK.Rusty.Core);
        }

        public CodeCompileUnit Parse(TextReader codeStream)
        {
            var lines = Read(codeStream, null);
            Compile(lines);
            return CompileUnit;
        }
    }
}
