using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;

namespace IronAHK.Scripting
{
    partial class Parser : ICodeParser
    {
        const string mainScope = "";
        CodeEntryPointMethod main = new CodeEntryPointMethod();
        Dictionary<string, CodeMemberMethod> methods = new Dictionary<string, CodeMemberMethod>();
        CodeStatementCollection prepend = new CodeStatementCollection();

        /// <summary>
        /// Return a DOM representation of a script.
        /// </summary>
        public CodeCompileUnit CompileUnit
        {
            get
            {
                var unit = new CodeCompileUnit();
                var bcl = typeof(Script);

                var space = new CodeNamespace(bcl.Namespace + ".Instance");
                unit.Namespaces.Add(space);

                var container = new CodeTypeDeclaration("Program");
                container.BaseTypes.Add(bcl);
                container.Attributes = MemberAttributes.Private;
                space.Types.Add(container);

                foreach (CodeStatement statement in prepend)
                    main.Statements.Insert(0, statement);
                prepend = new CodeStatementCollection();

                foreach (CodeMemberMethod method in methods.Values)
                    container.Members.Add(method);

                return unit;
            }
        }

        public Parser()
        {
            main.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(STAThreadAttribute))));
            methods.Add(mainScope, main);
        }

        public CodeCompileUnit Parse(TextReader codeStream)
        {
            var lines = Read(codeStream, null);
            Statements(lines);
            return CompileUnit;
        }
    }
}
