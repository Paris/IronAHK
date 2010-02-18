using System;
using System.Collections;
using System.Collections.Generic;

namespace IronAHK.Scripting
{
    partial class Script
    {
        #region Index

        public static object Index(object item, object key)
        {
            if (item == null || key == null)
                return null;

            bool isDictionary = typeof(IDictionary).IsAssignableFrom(item.GetType());

            // don't allow numeric indexing for objects which have only string keys
            if (IsNumeric(key) && !isDictionary)
                return IndexAt(item, ForceInt(key));

            if (!(key is string) || !isDictionary)
                return null;

            var table = (IDictionary)item;
            string lookup = ((string)key).ToLowerInvariant();

            if (table.Contains(lookup))
                return table[lookup];

            return null;
        }

        public static object IndexAt(object item, int position)
        {
            if (position < 0 || item == null)
                return null;

            var type = item.GetType();

            if (item is object[])
            {
                object[] array = (object[])item;
                
                if (position > array.Length - 1)
                    return null;
                
                return array[position];
            }
            else if (type.IsArray)
            {
                var array = (Array)item;

                if (position > array.Length - 1)
                    return null;

                return array.GetValue(position);
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                var enumerator = ((IEnumerable)item).GetEnumerator();
                enumerator.MoveNext();
                int i = 0;

                while (enumerator.MoveNext())
                {
                    if (i == position)
                        return enumerator.Current;
                    i++;
                }

                return null;
            }

            return null;
        }

        #endregion

        #region Create

        public static Dictionary<string, object> Dictionary(string[] keys, object[] values)
        {
            var table = new Dictionary<string, object>();
            values = (object[])values[0];

            for (int i = 0; i < keys.Length; i++)
            {
                string name = keys[i].ToLowerInvariant();
                object entry = i < values.Length ? values[i] : null;

                if (entry == null)
                {
                    if (table.ContainsKey(name))
                        table.Remove(name);
                }
                else
                {
                    if (table.ContainsKey(name))
                        table[name] = entry;
                    else
                        table.Add(name, entry);
                }
            }

            return table;
        }

        #endregion
    }
}
