using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace IronAHK.Rusty
{
    partial class Core
    {
        static Stack<LoopInfo> loops = new Stack<LoopInfo>();

        enum LoopType { Normal, Registry, Directory, Parse, File }

        class LoopInfo
        {
            public int index = 0;
            public LoopType type = LoopType.Normal;
            public string result = null;
        }

        public static IEnumerable Loop(int n)
        {
            var info = new LoopInfo { type = LoopType.Normal };
            loops.Push(info);

            for (int i = 0; i < n; i++)
            {
                info.index = i;
                yield return i;
            }

            loops.Pop();
        }

        public static IEnumerable LoopParse(string input, string delimiters, string omit)
        {
            var info = new LoopInfo { type = LoopType.Parse };
            loops.Push(info);

            if (delimiters.ToUpperInvariant() == "CSV") // TODO: keywords
            {
                // TODO: csv parsing
            }

            string[] parts = input.Split(delimiters.ToCharArray(), StringSplitOptions.None);
            char[] remove = omit.ToCharArray();

            foreach (string part in parts)
            {
                string result = part.Trim(remove);
                info.result = result;
                info.index++;
                yield return result;
            }

            loops.Pop();
        }

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

        public static IEnumerable LoopFile(string pattern, int folders, bool recurse)
        {
            var info = new LoopInfo { type = LoopType.Directory };
            loops.Push(info);

            string[] list = Directory.GetFiles(pattern, string.Empty, recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            foreach (string file in list)
            {
                info.result = file;
                info.index++;
                yield return file;
            }

            loops.Pop();
        }

        public static IEnumerable LoopRegistry(string root, string key, int subkeys, bool recurse)
        {
            var info = new LoopInfo { type = LoopType.Registry };
            loops.Push(info);

            loops.Pop();

            yield break;

            // TODO: registry loop
        }
    }
}
