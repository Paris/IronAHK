using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace IronAHK.Setup
{
    partial class Program
    {
        static int c;
        static List<string> components;

        static void BuildMsi()
        {
            BuildMsi(false);
            BuildMsi(true);
            components = null;
        }

        static void BuildMsi(bool x64)
        {
            string xml = Path.GetTempFileName(), proj = Path.GetTempFileName();
            var src = new StreamWriter(xml);
            BuildMsi(src, x64);
            src.Close();

            string dir = string.Format("..{0}..{0}WiX", Path.DirectorySeparatorChar.ToString());

            if (Directory.Exists(dir))
                Environment.SetEnvironmentVariable("PATH", string.Concat(Environment.GetEnvironmentVariable("PATH"), Path.PathSeparator.ToString(), dir));

            var wix = new Process { StartInfo = new ProcessStartInfo { UseShellExecute = false } };

            wix.StartInfo.FileName = "candle";
            wix.StartInfo.Arguments = string.Format("-arch x{2} -nologo -out \"{1}\" \"{0}\"", xml, proj, x64 ? "64" : "86");
            wix.Start();
            wix.WaitForExit();
            File.Delete(xml);

            string output = string.Format("{0}-{1}-x{2}.msi", Name, Version, x64 ? "64" : "86");

            if (File.Exists(output))
                File.Delete(output);

            wix.StartInfo.FileName = "light";
            wix.StartInfo.Arguments = string.Format("-nologo -sw2738 -ext WixUIExtension.dll \"{0}\" -out \"{1}\"", proj, output);
            wix.Start();
            wix.WaitForExit();
            File.Delete(proj);

            string export = Path.Combine(Output, output);

            if (File.Exists(export))
                File.Delete(export);

            File.Move(output, export);
        }

        static void BuildMsi(TextWriter writer, bool x64)
        {
            c = 0;
            components = new List<string>();
            string upgrade = Guid;

            const string config =
#if DEBUG
 "Debug"
#else
 "Release"
#endif
;

            const string author = "A";
            string name = Name;
            string version = Version;
            string path = Path.GetFullPath(string.Format("..{0}..{0}..{0}{1}{0}bin{0}" + config, Path.DirectorySeparatorChar, name));
            string docs = Path.GetFullPath(string.Format("{1}{0}..{0}..{0}Site{0}docs", Path.DirectorySeparatorChar, path));
            string favicon = string.Format("{1}{0}..{0}favicon.ico", Path.DirectorySeparatorChar.ToString(), docs);
            string main = name + ".exe";
            const string license = "license.txt";
            const string licenseRtf = "license.rtf";
            const string extension = "ia";

            writer.WriteLine("<?xml version='1.0' encoding='utf-8'?>");
            writer.WriteLine("<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi' xmlns:netfx='http://schemas.microsoft.com/wix/NetFxExtension'>");
            writer.WriteLine("  <Product Id='{0}' Name='{1}' Language='1033' Codepage='Windows-1252' Version='{2}' Manufacturer='{3}' UpgradeCode='{4}'>",
                Guid, name, version, author, upgrade);

            writer.WriteLine("    <Package InstallerVersion='200' Compressed='yes' Languages='1033' InstallScope='perMachine' Platform='x{0}' />", x64 ? "64" : "86");
            writer.WriteLine("    <Media Id='1' Cabinet='{0}.cab' EmbedCab='yes' />", name);
            writer.WriteLine("    <Directory Id='TARGETDIR' Name='SourceDir'>");
            writer.WriteLine("      <Directory Id='ProgramFiles{0}Folder' Name='ProgramFiles{0}Folder'>", x64 ? "64" : string.Empty);
            writer.WriteLine("        <Directory Id='INSTALLDIR' Name='{0}'>", name);

            foreach (var dll in new[] { typeof(Rusty.Core), typeof(Scripting.IACodeProvider) })
            {
                writer.WriteLine("          <Component Id='{0}' Guid='{1}'>", IdL, Guid);
                writer.WriteLine("            <File Id='{0}' Source='{1}' KeyPath='yes' Assembly='.net' AssemblyManifest='{0}' />", Id, dll.Assembly.Location);
                writer.WriteLine("          </Component>");
            }

            string exe = Id;
            writer.WriteLine("          <Component Id='{0}' Guid='{1}'>", IdL, Guid);
            writer.WriteLine("            <File Id='{0}' Source='{1}'>", exe, main);
            writer.WriteLine("              <Shortcut Id='{0}' WorkingDirectory='ProgramMenuFolder' Directory='ProgramMenuFolder' Name='{1}.lnk' />", Id, name);
            writer.WriteLine("            </File>");
            writer.WriteLine("            <ProgId Id='{0}' Advertise='no' Description='{1} Script'>", Id, name);
            writer.WriteLine("              <Extension Id='{0}' ContentType='text/plain'>", extension);
            writer.WriteLine("                <Verb TargetFile='{0}' Id='Open' Command='Open' Argument='&quot;%1&quot; %*' />", exe);
            writer.WriteLine("                <Verb TargetFile='{0}' Id='Compile' Command='Compile' Argument='/out ! &quot;%1&quot; %*' />", exe); 
            writer.WriteLine("              </Extension>");
            writer.WriteLine("            </ProgId>");
            writer.WriteLine("          </Component>");

            writer.WriteLine("          <Component Id='{0}' Guid='{1}'>", IdL, Guid);
            writer.WriteLine("            <File Id='{0}' Source='{1}' />", Id, license);
            writer.WriteLine("          </Component>");

            writer.WriteLine("          <Directory Id='INSTALLDIR.{0}' Name='{0}'>", Path.GetFileName(docs));
            RecurseTree(writer, docs);
            writer.WriteLine("          </Directory>");

            writer.WriteLine("          <Component Id='{0}' Guid='{1}'>", IdL, Guid);
            writer.WriteLine("            <RegistryKey Root='HKCR' Key='.{0}'>", extension);
            writer.WriteLine("              <RegistryValue Value='text' Type='string' KeyPath='yes' Name='PerceivedType' />");
            writer.WriteLine("            </RegistryKey>");
            writer.WriteLine("          </Component>");

            writer.WriteLine("        </Directory>");
            writer.WriteLine("      </Directory>");

            writer.WriteLine("      <Directory Id='ProgramMenuFolder' Name='ProgramMenuFolder'>");
            writer.WriteLine("        <Component Id='{0}' Guid='{1}' />", IdL, Guid);
            writer.WriteLine("      </Directory>");
            writer.WriteLine("    </Directory>");

            writer.WriteLine("    <Feature Id='{0}' Title='Complete' Absent='allow' Level='1'>", Id);
            foreach (string item in components)
                writer.WriteLine("      <ComponentRef Id='{0}' />", item);
            writer.WriteLine("    </Feature>");

            writer.WriteLine("    <Property Id='WIXUI_INSTALLDIR' Value='INSTALLDIR' />");
            writer.WriteLine("    <UIRef Id='WixUI_InstallDir' />");
            writer.WriteLine("    <Icon Id='MainIcon' SourceFile='{0}' />", favicon);
            writer.WriteLine("    <Property Id='ARPPRODUCTICON' Value='MainIcon' />");
            writer.WriteLine("    <WixVariable Id='WixUILicenseRtf' Value='{0}' />", licenseRtf);
            writer.WriteLine("    <Upgrade Id='{0}'>", upgrade);
            writer.WriteLine("      <UpgradeVersion Minimum='0.0.0.0' IncludeMinimum='yes' Maximum='{0}' IncludeMaximum='no' Property='UPGRADEFOUND' />", version);
            writer.WriteLine("      <UpgradeVersion Minimum='{0}' IncludeMinimum='no' OnlyDetect='yes' Property='NEWPRODUCTFOUND' />", version);
            writer.WriteLine("    </Upgrade>");
            writer.WriteLine("    <CustomAction Id='PreventDowngrading' Error='Newer version already installed' />");
            writer.WriteLine("    <InstallExecuteSequence>");
            writer.WriteLine("      <Custom Action='PreventDowngrading' After='FindRelatedProducts'>NEWPRODUCTFOUND</Custom>");
            writer.WriteLine("      <RemoveExistingProducts After='InstallFinalize' />");
            writer.WriteLine("    </InstallExecuteSequence>");
            writer.WriteLine("    <InstallUISequence>");
            writer.WriteLine("      <Custom Action='PreventDowngrading' After='FindRelatedProducts'>NEWPRODUCTFOUND</Custom>");
            writer.WriteLine("    </InstallUISequence>");

            writer.WriteLine("  </Product>");
            writer.WriteLine("</Wix>");
        }

        static void RecurseTree(TextWriter writer, string root)
        {
            RecurseTree(writer, root, root);
        }

        static void RecurseTree(TextWriter writer, string parent, string root)
        {
            int offset = Directory.GetParent(root).FullName.Length + 1;
            const string sp = "            ";

            foreach (string dir in Directory.GetDirectories(parent))
            {
                string sub = Path.GetFullPath(dir).Substring(offset).Replace(Path.DirectorySeparatorChar, '.');
                writer.WriteLine(sp + "<Directory Id='INSTALLDIR.{0}' Name='{1}'>", sub, Path.GetFileName(dir));
                RecurseTree(writer, dir, root);
                writer.WriteLine(sp + "</Directory>");
            }

            foreach (string file in Directory.GetFiles(parent))
            {
                writer.WriteLine(sp + "<Component Id='{0}' Guid='{1}'>", IdL, Guid);
                writer.WriteLine(sp + "  <File Id='{0}' Source='{1}' />", Id, file);
                writer.WriteLine(sp + "</Component>");
            }
        }

        static string Guid
        {
            get { return System.Guid.NewGuid().ToString(); }
        }

        static string Id
        {
            get { return "c" + c++.ToString(); }
        }

        static string IdL
        {
            get
            {
                string id = Id;
                
                if (components != null)
                    components.Add(id);

                return id;
            }
        }
    }
}
