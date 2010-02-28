using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;

namespace IronAHK.Scripting
{
    partial class Parser : ICodeParser
    {
        #region Properties

        const string mainScope = "";
        const string className = "Program";
        CodeEntryPointMethod main = new CodeEntryPointMethod();
        Dictionary<string, CodeMemberMethod> methods = new Dictionary<string, CodeMemberMethod>();
        CodeStatementCollection prepend = new CodeStatementCollection();

        string fileName;
        int line;

        #endregion

        #region Getters

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

                var container = new CodeTypeDeclaration(className);
                container.BaseTypes.Add(bcl);
                container.Attributes = MemberAttributes.Private;
                space.Types.Add(container);

                var run = new CodeExpressionStatement((CodeMethodInvokeExpression)InternalMethods.ApplicationRun);
                foreach (CodeStatement statement in prepend)
                    main.Statements.Insert(0, statement);
                if (prepend.Count > 0)
                    main.Statements.Insert(prepend.Count, run);
                prepend = new CodeStatementCollection();

                ResolveLocalInvokes();
                foreach (CodeMemberMethod method in methods.Values)
                    container.Members.Add(method);

                return unit;
            }
        }

        #endregion

        #region Methods

        public Parser()
        {
            main.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(STAThreadAttribute))));
            methods.Add(mainScope, main);
        }

        public CodeCompileUnit Parse(TextReader codeStream)
        {
            return Parse(codeStream, string.Empty);
        }

        public CodeCompileUnit Parse(TextReader codeStream, string name)
        {
            var lines = Read(codeStream, name);
            Statements(lines);
            return CompileUnit;
        }

        #endregion
    }
}
