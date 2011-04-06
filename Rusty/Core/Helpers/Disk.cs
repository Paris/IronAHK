using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IronAHK.Rusty
{
    partial class Core
    {
        // adapted from http://stackoverflow.com/questions/398518/how-to-implement-glob-in-c

        static IEnumerable<string> Glob(string glob)
        {
            if (File.Exists(glob) || Directory.Exists(glob))
            {
                yield return glob;
                yield break;
            }

            foreach (string path in Glob(PathHead(glob) + Path.DirectorySeparatorChar, PathTail(glob)))
            {
                yield return path;
            }
        }

        static IEnumerable<string> Glob(string head, string tail)
        {
            if (PathTail(tail) == tail)
            {
                foreach (string path in Directory.GetFiles(head, tail))
                {
                    yield return path;
                }
            }
            else
            {
                foreach (string dir in Directory.GetDirectories(head, PathHead(tail)))
                {
                    foreach (string path in Glob(Path.Combine(head, dir), PathTail(tail)))
                    {
                        yield return path;
                    }
                }
            }
        }

        static string PathHead(string path)
        {
            const int root = 2;

            if (path.StartsWith(new string(Path.DirectorySeparatorChar, root)))
            {
                var parts = path.Substring(root).Split(new[] { Path.DirectorySeparatorChar }, 2);
                var buf = new StringBuilder(root + parts[0].Length + 1 + parts[1].Length);
                buf.Append(path, 0, root);
                buf.Append(parts[0]);
                buf.Append(Path.DirectorySeparatorChar);
                buf.Append(parts[1]);
                return buf.ToString();
            }

            return path.Split(Path.DirectorySeparatorChar)[0];
        }

        static string PathTail(string path)
        {
            if (!path.Contains(Path.DirectorySeparatorChar.ToString()))
            {
                return path;
            }

            return path.Substring(1 + PathHead(path).Length);
        }
    }
}
