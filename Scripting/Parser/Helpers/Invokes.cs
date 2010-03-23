using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        List<CodeMethodInvokeExpression> invokes = new List<CodeMethodInvokeExpression>();
        const string invokeCommand = "IsCommand";

        #region DOM

        CodeMemberMethod LocalMethod(string name)
        {
            var method = new CodeMemberMethod() { Name = name, ReturnType = new CodeTypeReference(typeof(object)) };
            var param = new CodeParameterDeclarationExpression(typeof(object[]), args);
            param.UserData.Add("rawtype", typeof(object[]));
            method.Parameters.Add(param);
            return method;
        }

        CodeMethodInvokeExpression LocalLabelInvoke(string name)
        {
            var invoke = (CodeMethodInvokeExpression)InternalMethods.LabelCall;
            invoke.Parameters.Add(new CodePrimitiveExpression(name));
            return invoke;
        }

        CodeMethodInvokeExpression LocalMethodInvoke(string name)
        {
            var invoke = new CodeMethodInvokeExpression();
            invoke.Method.MethodName = name;
            invoke.Method.TargetObject = null;
            return invoke;
        }

        #endregion

        #region Resolve

        void StdLib()
        {
            #region Paths

            var search = new StringBuilder();

            search.Append(Environment.GetEnvironmentVariable(LibEnv));
            search.Append(Path.PathSeparator);
            search.Append(Path.Combine(Assembly.GetExecutingAssembly().Location, LibDir));

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                search.Append(Path.PathSeparator);
                search.Append(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Path.Combine("AutoHotkey", LibDir)));
            }
            else if (Path.DirectorySeparatorChar == '/' && Environment.OSVersion.Platform == PlatformID.Unix)
            {
                search.Append(Path.PathSeparator);
                search.Append("/usr/" + LibDir + "/" + LibExt);
                search.Append(Path.PathSeparator);
                search.Append("/usr/local/" + LibDir + "/" + LibExt);
                search.Append(Path.PathSeparator);
                search.Append(Path.Combine(Environment.GetEnvironmentVariable("HOME"), Path.Combine(LibDir, LibExt)));
            }

            var paths = search.ToString().Split(new[] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries);
            search = null;

            #endregion

            foreach (var invoke in invokes)
            {
                string name = invoke.Method.MethodName;

                if (IsLocalMethodReference(name))
                {
                    var obj = new CodeArrayCreateExpression { Size = invoke.Parameters.Count, CreateType = new CodeTypeReference(typeof(object)) };
                    obj.Initializers.AddRange(invoke.Parameters);
                    invoke.Parameters.Clear();
                    invoke.Parameters.Add(obj);
                    continue;
                }

                if (invoke.Method.TargetObject != null)
                    continue;

                MethodInfo core;

                try { core = typeof(Rusty.Core).GetMethod(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase); }
                catch (AmbiguousMatchException) { continue; }

                if (core != null)
                {
                    FixDirectionalParameters(core, invoke);
                    continue;
                }

                int z = name.IndexOf(LibSeperator);

                if (z != -1)
                    name = name.Substring(0, z);

                foreach (string dir in paths)
                {
                    if (!Directory.Exists(dir))
                        continue;

                    string file = Path.Combine(dir, string.Concat(name, ".", LibExt));

                    if (File.Exists(file))
                    {
                        Parse(new StreamReader(file), Path.GetFileName(file));
                        return;
                    }
                }
            }

            invokes.Clear();
        }

        [Conditional("LEGACY")]
        void FixDirectionalParameters(MethodInfo core, CodeMethodInvokeExpression invoke)
        {
            if (!(invoke.UserData.Contains(invokeCommand) && invoke.UserData[invokeCommand] is bool && (bool)invoke.UserData[invokeCommand]))
                return;

            var param = core.GetParameters();
            int c = Math.Min(invoke.Parameters.Count, param.Length);

            for (int i = 0; i < c; i++)
            {
                if (!(param[i].IsOut || param[i].ParameterType.IsByRef))
                    continue;

                if (invoke.Parameters[i] is CodeExpression)
                {
                    invoke.Parameters[i] = VarId((CodeExpression)invoke.Parameters[i]);
                    continue;
                }

                if (!(invoke.Parameters[i] is CodePrimitiveExpression))
                    continue;

                object value = ((CodePrimitiveExpression)invoke.Parameters[i]).Value;

                if (value is string)
                    invoke.Parameters[i] = VarId((string)value);
                else
                    invoke.Parameters[i] = VarId((CodeExpression)value);
            }
        }

        bool IsLocalMethodReference(string name)
        {
            foreach (var method in methods)
                if (method.Key.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        #endregion
    }
}
