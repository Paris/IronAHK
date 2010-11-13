using System.IO;
using System.Xml;
using System.Xml.Xsl;
using IronAHK.Rusty;

namespace IronAHK.Setup
{
    partial class Program
    {
        static void TransformDocs()
        {
            string root = string.Format("..{0}..{0}..{0}{1}{0}Site{0}docs{0}commands", Path.DirectorySeparatorChar, Name);

            var xsl = new XslCompiledTransform();

            using (var src = XmlReader.Create(Path.Combine(root, "view.xsl"), new XmlReaderSettings { ProhibitDtd = false }))
            {
                xsl.Load(src);
            }

            string prefix = string.Format("M:{0}.", typeof(Core).FullName), assembly = typeof(Core).Namespace;
            const string index = "index", srcName = index + ".xml", htmlName = index + ".html", member = "member";

            foreach (var path in Directory.GetDirectories(root))
            {
                RemoveCommonPaths(path, srcName, htmlName);
                Directory.Delete(path, false);
            }

            string xml = assembly + ".xml";

            if (!File.Exists(xml))
            {
                string parent = Path.GetFullPath(Path.Combine(WorkingDir, string.Format("..{0}..{0}..", Path.DirectorySeparatorChar)));
                string diff = Path.GetDirectoryName(Path.GetFullPath(xml)).Substring(parent.Length + 1);
                diff = diff.Replace("Deploy", assembly.Substring(Name.Length + 1));
                xml = Path.Combine(Path.Combine(parent, diff), xml);
            }

            using (var reader = XmlReader.Create(xml))
            {
                reader.ReadToDescendant(member);

                do
                {
                    string name = reader.GetAttribute("name");
                    int z = name.IndexOf('(');

                    if (!name.StartsWith(prefix) || z == -1 || z <= prefix.Length)
                        continue;

                    name = name.Substring(prefix.Length, z - prefix.Length);
                    string path = Path.Combine(root, name.ToLowerInvariant());

                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    RemoveCommonPaths(path, srcName, htmlName);
                    string src = Path.Combine(path, srcName), html = Path.Combine(path, htmlName);

                    using (var writer = new StreamWriter(src))
                    {
                        writer.Write(@"<?xml version='1.0'?>
<?xml-stylesheet type='text/xsl' href='../view.xsl'?>
<doc>
    <assembly>
        <name>{0}</name>
    </assembly>
    <members>
        ", assembly);
                        writer.Write(reader.ReadOuterXml());
                        writer.Write(@"
    </members>
</doc>");
                    }

                    xsl.Transform(src, html);
                }
                while (reader.ReadToNextSibling(member));
            }
        }

        static void RemoveCommonPaths(string parent, params string[] files)
        {
            foreach (var file in files)
            {
                string path = Path.Combine(parent, file);

                if (File.Exists(path))
                    File.Delete(path);
            }
        }
    }
}
