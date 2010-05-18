using System;
using System.Collections.Generic;
using System.Reflection;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// Assigns the specified value to a variable.
        /// </summary>
        /// <param name="name">The prefixed name of the variable in which to store <paramref name="value"/>.</param>
        /// <param name="value">The value to store.</param>
        /// <returns>The new contents of the variable.</returns>
        public static object SetEnv(string name, object value)
        {
            var method = GetReservedVariableReference(name, true);

            if (method != null)
            {
                try { method.Invoke(null, new[] { value }); }
                catch (ArgumentException)
                {
                    error = 1;
                    return null;
                }
                return GetReservedVariableReference(name, false).Invoke(null, new object[] { });
            }

            name = NormaliseVariableName(name);

            lock (variables)
            {
                bool exists = variables.ContainsKey(name);

                if (value == null)
                {
                    if (!exists)
                        variables.Remove(name);
                }
                else
                {
                    if (exists)
                        variables[name] = value;
                    else
                        variables.Add(name, value);
                }
            }

            return value;
        }

        /// <summary>
        /// Returns the contents of a variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <returns>The contents of the variable.</returns>
        public static object GetEnv(string name)
        {
            name = NormaliseVariableName(name);

            lock (variables)
            {
                if (variables != null && variables.ContainsKey(name))
                    return variables[name];
            }

            var method = GetReservedVariableReference(name, false);
            return method == null ? null : method.Invoke(null, new object[] { });
        }

        static string NormaliseVariableName(string name)
        {
            if (variables == null)
                variables = new Dictionary<string, object>();

            return name.ToLowerInvariant();
        }

        static MethodInfo GetReservedVariableReference(string name, bool set)
        {
            const string A_ = "A_";
            int z = name.LastIndexOf('.');

            if (z != -1)
            {
                z++;
                if (z + A_.Length > name.Length)
                    return null;
                name = name.Substring(z);
            }

            if (!name.Substring(0, A_.Length).Equals(A_, StringComparison.OrdinalIgnoreCase))
                return null;

            name = (set ? "set_" : "get_") + name;
            var list = typeof(Core).GetMethods();

            foreach (var method in list)
            {
                if (method.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && method.IsStatic)
                    return method;
            }

            return null;
        }
    }
}
