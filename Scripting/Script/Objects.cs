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

        #region Modify

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

        public static object SetObject(object key, object item, object[] parents, object value)
        {
            bool isDictionary;

            for (int i = parents.Length - 1; i > -1; i--)
            {
                object child = Index(item, parents[i]);

                if (child == null)
                {
                    if (!(parents[i] is string))
                        return null;

                    isDictionary = typeof(IDictionary).IsAssignableFrom(item.GetType());

                    if (!isDictionary)
                        return null;

                    var dictionary = (IDictionary)item;
                    string name = (string)parents[i];
                    dictionary.Add(name, new Dictionary<string, object>());

                    item = dictionary[name];
                }
                else
                    item = child;
            }

            if (item == null)
                return null;

            var type = item.GetType();
            isDictionary = typeof(IDictionary).IsAssignableFrom(type);
            bool isNumericKey = IsNumeric(key);

            if (isNumericKey && isDictionary)
                return null;

            if (isDictionary) // set object
            {
                if (!(key is string))
                    return null;

                string name = ((string)key).ToLowerInvariant();

                var dictionary = (IDictionary)item;

                if (dictionary.Contains(name))
                    dictionary[name] = value;
                else
                    dictionary.Add(name, value);
            }
            else // set array
            {
                int index = ForceInt(key);

                if (!type.IsArray)
                    return null;

                var array = (Array)item;

                if (index < 0 || index > array.Length - 1)
                    return null;

                array.SetValue(value, index);
            }

            return value;
        }

        public static int ExtendArray(ref object item)
        {
            if (item == null || !item.GetType().IsArray)
                return -1;

            var array = (object[])item;
            int i = array.Length;
            Array.Resize<object>(ref array, i + 1);
            item = array;
            return i;
        }

        #endregion
    }
}
