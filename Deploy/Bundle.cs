using System.IO;
using IronAHK.Rusty;
using IronAHK.Scripting;

namespace IronAHK.Setup
{
    partial class Program
    {
        static void AppBundle()
        {
            string name = Name + ".app";
            string zip = Path.Combine(Output, string.Format("{0}-{1}.zip", name, Version));
            string tmp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            string root = Path.Combine(tmp, name);
            string res = Path.Combine(Path.Combine(root, "Contents"), "Resources");
            string favicon = string.Format("..{0}..{0}..{0}{1}{0}Site{0}favicon.ico", Path.DirectorySeparatorChar, Name);

            if (!Directory.Exists(res))
                Directory.CreateDirectory(res);

            foreach (var dll in new[] { typeof(Core), typeof(IACodeProvider) })
                File.Copy(dll.Assembly.Location, Path.Combine(res, Path.GetFileName(dll.Assembly.Location)));

            foreach (var path in new[] { favicon, Name + ".exe" })
                File.Copy(path, Path.Combine(res, Path.GetFileName(path)));

            string osx = Path.Combine(root, "MacOS");

            if (!Directory.Exists(osx))
                Directory.CreateDirectory(osx);

            const string sh = "MacOS.sh";
            File.Copy(sh, Path.Combine(osx, Name));

            using (var writer = new StreamWriter(Path.Combine(root, "Info.plist")))
            {
                writer.NewLine = "\n";
                writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                writer.WriteLine("<!DOCTYPE plist PUBLIC \"-//Apple Computer//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">");
                writer.WriteLine("<plist version=\"1.0\">");
                writer.WriteLine("<dict>");
                writer.WriteLine("  <key>CFBundleIdentifier</key>");
                writer.WriteLine("  <string>{0}</string>", Name);
                writer.WriteLine("  <key>CFBundleExecutable</key>");
                writer.WriteLine("  <string>{0}</string>", Name);
                writer.WriteLine("  <key>CFBundleIconFile</key>");
                writer.WriteLine("  <string>{0}</string>", Path.GetFileName(favicon));
                writer.WriteLine("</dict>");
                writer.WriteLine("</plist>");
            }

            Zip(zip, name, tmp);

            Directory.Delete(tmp, true);
        }
    }
}
