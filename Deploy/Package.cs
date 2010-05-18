using System.IO;
using IronAHK.Rusty;
using IronAHK.Scripting;

namespace IronAHK.Setup
{
    partial class Program
    {
        static void PackageZip()
        {
            string zip = Path.Combine(Output, string.Format("{0}-{1}.zip", Name, Version));
            string tmp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()), target = Path.Combine(tmp, Name);

            if (!Directory.Exists(target))
                Directory.CreateDirectory(target);

            foreach (var dll in new[] { typeof(Core), typeof(IACodeProvider) })
                File.Copy(dll.Assembly.Location, Path.Combine(target, Path.GetFileName(dll.Assembly.Location)));

            foreach (var file in new[] { "license.txt", "Example.ahk", "setup.sh", Name + ".exe" })
                File.Copy(file, Path.Combine(target, file));

            Zip(zip, Path.GetFileName(target), tmp);

            Directory.Delete(tmp, true);
        }
    }
}
