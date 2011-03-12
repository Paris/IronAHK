using System;
using System.Collections.Generic;
using System.Reflection;

namespace IronAHK.Scripting
{
    partial class Script
    {
        public class Variables
        {
            Dictionary<string, object> table = new Dictionary<string, object>();
            Stack<string> collect = new Stack<string>();

            public bool AutoMark { get; set; }
            
            public object SetVariable(string key, object value)
            {
                if (SetReservedVariable(key, value))
                    return value;

                lock (table)
                {
                    var exists = table.ContainsKey(key);

                    if (value == null)
                    {
                        if (exists)
                            table.Remove(key);
                    }
                    else
                    {
                        if (exists)
                            table[key] = value;
                        else
                            table.Add(key, value);
                    }
                
                    return value;
                }
            }
            
            public object GetVariable(string key)
            {
                lock (table)
                {
                    if (!table.ContainsKey(key))
                        return GetReservedVariable(key);

                    if (AutoMark && !collect.Contains(key))
                        collect.Push(key);

                    return table[key];
                }
            }
            
            public object this[string key]
            {
                get { return GetVariable(key); }
                set { SetVariable(key, value); }
            }

            #region Collection

            public void Mark(params string[] keys)
            {
                foreach (var key in keys)
                    if (!collect.Contains(key))
                        collect.Push(key);
            }

            public void Sweep()
            {
                while (collect.Count != 0)
                    this[collect.Pop()] = null;
            }

            #endregion

            #region Reserved

            static bool SetReservedVariable(string name, object value)
            {
                var prop = FindReservedVariable(name);
                var set = prop != null && prop.CanWrite;

                if (set)
                {
                    value = ForceType(prop.PropertyType, value);
                    prop.SetValue(null, value, null);
                }

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
                // UNDONE: This check fails on ErrorLevel
                //if(!name.Substring(0, A_.Length).Equals(A_, StringComparison.OrdinalIgnoreCase))
                //    return null;

                PropertyInfo prop = null;

                foreach (var item in typeof(Script).BaseType.GetProperties())
                    if (item.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                        prop = item;

                return prop;
            }
            #endregion
        }
    }
}
