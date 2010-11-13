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

            using (var writer = new StreamWriter(Path.Combine(osx, Name)))
            {
                writer.NewLine = "\n";
                writer.WriteLine("#!/bin/sh");
                writer.WriteLine("PWD=`pwd`");
                writer.WriteLine("APP_PATH=`echo $0 | awk '{split($0,patharr,\"/\"); idx=1; while(patharr[idx+3] != \"\") { if (patharr[idx] != \"/\") {printf(\"%s/\", patharr[idx]); idx++ }} }'`");
                writer.WriteLine("APP_NAME=`echo $0 | awk '{split($0,patharr,\"/\"); idx=1; while(patharr[idx+1] != \"\") {idx++} printf(\"%s\", patharr[idx]); }'`");
                writer.WriteLine("ASSEMBLY=`echo $0 | awk '{split($0,patharr,\"/\"); idx=1; while(patharr[idx+1] != \"\") {idx++} printf(\"%s.exe\", patharr[idx]); }'`");
                writer.WriteLine("export MONO_MWF_USE_CARBON_BACKEND=1");
                writer.WriteLine("export GDIPLUS_NOX=1");
                writer.WriteLine("cd \"$APP_PATH/Contents/Resources\"");
                writer.WriteLine("if [ ! -d \"./bin\" ]; then mkdir bin ; fi");
                writer.WriteLine("if [ -f \"./bin/$APP_NAME\" ]; then rm -f \"./bin/$APP_NAME\" ; fi");
                writer.WriteLine("ln -s `which mono` \"./bin/$APP_NAME\" ");
                writer.WriteLine("\"./bin/$APP_NAME\" \"$ASSEMBLY\"");
            }


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
