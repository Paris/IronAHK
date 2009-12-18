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
        CodeEntryPointMethod main;

        /// <summary>
        /// Return a DOM representation of a script.
        /// </summary>
        public CodeCompileUnit CompileUnit
        {
            get
            {
                CodeCompileUnit unit = new CodeCompileUnit();

                CodeNamespace space = new CodeNamespace(core.Namespace + ".Instance");
                unit.Namespaces.Add(space);

                var container = new CodeTypeDeclaration("Program");
                container.BaseTypes.Add(typeof(Script));
                container.Attributes = MemberAttributes.Private;
                space.Types.Add(container);

                foreach (CodeMemberMethod method in methods.Values)
                    container.Members.Add(method);

                return unit;
            }
        }

        public Parser()
        {
            main = new CodeEntryPointMethod();
            main.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(STAThreadAttribute))));
            methods = new Dictionary<string, CodeMemberMethod>();
            methods.Add(mainScope, main);

            core = typeof(Script);
            internalID = 0;
        }

        public CodeCompileUnit Parse(TextReader codeStream)
        {
            var lines = Read(codeStream, null);
            Compile(lines);
            return CompileUnit;
        }
    }
}
