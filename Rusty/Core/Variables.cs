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
            if (SetReservedVariable(name, value))
                return value;

            InitVariables();

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
            InitVariables();

            lock (variables)
            {
                if (variables != null && variables.ContainsKey(name))
                    return variables[name];
            }

            return GetReservedVariable(name);
        }

        static bool SetReservedVariable(string name, object value)
        {
            var prop = FindReservedVariable(name);
            var set = prop != null && prop.CanWrite;

            if (set)
                prop.SetValue(null, value, null);

            return set;
        }

        static object GetReservedVariable(string name)
        {
            var prop = FindReservedVariable(name);
            return prop == null || !prop.CanRead ? null : prop.GetValue(null, null);
        }

        static PropertyInfo FindReservedVariable(string name)
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

            PropertyInfo prop = null;

            foreach (var item in typeof(Core).GetProperties())
                if (item.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    prop = item;

            return prop;
        }
    }
}
