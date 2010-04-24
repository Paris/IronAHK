using System;

namespace IronAHK.Rusty
{
    class StubGui : BaseGui
    {
        public override bool Available
        {
            get { return false; }
        }

        public override string Name
        {
            get { return "Stub"; }
        }

        public override BaseGui.Window CreateWindow()
        {
            throw new NotImplementedException();
        }
    }
}
