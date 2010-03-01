using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using Microsoft.Win32;
using WixSharp;

[assembly: CLSCompliant(true)]

namespace IronAHK.Setup
{
    static class Program
    {
        static bool x64, ext;
        static int n;

        static void Main(string[] args)
        {
            x64 = false;
            Build();
            x64 = true;
            Build();

#if DEBUG
            Console.Read();
#endif
        }

        static void Build()
        {
            const string config =
#if DEBUG
 "Debug"
#else
 "Release"
#endif
;

            string name = typeof(Program).Namespace.Split(new char[] { '.' }, 2)[0];
            string path = Path.GetFullPath(string.Format("..{0}..{0}..{0}{1}{0}bin{0}" + config, Path.DirectorySeparatorChar, name));
            string target = string.Format("%ProgramFiles{2}%{0}{1}", Path.DirectorySeparatorChar, name, x64 ? "64Folder" : string.Empty);
            string version = System.IO.File.ReadAllText(string.Format("{1}{0}..{0}..{0}version.txt", Path.DirectorySeparatorChar, path)).Trim();
            string docroot = Path.GetFullPath(string.Format("{1}{0}..{0}..{0}Site{0}docs", Path.DirectorySeparatorChar, path));
            string shortcut = string.Format("%ProgramMenu%{0}{1}", Path.DirectorySeparatorChar, name);
            const string extension = "ia";

            var project = new Project();

            project.Name = name;
            project.Version = new Version(version);
            project.UI = WUI.WixUI_InstallDir;
            project.MSIFileName = x64 ? "x64" : "x86";
            project.AddRemoveProgramsIcon = string.Format("{1}{0}..{0}..{0}Site{0}favicon.ico", Path.DirectorySeparatorChar, path);

            project.LicenceFile = string.Format("{1}{0}..{0}..{0}license.rtf", Path.DirectorySeparatorChar, path);
            project.MajorUpgradeStrategy = MajorUpgradeStrategy.Default;

            n = 0;
            var docs = RecurseFolder(docroot);

            foreach (var file in docs.Files)
                if (file.Id.EndsWith("index.html"))
                    file.Shortcuts = new[] { new FileShortcut("Documentation", shortcut) };

            project.Dirs = new[]
            {
                new Dir(target,
                    new Assembly(typeof(Rusty.Core).Namespace + ".dll", true),
                    new Assembly(typeof(Scripting.IACodeProvider).Namespace + ".dll", true),
                    new WixSharp.File(name + ".exe",
                        new FileShortcut(name, shortcut),
                        new FileAssociation(extension) { ContentType = "text/plain", Description= name + " Script", Arguments = "\"%1\" %*", Icon = null }
                        ),
                    new WixSharp.File("license.txt", new FileShortcut("License", shortcut)),
                    docs)
            };

            project.RegValues = new[]
            {
                new RegValue(RegistryHive.ClassesRoot, "." + extension, "PerceivedType", "text")
            };

            // TODO: add installer path to HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths
            // TODO: add installer compile option for file association

            string wixbin = Path.GetFullPath(string.Format("..{0}..{0}WixSharp{0}Wix_bin{0}bin", Path.DirectorySeparatorChar));
            Compiler.WixLocation = wixbin;

            project.WixExtensions.Add("WixNetFxExtension");
            project.WixNamespaces.Add("xmlns:netfx=\"http://schemas.microsoft.com/wix/NetFxExtension\"");

            ext = false;
            Compiler.WixSourceGenerated += new XDocumentGeneratedDlgt(XmlGenerated);

            if (x64)
                Compiler.CandleOptions += " -arch x64 ";
            Compiler.LightOptions += " -sw217 ";

            Compiler.BuildMsi(project);
        }

        static Dir RecurseFolder(string parent)
        {
            var entities = new List<WixEntity>();

            foreach (var file in Directory.GetFiles(parent))
            {
                if (Path.GetFileName(file) == "index.xml" || Path.GetExtension(file) == "xsl")
                    continue;
                entities.Add(new WixSharp.File(file) { Id = n++.ToString() + Path.GetFileName(file) });
            }

            foreach (var dir in Directory.GetDirectories(parent))
                entities.Add(RecurseFolder(dir));

            var folder = new Dir(Path.GetFileNameWithoutExtension(parent), entities.ToArray());
            return folder;
        }

        static void XmlGenerated(XDocument document)
        {
            if (ext)
                return;

            XNamespace wi = "http://schemas.microsoft.com/wix/2006/wi";
            foreach (var e in document.Descendants(wi + "Package"))
            {
                if (x64)
                    e.Add(new XAttribute("Platform", "x64"));
                e.Add(new XAttribute("AdminImage", "yes"));
            }

            foreach (var e in document.Descendants("File"))
            {
                string id = e.Attribute("Id").Value.Substring("File.".Length);

                if (!(id.EndsWith(".dll") || id.EndsWith(".exe")))
                    continue;

                var netfx = document.Root.GetNamespaceOfPrefix("netfx");
                var ngen = new XElement(netfx + "NativeImage");
                ngen.Add(new XAttribute("Id", "ngen_" + id));
                e.Add(ngen);
            }

            ext = true;
        }
    }
}
