using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IronAHK.Rusty
{
    partial class Core
    {
        // TODO: organise Loops.cs

        static Stack<LoopInfo> loops = new Stack<LoopInfo>();

        enum LoopType
        {
            Normal,
            Registry,
            Directory,
            Parse,
            File,
            Each,
        }

        class LoopInfo
        {
            public int index = -1;
            public LoopType type = LoopType.Normal;
            public object result;
        }

        /// <summary>
        /// Perform a series of commands repeatedly: either the specified number of times or until break is encountered.
        /// </summary>
        /// <param name="n">How many times (iterations) to perform the loop.</param>
        /// <returns></returns>
        public static IEnumerable Loop(int n)
        {
            var info = new LoopInfo { type = LoopType.Normal };
            loops.Push(info);

            for (int i = 0; i < n;)
            {
                info.index = i;
                yield return ++i;
            }

            loops.Pop();
        }

        /// <summary>
        /// Retrieves substrings (fields) from a string, one at a time.
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <param name="delimiters">One of the following:
        /// <list>
        /// <item>the word <code>CSV</code> to parse in comma seperated value format;</item>
        /// <item>a sequence of characters to treat as delimiters;</item>
        /// <item>blank to parse each character of the string.</item>
        /// </list>
        /// </param>
        /// <param name="omit">An optional list of characters (case sensitive) to exclude from the beginning and end of each substring.</param>
        /// <returns></returns>
        public static IEnumerable LoopParse(string input, string delimiters, string omit)
        {
            var info = new LoopInfo { type = LoopType.Parse };
            loops.Push(info);

            if (delimiters.ToLowerInvariant() == Keyword_CSV)
            {
                var reader = new StringReader(input);
                var part = new StringBuilder();
                bool str = false, next = false;

                while (true)
                {
                    int current = reader.Read();
                    if (current == -1)
                        goto collect;

                    const char tokenStr = '"', tokenDelim = ',';

                    var sym = (char)current;

                    switch (sym)
                    {
                        case tokenStr:
                            if (str)
                            {
                                if ((char)reader.Peek() == tokenStr)
                                {
                                    part.Append(tokenStr);
                                    reader.Read();
                                }
                                else
                                    str = false;
                            }
                            else
                            {
                                if (next)
                                    part.Append(tokenStr);
                                else
                                    str = true;
                            }
                            break;

                        case tokenDelim:
                            if (str)
                                goto default;
                            goto collect; // sorry

                        default:
                            next = true;
                            part.Append(sym);
                            break;
                    }

                    continue;

                collect:
                    next = false;
                    string result = part.ToString();
                    part.Length = 0;
                    info.result = result;
                    info.index++;
                    yield return result;
                    if (current == -1)
                        break;
                }
            } else {
                string[] parts;
                var remove = omit.ToCharArray();

                if(string.IsNullOrEmpty(delimiters)) {
                    var chars = input.ToCharArray();
                    parts = new string[chars.Length];
                    for(int i=0; i < chars.Length; i++)
                        parts[i] = chars[i].ToString();
                }else
                    parts = input.Split(delimiters.ToCharArray(), StringSplitOptions.None);

                foreach (var part in parts)
                {
                    var result = part.Trim(remove);
                    if(string.IsNullOrEmpty(result))
                        continue;
                    info.result = result;
                    info.index++;
                    yield return result;
                }
            }

            loops.Pop();
        }

        /// <summary>
        /// Retrieves the lines in a text file, one at a time.
        /// </summary>
        /// <param name="input">The name of the text file whose contents will be read by the loop.</param>
        /// <param name="output">The optional name of the file to be kept open for the duration of the loop.</param>
        /// <returns></returns>
        public static IEnumerable LoopRead(string input, string output)
        {
            if (!File.Exists(input))
                yield break;

            StreamWriter writer = null;

            if (output == "*")
            {
                writer = new StreamWriter(Console.OpenStandardOutput());
            }
            else if (!string.IsNullOrEmpty(output))
            {
                bool binary = output[0] == '*';

                if (binary)
                    output = output.Substring(1);

                if (output.Length == 0 || !File.Exists(output))
                    yield break;

                writer = new StreamWriter(File.OpenWrite(output));

                if (binary)
                    writer.NewLine = "\n";
            }

            var info = new LoopInfo { type = LoopType.File };
            loops.Push(info);
            
            var reader = File.OpenText(input);
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                if (writer != null)
                    writer.WriteLine(line);

                info.result = line;
                info.index++;

                yield return line;
            }

            loops.Pop();
        }

        /// <summary>
        /// Retrieves the specified files or folders, one at a time.
        /// </summary>
        /// <param name="pattern">The name of a single file or folder, or a wildcard pattern.</param>
        /// <param name="folders">One of the following digits, or blank to use the default:
        /// <list>
        /// <item><code>1</code> (default) folders are not retrieved (only files);</item>
        /// <item><code>1</code> all files and folders that match the wildcard pattern are retrieved;</item>
        /// <item><code>2</code> only folders are retrieved (no files).</item>
        /// </list>
        /// </param>
        /// <param name="recurse"><code>1</code> to recurse into subfolders, <code>0</code> otherwise.</param>
        /// <returns></returns>
        public static IEnumerable LoopFile(string pattern, int folders, bool recurse)
        {
            var info = new LoopInfo { type = LoopType.Directory };
            loops.Push(info);

            string[] list = Directory.GetFiles(pattern, string.Empty, recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            foreach (var file in list)
            {
                info.result = file;
                info.index++;
                yield return file;
            }

            loops.Pop();
        }

        /// <summary>
        /// Retrieves the contents of the specified registry subkey, one item at a time.
        /// </summary>
        /// <param name="root">Must be either HKEY_LOCAL_MACHINE (or HKLM), HKEY_USERS (or HKU), HKEY_CURRENT_USER (or HKCU), HKEY_CLASSES_ROOT (or HKCR), or HKEY_CURRENT_CONFIG (or HKCC).</param>
        /// <param name="key">The name of the key (e.g. Software\SomeApplication). If blank or omitted, the contents of RootKey will be retrieved.</param>
        /// <param name="subkeys">
        /// <list>
        /// <item><code>1</code> subkeys contained within Key are not retrieved (only the values);</item>
        /// <item><code>1</code> all values and subkeys are retrieved;</item>
        /// <item><code>2</code> only the subkeys are retrieved (not the values).</item>
        /// </list>
        /// </param>
        /// <param name="recurse"><code>1</code> to recurse into subkeys, <code>0</code> otherwise.</param>
        /// <returns></returns>
        public static IEnumerable LoopRegistry(string root, string key, int subkeys, bool recurse)
        {
            var info = new LoopInfo { type = LoopType.Registry };
            loops.Push(info);

            loops.Pop();

            yield break;

            // TODO: registry loop
        }

        /// <summary>
        /// Retrieves each element of an array with its key if any.
        /// </summary>
        /// <param name="array">An array or object.</param>
        /// <returns>The current element.</returns>
        public static IEnumerable LoopEach(object array)
        {
            if (array == null)
                yield break;

            var info = new LoopInfo { type = LoopType.Each };
            loops.Push(info);

            var type = array.GetType();

            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                var dictionary = (IDictionary)array;

                foreach (var key in dictionary.Keys)
                {
                    info.result = new[] { key, dictionary[key] };
                    info.index++;
                    yield return info.result;
                }
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                var enumerator = ((IEnumerable)array).GetEnumerator();

                while (enumerator.MoveNext())
                {
                    info.result = new[] { null, enumerator.Current };
                    info.index++;
                    yield return info.result;
                }
            }

            loops.Pop();
        }
    }
}
