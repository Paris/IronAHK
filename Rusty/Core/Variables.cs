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
        /// <param name="Name">The prefixed name of the variable in which to store <paramref name="Value"/>.</param>
        /// <param name="Value">The string or number to store.</param>
        /// <returns>The value set.</returns>
        public static object SetEnv(string Name, object Value)
        {
            var method = GetReservedVariableReference(Name, true);

            if (method != null)
            {
                try { method.Invoke(null, new object[] { Value }); }
                catch (ArgumentException)
                {
                    error = 1;
                    return null;
                }
                return GetReservedVariableReference(Name, false).Invoke(null, new object[] { });
            }

            Name = NormaliseVariableName(Name);

            lock (variables)
            {
                bool exists = variables.ContainsKey(Name);

                if (Value == null)
                {
                    if (!exists)
                        variables.Remove(Name);
                }
                else
                {
                    if (exists)
                        variables[Name] = Value;
                    else
                        variables.Add(Name, Value);
                }
            }

            return Value;
        }

        /// <summary>
        /// Resolve a variable.
        /// </summary>
        /// <param name="Name">Name of variable.</param>
        /// <returns>Corresponding value.</returns>
        public static object GetEnv(string Name)
        {
            Name = NormaliseVariableName(Name);

            lock (variables)
            {
                if (variables != null && variables.ContainsKey(Name))
                    return variables[Name];
            }

            var method = GetReservedVariableReference(Name, false);
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

            foreach (MethodInfo method in list)
            {
                if (method.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && method.IsStatic)
                    return method;
            }

            return null;
        }
    }
}
