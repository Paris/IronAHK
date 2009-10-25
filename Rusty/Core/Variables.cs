using System;
using System.Collections.Generic;
using System.Reflection;

namespace IronAHK.Rusty
{
    partial class Core
    {
        // TODO: move these methods to a subclass for script instances to extend

        const string A_ = "A_";
        static Dictionary<string, object> variables = new Dictionary<string, object>();

        /// <summary>
        /// Assigns the specified value to a variable.
        /// </summary>
        /// <param name="Name">The prefixed name of the variable in which to store <paramref name="Value"/>.</param>
        /// <param name="Value">The string or number to store.</param>
        public static void SetEnv(string Name, object Value)
        {
            int z = Name.LastIndexOf('.') + 1;
            if (z > 0 && z + A_.Length < Name.Length && Name.Substring(z, A_.Length).Equals(A_, StringComparison.OrdinalIgnoreCase))
            {
                string name = Name.Substring(z);
                if (typeof(Core).GetMethod("get_" + name) != null)
                    return;
            }

            bool exists = variables.ContainsKey(Name);

            if (Value == null)
            {
                if (exists)
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

        /// <summary>
        /// Resolve a variable.
        /// </summary>
        /// <param name="Name">Name of variable.</param>
        /// <returns>Corresponding value.</returns>
        public static object GetEnv(string Name)
        {
            if (variables.ContainsKey(Name))
                return variables[Name];
            else
            {
                int z = Name.LastIndexOf('.') + 1;
                if (z == 0 || z + A_.Length > Name.Length)
                    return null;

                string name = Name.Substring(z);
                if (!name.Substring(0, A_.Length).Equals(A_, StringComparison.OrdinalIgnoreCase))
                    return null;

                string match = "get_" + name;

                try
                {
                    return typeof(Core).GetMethod(match).Invoke(null, null);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}
