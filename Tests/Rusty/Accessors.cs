using System;
using IronAHK.Rusty;
using NUnit.Framework;

namespace IronAHK.Tests
{
    partial class Rusty
    {
        // TODO: accessors tests

        [Test, Category("Accessors")]
        public void Accessors()
        {
            Assert.IsNotEmpty(Core.A_AhkPath, "A_AhkPath");
            Assert.IsNotEmpty(Core.A_AhkVersion, "A_AhkVersion");
            Assert.AreEqual(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Core.A_AppData, "A_AppData");


        }
    }
}
