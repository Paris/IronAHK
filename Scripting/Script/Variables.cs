using System.Collections.Generic;

namespace IronAHK.Scripting
{
    partial class Script
    {
        public class Variables
        {
            Dictionary<string, object> table = new Dictionary<string, object>();
            Stack<string> collect = new Stack<string>();

            public bool AutoMark { get; set; }

            public object this[string key]
            {
                get
                {
                    if (!table.ContainsKey(key))
                        return null;

                    if (AutoMark && !collect.Contains(key))
                        collect.Push(key);

                    return table[key];
                }
                set
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
                }
            }

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
        }
    }
}
